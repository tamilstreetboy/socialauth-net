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
using Brickred.SocialAuth.NET.Core.BusinessObjects;
using System.Web;
using Microsoft.IdentityModel.Claims;
using System.Threading;

namespace Brickred.SocialAuth.NET.Core
{

    public abstract class Provider : IProvider
    {

        #region HELPER_PROPERTIES

        protected HttpContext Current
        {
            get { return HttpContext.Current; }
        }

        internal Token ContextToken
        {
            get { return SocialAuthUser.GetCurrentUser().contextToken; }
        }

        #endregion

        #region CONFIGURATION_PROPERTIES

        public abstract string RequestTokenURL { get; }
        public abstract string AuthorizationTokenURL { get; }
        public abstract SIGNATURE_TYPE SignatureMethod { get; }
        public abstract TRANSPORT_METHOD TransportName { get; }
        public abstract string AccessTokenURL { get; }
        public abstract string ContactsEndpoint { get; }
        public abstract string ProfileEndpoint { get; }
        public abstract string ProfilePictureEndpoint { get; }
        public abstract PROVIDER_TYPE ProviderType { get; }
        public string AssocHandleURL { get; set; }
        public string Consumerkey { get; set; }
        public string Consumersecret { get; set; }
        public bool Secure { get; set; }

        #endregion

        #region OAUTH_WORKFLOW_METHODS

        public abstract void RequestUserAuthentication();
        public abstract void ProcessAuthenticationResponse();
        public abstract void AuthorizeUser();
        public virtual void Logout()
        {

        }

        #endregion

        #region DATA_RETRIEVAL_METHODS

        public abstract UserProfile GetProfile();
        public abstract List<Contact> GetContacts();

        protected void SetClaims()
        {
            if (HttpContext.Current.ApplicationInstance.IsSTSaware())
            {
                //Set Claims
                IClaimsPrincipal principal = (IClaimsPrincipal)Thread.CurrentPrincipal;
                IClaimsIdentity identity = (IClaimsIdentity)principal.Identity;

                SocialAuthUser user = SocialAuthUser.GetCurrentUser();
                if (!string.IsNullOrEmpty(user.Profile.DateOfBirth))
                    identity.Claims.Add(new Claim(ClaimTypes.DateOfBirth.ToString(), user.Profile.DateOfBirth, "string", "SocialAuth.NET", user.contextToken.provider.ToString()));
                if (!string.IsNullOrEmpty(user.Profile.FirstName))
                    identity.Claims.Add(new Claim(ClaimTypes.GivenName.ToString(), user.Profile.FirstName, "string", "SocialAuth.NET", user.contextToken.provider.ToString()));
                if (!string.IsNullOrEmpty(user.Profile.LastName))
                    identity.Claims.Add(new Claim(ClaimTypes.Surname.ToString(), user.Profile.LastName, "string", "SocialAuth.NET", user.contextToken.provider.ToString()));
                if (!string.IsNullOrEmpty(user.Profile.Email))
                    identity.Claims.Add(new Claim(ClaimTypes.Email.ToString(), user.Profile.Email, "string", "SocialAuth.NET", user.contextToken.provider.ToString()));
                if (!string.IsNullOrEmpty(user.Profile.Gender))
                    identity.Claims.Add(new Claim(ClaimTypes.Gender.ToString(), user.Profile.Gender, "string", "SocialAuth.NET", user.contextToken.provider.ToString()));
                if (!string.IsNullOrEmpty(user.Profile.Country))
                    identity.Claims.Add(new Claim(ClaimTypes.Country.ToString(), user.Profile.Country, "string", "SocialAuth.NET", user.contextToken.provider.ToString()));


            }
        }

        public static void SetIClaims()
        {
            if (HttpContext.Current.ApplicationInstance.IsSTSaware())
            {
                //Set Claims
                IClaimsPrincipal principal = (IClaimsPrincipal)Thread.CurrentPrincipal;
                IClaimsIdentity identity = (IClaimsIdentity)principal.Identity;

                SocialAuthUser user = SocialAuthUser.GetCurrentUser();
                
                if (user.contextToken == null) { return; }//This triggers during logout
                if (!string.IsNullOrEmpty(user.Profile.DateOfBirth))
                    identity.Claims.Add(new Claim(ClaimTypes.DateOfBirth.ToString(), user.Profile.DateOfBirth, "string", "SocialAuth.NET", user.contextToken.provider.ToString()));
                if (!string.IsNullOrEmpty(user.Profile.FirstName))
                    identity.Claims.Add(new Claim(ClaimTypes.GivenName.ToString(), user.Profile.FirstName, "string", "SocialAuth.NET", user.contextToken.provider.ToString()));
                if (!string.IsNullOrEmpty(user.Profile.LastName))
                    identity.Claims.Add(new Claim(ClaimTypes.Surname.ToString(), user.Profile.LastName, "string", "SocialAuth.NET", user.contextToken.provider.ToString()));
                if (!string.IsNullOrEmpty(user.Profile.Email))
                    identity.Claims.Add(new Claim(ClaimTypes.Email.ToString(), user.Profile.Email, "string", "SocialAuth.NET", user.contextToken.provider.ToString()));
                if (!string.IsNullOrEmpty(user.Profile.Gender))
                    identity.Claims.Add(new Claim(ClaimTypes.Gender.ToString(), user.Profile.Gender, "string", "SocialAuth.NET", user.contextToken.provider.ToString()));
                if (!string.IsNullOrEmpty(user.Profile.Country))
                    identity.Claims.Add(new Claim(ClaimTypes.Country.ToString(), user.Profile.Country, "string", "SocialAuth.NET", user.contextToken.provider.ToString()));

         }

        }
        #endregion
    }
}
