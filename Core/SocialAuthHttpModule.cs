/*
===========================================================================
Copyright (c) 2010 BrickRed Technologies Limited

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sub-license, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
===========================================================================

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Security.Principal;
using Brickred.SocialAuth.NET.Core.BusinessObjects;

namespace Brickred.SocialAuth.NET.Core
{
    class SocialAuthHttpModule : IHttpModule, IReadOnlySessionState
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        //Hook our module to httprequest pipeline
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += new EventHandler(context_AuthenticateRequest);
            context.PreRequestHandlerExecute += new EventHandler(context_PreRequestHandlerExecute);
        }

        private void context_AuthenticateRequest(object sender, EventArgs e)
        {
            string loginUrl = "";
            string defaultUrl = "";
            //string allowedURls = "";

            if (VirtualPathUtility.GetExtension(HttpContext.Current.Request.RawUrl) != ".aspx")
                return;

            //User wants SocialAuth to manage authentication
            if (Utility.GetAuthenticationMode() == System.Web.Configuration.AuthenticationMode.None &&
                Utility.GetConfiguration().Authentication.Enabled == true)
            {
                loginUrl = Utility.GetConfiguration().Authentication.LoginUrl;
                if (string.IsNullOrEmpty(loginUrl))
                    if (HttpContext.Current.Request["p"] == null)
                        loginUrl = "SocialAuth/LoginForm.sauth";
                    else
                        loginUrl = "SocialAuth/LoginAction.sauth?p=facebook";

                defaultUrl = Utility.GetConfiguration().Authentication.DefaultUrl;

                string cookieName = FormsAuthentication.FormsCookieName;
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[cookieName];
                bool isauth = HttpContext.Current.Request.IsAuthenticated;

                if (authCookie == null && (!
                        ((HttpContext.Current.Request.Url.ToString().ToLower().Contains(loginUrl.ToLower()) && loginUrl != "") ||
                    HttpContext.Current.Request.Url.ToString().ToLower().Contains(".sauth"))))
                {
                    HttpContext.Current.Response.Redirect(HttpContext.Current.Request.GetBaseURL() +  loginUrl);
                }
                else if (authCookie != null)
                {
                    HttpContext.Current.User = new GenericPrincipal(new FormsIdentity(FormsAuthentication.Decrypt(authCookie.Value)), null);

                }

            }
        }


        protected void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (HttpContext.Current.ApplicationInstance.IsSTSaware())
                if (HttpContext.Current.Session != null)
                    if (SocialAuthUser.GetCurrentUser() != null)
                        if (SocialAuthUser.GetCurrentUser().Profile != null)
                            SocialAuthUser.GetCurrentUser().SetClaims();
        }
    }
}
