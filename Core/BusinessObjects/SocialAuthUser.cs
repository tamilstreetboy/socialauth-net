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
using Microsoft.IdentityModel.Claims;
using System.Threading;
using System.Net;

namespace Brickred.SocialAuth.NET.Core.BusinessObjects
{
    /// <summary>
    /// Represents the user in context
    /// </summary>
    public class SocialAuthUser
    {

        #region ConsumerMethods&Properties

        //--------- Constructor (Optional and is for backward compatibility)
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="providerType">Provider Type for this connection</param>
        public SocialAuthUser(PROVIDER_TYPE providerType)
        {
            this.providerType = providerType;
        }

        public SocialAuthUser() { }


        //--------- Authentication Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl">URL where user should return after login.</param>
        /// <param name="callback">Delegate invoked just before redirecting user after successful login</param>
        public void Login(PROVIDER_TYPE providerType = PROVIDER_TYPE.NOT_SPECIFIED, string returnUrl = "", Action callback = null)
        {

            if (this.providerType == PROVIDER_TYPE.NOT_SPECIFIED && providerType == PROVIDER_TYPE.NOT_SPECIFIED)
                throw new Exception("Provider not specified. Either pass provider as parameter to Login or pass it in constructor");
            if (callback != null)
                SessionManager.SetCallback(callback);
            if (providerType == PROVIDER_TYPE.NOT_SPECIFIED && this.providerType != PROVIDER_TYPE.NOT_SPECIFIED)
                providerType = this.providerType;
            Connect(providerType, returnUrl);
        }


        /// <summary>
        /// Logs user out of local application (User may still remain logged in at provider)
        /// </summary>
        /// <param name="loginUrl">Where should user be redirected after logout. (Only applicable when using custom authentication)</param>
        /// <param name="callback">Delegate invoked (if specified) just before redirecting user to login page</param>
        public void Logout(string loginUrl = "", Action callback = null)
        {
            Disconnect(loginUrl, callback);
        }


        /// <summary>
        /// If callback was specified in Login(), use this method to stop invoking delegate.
        /// </summary>
        public void ResetCallback()
        {
            SessionManager.SetCallback(null);
        }



        //------------ Getting & Checking Connections
        /// <summary>
        /// Returns an instance of SocialAuthUser with current connection
        /// </summary>
        /// <returns></returns>
        public static SocialAuthUser GetCurrentUser()
        {
            AUTHENTICATION_OPTION option = Utility.GetAuthenticationOption();

            if (SessionManager.ConnectionsCount == 0)
            {
                if (option == AUTHENTICATION_OPTION.SOCIALAUTH_SECURITY_CUSTOM_SCREEN || option == AUTHENTICATION_OPTION.SOCIALAUTH_SECURITY_SOCIALAUTH_SCREEN)
                    RedirectToLoginPage();
                else
                {
                    return new SocialAuthUser() { contextToken = new Token() };
                }
            }
            return new SocialAuthUser() { contextToken = GetCurrentConnectionToken() };
        }

        /// <summary>
        /// Is User connected with any provider?
        /// </summary>
        /// <returns></returns>
        public static bool IsLoggedIn() { return SessionManager.IsConnected; }

        /// <summary>
        /// Get Access Token for Current or specified provider
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public string GetAccessToken(PROVIDER_TYPE providerType = PROVIDER_TYPE.NOT_SPECIFIED)
        {
            if (providerType == PROVIDER_TYPE.NOT_SPECIFIED)
            {
                return SessionManager.GetConnectionToken(SessionManager.GetCurrentConnection().ProviderType).AccessToken;
            }
            else
            {
                if (IsConnectedWith(providerType))
                    return SessionManager.GetConnectionToken(providerType).AccessToken;
                else
                    throw new InvalidSocialAuthConnectionException(providerType);
            }
        }

        /// <summary>
        /// PROVIDER_TYPE for last connected
        /// </summary>
        public static PROVIDER_TYPE CurrentProvider
        {
            get
            {
                if (IsLoggedIn())
                    return SessionManager.GetCurrentConnection().ProviderType;
                else
                    return PROVIDER_TYPE.NOT_SPECIFIED;
            }
        }


        /// <summary>
        /// specifies whether user is connected with specified provider
        /// </summary>
        /// <param name="providerType">Provider Type</param>
        /// <returns></returns>
        public static bool IsConnectedWith(PROVIDER_TYPE providerType)
        {
            return SessionManager.IsConnectedWith(providerType);
        }

        /// <summary>
        /// Returns connection for specified provider. (throws exception is not connected)
        /// </summary>
        /// <param name="provider">Provider Type</param>
        /// <returns></returns>
        public IProvider GetConnection(PROVIDER_TYPE provider)
        {
            return ProviderFactory.GetProvider(provider);
        }

        /// <summary>
        /// Returns a list of all types of providers, user is connected with
        /// </summary>
        /// <returns></returns>
        public static List<PROVIDER_TYPE> GetConnectedProviders()
        {
            return SessionManager.GetConnectedProviders();
        }


        //------------ Data Retrieval Methods
        /// <summary>
        /// Returns Profile from current connection or specified provider
        /// </summary>
        /// <param name="providerType">Provider Type (Connection should exist else exception is thrown)</param>
        /// <returns></returns>
        public UserProfile GetProfile(PROVIDER_TYPE providerType = PROVIDER_TYPE.NOT_SPECIFIED)
        {
            if (providerType != PROVIDER_TYPE.NOT_SPECIFIED)
            {

                if (SessionManager.IsConnectedWith(providerType))
                {
                    if (GetConnection(providerType).GetConnectionToken().Profile.IsSet)
                        return GetConnection(providerType).GetConnectionToken().Profile;
                    else
                        return SessionManager.GetConnection(providerType).GetProfile();
                }
                else
                {
                    throw new InvalidSocialAuthConnectionException(providerType);
                }
            }
            else
            {
                if (SessionManager.IsConnected)
                {
                    if (GetCurrentConnectionToken().Profile.IsSet)
                        return GetCurrentConnectionToken().Profile;
                    else
                        return SessionManager.GetCurrentConnection().GetProfile();
                }

                else
                {
                    throw new InvalidSocialAuthConnectionException();
                }
            }
        }

        /// <summary>
        /// Returns contacts from current connection or specified provider
        /// </summary>
        /// <param name="providerType">Provider Type (Connection should exist else exception is thrown)</param>
        /// <returns></returns>
        public List<Contact> GetContacts(PROVIDER_TYPE providerType = PROVIDER_TYPE.NOT_SPECIFIED)
        {
            if (providerType != PROVIDER_TYPE.NOT_SPECIFIED)
            {

                if (SessionManager.IsConnectedWith(providerType))
                {
                    return GetConnection(providerType).GetContacts();
                }
                else
                {
                    throw new InvalidSocialAuthConnectionException(providerType);
                }
            }
            else
            {
                if (SessionManager.IsConnected)
                {

                    return CurrentConnection.GetContacts();
                }

                else
                {
                    throw new InvalidSocialAuthConnectionException();
                }
            }
        }

        ///// <summary>
        ///// Execute data feed with current or specified provider
        ///// </summary>
        ///// <param name="feedUrl"></param>
        ///// <param name="transportMethod"></param>
        ///// <returns></returns>
        public WebResponse ExecuteFeed(string feedUrl, TRANSPORT_METHOD transportMethod, PROVIDER_TYPE providerType = PROVIDER_TYPE.NOT_SPECIFIED)
        {
            if (providerType != PROVIDER_TYPE.NOT_SPECIFIED)
            {

                if (SessionManager.IsConnectedWith(providerType))
                {
                    IProvider provider = ProviderFactory.GetProvider(providerType);
                    return provider.ExecuteFeed(feedUrl, transportMethod);
                }
                else
                {
                    throw new InvalidSocialAuthConnectionException(providerType);
                }
            }
            else
            {
                if (SessionManager.IsConnected)
                {
                    IProvider p = CurrentConnection;
                    return p.ExecuteFeed(feedUrl, transportMethod);
                }

                else
                {
                    throw new InvalidSocialAuthConnectionException();
                }
            }

        }



        //------------ Future Methods
        //internal static void Disconnect(PROVIDER_TYPE providerType);
        //internal static void RefreshToken()
        //internal static void Connect(Access Token)
        //internal static void SwitchConnection(PROVIDER_TYPE)
        // & more....

        #endregion

        #region InternalStuff


        PROVIDER_TYPE providerType { get; set; }
        internal Token contextToken { get; set; }

        /// <summary>
        /// Connects to a provider (Same as Login())
        /// </summary>
        /// <param name="providerType">Provider to which connection has to be established</param>
        /// <param name="returnURL">Optional URL where user will be redirected after login (for this provider only)</param>
        internal static void Connect(PROVIDER_TYPE providerType, string returnURL = "")
        {
            returnURL = returnURL ?? "";
            if (!returnURL.ToLower().StartsWith("http"))
                returnURL = HttpContext.Current.Request.GetBaseURL() + returnURL;

            try
            {

                //User is already connected. return or redirect
                if (IsConnectedWith(providerType))
                {
                    if (!string.IsNullOrEmpty(returnURL))
                        SocialAuthUser.Redirect(returnURL);
                    return;
                }

                AUTHENTICATION_OPTION option = Utility.GetAuthenticationOption();

                //Set where user should be redirected after successful login
                if (Utility.GetAuthenticationOption() == AUTHENTICATION_OPTION.CUSTOM_SECURITY_CUSTOM_SCREEN
                     && string.IsNullOrEmpty(returnURL))
                    throw new Exception("Please specify return URL");
                else if (option == AUTHENTICATION_OPTION.SOCIALAUTH_SECURITY_CUSTOM_SCREEN || option == AUTHENTICATION_OPTION.SOCIALAUTH_SECURITY_SOCIALAUTH_SCREEN)
                {   //User has not specified and explicit return url. redirect to url from configuration
                    if (string.IsNullOrEmpty(returnURL))
                        returnURL = HttpContext.Current.Request.GetBaseURL() + Utility.GetSocialAuthConfiguration().Authentication.DefaultUrl;
                }

                //ReturnURL in request takes all priority
                if (HttpContext.Current.Request["ReturnUrl"] != null)
                {
                    string ret = HttpContext.Current.Request["ReturnUrl"];
                    if (!ret.ToLower().StartsWith("http"))
                        returnURL = HttpContext.Current.Request.GetBaseURL() + HttpUtility.UrlDecode(HttpContext.Current.Request["ReturnUrl"]);
                    else
                        returnURL = ret;
                }

                SessionManager.InProgressToken = (new Token()
                {
                    Provider = providerType,
                    Domain = HttpContext.Current.Request.GetBaseURL(),
                    UserReturnURL = returnURL,
                    SessionGUID = SessionManager.GetUserSessionGUID(),
                    Profile = new UserProfile() { Provider = providerType }

                });
                SessionManager.InProgressToken.Profile.Provider = providerType;

                ((IProviderConnect)ProviderFactory.GetProvider(providerType)).Connect();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Returns connection for last connected provider
        /// </summary>
        internal static IProvider CurrentConnection
        {
            get
            {
                return SessionManager.GetCurrentConnection();
            }
        }


        /// <summary>
        /// Logs user out of local application (User may still remain logged in at provider)
        /// </summary>
        /// <param name="loginUrl">Where should user be redirected after logout. (Only applicable when using custom authentication)</param>
        /// <param name="callback">Delegate invoked (if specified) just before redirecting user to login page</param>
        internal static void Disconnect(string loginUrl = "", Action callback = null)
        {
            //Remove all tokens
            if (HttpContext.Current.Session != null)
                SessionManager.RemoveAllConnections();

            //cleanup any cookie
            HttpContext.Current.Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
            FormsAuthentication.SignOut();

            //Redirect to login Page
            if (callback != null)
                callback.Invoke();

            RedirectToLoginPage(loginUrl);

        }


        /// <summary>
        /// Returns a GUID to identify current user
        /// </summary>
        public Guid Identifier { get { return SessionManager.GetUserSessionGUID(); } }

        /// <summary>
        /// Callbed by Authentication Strategy at end of authentication process
        /// </summary>
        /// <param name="isSuccess">Is authentication successful</param>
        internal static void OnAuthneticationProcessCompleted(bool isSuccess)
        {



            SessionManager.AddConnectionToken(SessionManager.InProgressToken);
            //LoadProfile
            SessionManager.GetCurrentConnection().GetConnectionToken().Profile = SessionManager.GetCurrentConnection().GetProfile();
            SetClaims();



            if (Utility.GetAuthenticationMode() == System.Web.Configuration.AuthenticationMode.None ||
               Utility.GetAuthenticationMode() == System.Web.Configuration.AuthenticationMode.Windows)
            {
                FormsAuthenticationTicket ticket =
                    new FormsAuthenticationTicket(SessionManager.GetUserSessionGUID().ToString(), false, HttpContext.Current.Session.Timeout);

                string EncryptedTicket = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, EncryptedTicket);
                HttpContext.Current.Response.Cookies.Add(cookie);
                SessionManager.ExecuteCallback();
                SocialAuthUser.Redirect(GetCurrentConnectionToken().UserReturnURL);
            }
            else if (Utility.GetAuthenticationMode() == System.Web.Configuration.AuthenticationMode.Forms)
            {
                SessionManager.ExecuteCallback();
                FormsAuthentication.RedirectFromLoginPage(SessionManager.GetUserSessionGUID().ToString(), false);
            }


        }

        /// <summary>
        /// Sets Windows Identify Foundatin Claims
        /// </summary>
        internal static void SetClaims()
        {
            if (HttpContext.Current.ApplicationInstance.IsSTSaware())
            {
                //Set Claims
                IClaimsPrincipal principal = (IClaimsPrincipal)Thread.CurrentPrincipal;
                IClaimsIdentity identity = (IClaimsIdentity)principal.Identity;

                UserProfile Profile = GetCurrentConnectionToken().Profile;
                if (!string.IsNullOrEmpty(Profile.DateOfBirth))
                    identity.Claims.Add(new Claim(ClaimTypes.DateOfBirth.ToString(), Profile.DateOfBirth, "string", "SocialAuth.NET", Profile.Provider.ToString()));
                if (!string.IsNullOrEmpty(Profile.FirstName))
                    identity.Claims.Add(new Claim(ClaimTypes.GivenName.ToString(), Profile.FirstName, "string", "SocialAuth.NET", Profile.Provider.ToString()));
                if (!string.IsNullOrEmpty(Profile.LastName))
                    identity.Claims.Add(new Claim(ClaimTypes.Surname.ToString(), Profile.LastName, "string", "SocialAuth.NET", Profile.Provider.ToString()));
                if (!string.IsNullOrEmpty(Profile.Email))
                    identity.Claims.Add(new Claim(ClaimTypes.Email.ToString(), Profile.Email, "string", "SocialAuth.NET", Profile.Provider.ToString()));
                if (!string.IsNullOrEmpty(Profile.Gender))
                    identity.Claims.Add(new Claim(ClaimTypes.Gender.ToString(), Profile.Gender, "string", "SocialAuth.NET", Profile.Provider.ToString()));
                if (!string.IsNullOrEmpty(Profile.Country))
                    identity.Claims.Add(new Claim(ClaimTypes.Country.ToString(), Profile.Country, "string", "SocialAuth.NET", Profile.Provider.ToString()));


            }
        }

        /// <summary>
        /// Redirect function
        /// </summary>
        /// <param name="url"></param>
        internal static void Redirect(string url)
        {
            HttpContext.Current.Response.Redirect(url, false);
        }

        /// <summary>
        /// Redirects user to Login Screen based on authentication option chosen
        /// </summary>
        /// <param name="loginUrl"></param>
        internal static void RedirectToLoginPage(string loginUrl = "")
        {
            /***************LOGIC***************
             * If AuthenticationMode = SocialAuth 
             *      and LoginUrl == empty, redirect to loginform.sauth 
             *      and LoginUrl <> empty, redirect to LoginUrl
             * If AuthenticationMode = FormsAuthentication call RedirectToLoginPage()
             * If AuthenticationMode = Custom, redirect to Parameter passed in Login
             * ********************************/

            string loginUrlInConfigFile = Utility.GetSocialAuthConfiguration().Authentication.LoginUrl;
            string redirectTo = "?ReturnUrl=" + HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString());

            AUTHENTICATION_OPTION option = Utility.GetAuthenticationOption();

            //* If AuthenticationMode = SocialAuth and LoginUrl == empty, redirect to loginform.sauth
            if (option == AUTHENTICATION_OPTION.SOCIALAUTH_SECURITY_SOCIALAUTH_SCREEN)
                SocialAuthUser.Redirect(HttpContext.Current.Request.GetBaseURL() + "socialauth/loginForm.sauth" + redirectTo);

            //* If AuthenticationMode = SocialAuth and LoginUrl <> empty, redirect to LoginUrl
            else if (option == AUTHENTICATION_OPTION.SOCIALAUTH_SECURITY_CUSTOM_SCREEN)
                SocialAuthUser.Redirect(HttpContext.Current.Request.GetBaseURL() + loginUrlInConfigFile + (redirectTo.EndsWith(loginUrlInConfigFile) ? "" : redirectTo));

            //* If AuthenticationMode = FormsAuthentication call RedirectToLoginPage()
            else if (option == AUTHENTICATION_OPTION.FORMS_AUTHENTICATION)
                FormsAuthentication.RedirectToLoginPage();

            //* If AuthenticationMode = Custom, redirect to Configuration LoginURL OR otherwise SamePage as current request
            else if (option == AUTHENTICATION_OPTION.CUSTOM_SECURITY_CUSTOM_SCREEN)
            {
                if (string.IsNullOrEmpty(loginUrl))
                    throw new Exception("Please specify Login URL");
                else
                    SocialAuthUser.Redirect(HttpContext.Current.Request.GetBaseURL() + loginUrl + redirectTo);
            }

        }

        internal static Token InProgressToken()
        {
            return SessionManager.InProgressToken;
        }

        internal static void LoginCallback(string response)
        {
            try
            {
                ((IProviderConnect)ProviderFactory.GetProvider(InProgressToken().Provider)).LoginCallback(Utility.GetQuerystringParameters(response),
                        SocialAuthUser.OnAuthneticationProcessCompleted);
            }
            catch
            {
                throw;
            }

        }

        /// <summary>
        /// Returns Connection Token of last connected provider
        /// </summary>
        /// <returns></returns>
        internal static Token GetCurrentConnectionToken()
        {
            if (SessionManager.IsConnected)
                return SessionManager.GetConnectionToken(SessionManager.GetCurrentConnection().ProviderType);
            else
                return null;
        }

        #endregion
    }
}