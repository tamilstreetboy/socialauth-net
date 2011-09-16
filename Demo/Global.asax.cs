using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Brickred.SocialAuth.NET.Core;
using Brickred.SocialAuth.NET.Core.BusinessObjects;
using System.Security.Principal;

namespace Brickred.SocialAuth.NET.Demo
{

    public class CustomUser : IIdentity
    {

        #region IIdentity Members

        public string AuthenticationType
        {
            get { return "Forms"; }
        }

        public bool IsAuthenticated
        {
            get { return SocialAuthUser.IsLoggedIn(); }
        }

        public string Name
        {
            get
            {
                return SocialAuthUser.CurrentConnection.GetProfile().FullName;
            }
        }
        #endregion
    }
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            log4net.ThreadContext.Properties["ID"] = "asdasd";
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            log4net.ThreadContext.Properties["ID"] ="asdasd";
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
           
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        public void FormsAuthentication_OnAuthenticate(object sender, FormsAuthenticationEventArgs args)
        {
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var x = args.Context.Session;
                //if (SocialAuthUser.GetCurrentUser() != null)
                //{
                //     args.User = new System.Security.Principal.GenericPrincipal(
                //    new CustomUser(), new string[0]);
                //    //In Data Layer: Select User from LinkedAccounts where EmailID = SocialAuthUser.GetCurrentUser().GetProfile().Email and provider = SocialAuthUser.GetProvider()
                //    // In Business Layer: Overload existing LoginRoutine which creates user an
                //    HttpContext.Current.Response.Write("I am here");
                //    Response.End();
                //}
            }
        }

        protected void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            log4net.ThreadContext.Properties["SessionID"] = Session.SessionID;
        }
    }
}