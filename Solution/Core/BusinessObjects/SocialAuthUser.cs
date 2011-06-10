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

namespace Brickred.SocialAuth.NET.Core.BusinessObjects
{

    public class SocialAuthUser
    {

        #region CLASS_VARIABLES

        private SocialAuthUser contextUser;
        private IProvider provider;
        private Guid identifier;
        private string providerType;

        #endregion

        #region UTILITY_METHODS_PROPERTIES

        public static SocialAuthUser GetCurrentUser()
        {
            if (HttpContext.Current.Session["socialauthuser"] == null)
            {
                HttpContext.Current.Response.Redirect("~/socialAuth/logout.sauth");
            }
            return (SocialAuthUser)HttpContext.Current.Session["socialauthuser"];
        }

        private HttpContext current
        {
            get { return HttpContext.Current; }
        }

        internal Token contextToken { get; set; }

        internal UserProfile Profile { get; set; }

        public Guid Identifier { get { return identifier; } }

        #endregion

        #region INITIALIZATION

        public SocialAuthUser(PROVIDER_TYPE providerType)
        {
            contextUser = this;
            contextUser.provider = ProviderFactory.GetProvider(providerType);
            contextUser.contextToken = new Token();
            contextUser.contextToken.provider = providerType;
            identifier = Guid.NewGuid();
            HttpContext.Current.Session["socialauthuser"] = contextUser;
            this.providerType = providerType.ToString();
        }

        public static void CreateUser(PROVIDER_TYPE provider)
        {
            SocialAuthUser objUser = new SocialAuthUser(provider);
        }

        #endregion

        #region OAUTH_IMPLEMENTATION

        private void Login(string defaultURL)
        {
            contextUser.contextToken.CallbackURL = (defaultURL.Contains("http") ? defaultURL : current.Request.GetBaseURL() + defaultURL);
            provider.RequestUserAuthentication();
        }

        public void Login()
        {
            string callbackUrl = "";
            if (Utility.GetAuthenticationMode() == System.Web.Configuration.AuthenticationMode.None
                && Utility.GetConfiguration().Authentication.Enabled)
                callbackUrl = Utility.GetConfiguration().Authentication.DefaultUrl.Contains("http") ? Utility.GetConfiguration().Authentication.DefaultUrl
                     : current.Request.GetBaseURL() + Utility.GetConfiguration().Authentication.DefaultUrl;
            else
                callbackUrl = FormsAuthentication.DefaultUrl;

            Login(callbackUrl);
        }

        public List<Contact> GetContacts()
        {
            return provider.GetContacts();
        }

        public UserProfile GetProfile()
        {
            if (Profile == null)
                Profile = provider.GetProfile();
            return Profile;
        }

        public static bool IsLoggedIn()
        {
            if (GetCurrentUser() == null)
                return false;
            else
                return GetCurrentUser().HasUserLoggedIn;
        }

        internal bool HasUserLoggedIn
        {
            get;
            set;
        }

        public void Logout()
        {
                  HttpContext.Current.Response.Redirect("~/socialAuth/logout.sauth");
        }

        public void Logout(string callbackURL)
        {
            SocialAuthUser.GetCurrentUser().contextToken.CallbackURL = callbackURL;
            Logout();
        }

        internal void SetClaims()
        {
            Provider.SetIClaims();
        }

        public string ProviderName
        {
            get
            {
                return providerType;
            }
        }
        #endregion

    }
}
