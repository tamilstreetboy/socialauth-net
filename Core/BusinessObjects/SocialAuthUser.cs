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
    /// <summary>
    /// Represents the user in context
    /// </summary>
    public class SocialAuthUser
    {

        #region CLASS_VARIABLES

        private SocialAuthUser contextUser;
        private IProvider provider;
        private Guid identifier;
        private string providerType;

        #endregion

        #region UTILITY_METHODS_PROPERTIES
        /// <summary>
        /// Returns an instance of user in context
        /// </summary>
        /// <returns></returns>
        public static SocialAuthUser GetCurrentUser()
        {
            if (HttpContext.Current.Session["socialauthuser"] == null)
            {
                if (Utility.OperationMode() == OPERATION_MODE.SOCIALAUTH_SECURITY_SOCIALAUTH_SCREEN)
                    HttpContext.Current.Response.Redirect("~/socialAuth/logout.sauth");
                else
                {
                    return new SocialAuthUser() { HasUserLoggedIn = false, contextToken = new Token() };
                }
            }
            return (SocialAuthUser)HttpContext.Current.Session["socialauthuser"];
        }

        private HttpContext current
        {
            get { return HttpContext.Current; }
        }

        internal Token contextToken { get; set; }

        internal UserProfile Profile { get; set; }

        /// <summary>
        /// Returns a GUID to identify current user
        /// </summary>
        public Guid Identifier { get { return identifier; } }

        #endregion

        #region INITIALIZATION
        /// <summary>
        /// Initializes context user object
        /// </summary>
        /// <param name="providerType">Provider selected for authentication</param>
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

        internal SocialAuthUser()
        {
            HttpContext.Current.Session["socialauthuser"] = this;
        }


        /// <summary>
        /// Initializes context user object
        /// </summary>
        /// <param name="provider">Provider selected for authentication</param>
        public static void CreateUser(PROVIDER_TYPE provider)
        {
            SocialAuthUser objUser = new SocialAuthUser(provider);
        }

        #endregion

        #region OAUTH_IMPLEMENTATION

        public void Login(string defaultURL)
        {
            contextUser.contextToken.CallbackURL = (defaultURL.Contains("http") ? defaultURL : current.Request.GetBaseURL() + defaultURL);
            provider.RequestUserAuthentication();
        }

        /// <summary>
        /// Redirects user to provider's login for authentication
        /// </summary>
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

        /// <summary>
        /// Returns contacts list of context user
        /// </summary>
        /// <returns></returns>
        public List<Contact> GetContacts()
        {
            return provider.GetContacts();
        }

        /// <summary>
        /// Returns profile of context user
        /// </summary>
        /// <returns></returns>
        public UserProfile GetProfile()
        {
            if (Profile == null)
                Profile = provider.GetProfile();
            return Profile;
        }

        /// <summary>
        /// Returns boolean value indicating if a user is logged in
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Logs out user for current application (not provider) and redirects to login screen
        /// </summary>
        public void Logout()
        {
            
                  HttpContext.Current.Response.Redirect("~/socialAuth/logout.sauth");
        }

        /// <summary>
        /// Logs out user for current application (not provider) and redirects to specified URL
        /// </summary>
        /// <param name="callbackURL">URL where user should be redirected after logout</param>
        public void Logout(string callbackURL)
        {
            SocialAuthUser.GetCurrentUser().contextToken.CallbackURL = callbackURL;
            Logout();
        }

        internal void SetClaims()
        {
            Provider.SetIClaims();
        }

        /// <summary>
        /// Returns name of provider used for authenticating context user
        /// </summary>
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
