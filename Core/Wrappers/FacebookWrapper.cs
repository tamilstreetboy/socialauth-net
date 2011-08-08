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
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Brickred.SocialAuth.NET.Core.Wrappers
{
    /// <summary>
    /// Contains OAuth implementation for Facebook
    /// </summary>
    class FacebookWrapper : Provider, IProvider
    {

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(FacebookWrapper));

        #region CONFIGURATION_PROPERTIES

        public override string RequestTokenURL
        {
            get { return "https://www.facebook.com/dialog/oauth"; }
        }

        public override string AuthorizationTokenURL
        {
            get { return ""; }
        }

        public override SIGNATURE_TYPE SignatureMethod
        {
            get { throw new NotImplementedException(); }
        }

        public override string ContactsEndpoint
        {
            get { return "https://graph.facebook.com/me/friends"; }
        }

        public override string ProfileEndpoint
        {
            get { return "https://graph.facebook.com/me"; }
        }

        public override string AccessTokenURL
        {
            get { return "https://graph.facebook.com/oauth/access_token"; }
        }

        public override PROVIDER_TYPE ProviderType
        {
            get { return PROVIDER_TYPE.FACEBOOK; }
        }

        public override string ProfilePictureEndpoint
        {
            get { return "https://graph.facebook.com/me/picture"; }
        }

        public override TRANSPORT_METHOD TransportName
        {
            get { throw new NotImplementedException(); }
        }


        #endregion

        #region OAUTH_WORKFLOW_METHODS

        public override void RequestUserAuthentication()
        {
            UriBuilder ub = new UriBuilder(RequestTokenURL);
            ub.SetQueryparameter("client_id", Consumerkey);
            ub.SetQueryparameter("redirect_uri", Current.Request.GetBaseURL() + "SocialAuth/validate.sauth");
            ub.SetQueryparameter("scope", "email" + (string.IsNullOrEmpty(AdditionalScope) ? "" : "," + AdditionalScope));
            logger.LogAuthenticationRequest(ub.ToString());
            HttpContext.Current.Response.Redirect(ub.ToString());
        }

        public override void AuthorizeUser()
        {

            UriBuilder ub = new UriBuilder(AccessTokenURL);
            ub.SetQueryparameter("client_id", Consumerkey);
            ub.SetQueryparameter("client_secret", Consumersecret);
            ub.SetQueryparameter("code", ContextToken.RequestToken);
            ub.SetQueryparameter("redirect_uri", Current.Request.GetBaseURL() + "SocialAuth/validate.sauth");
            logger.LogAuthorizationRequest(ub.ToString());
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ub.ToString());

            string authToken = "";
            try
            {
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                    authToken = reader.ReadToEnd();
                var tokenParts = authToken.Split(new char[] { '&' });
                ContextToken.AuthorizationToken = tokenParts[0].Replace("access_token=", "");
                ContextToken.Expiry = tokenParts[1];
                logger.LogAuthorizationResponse(authToken, null);
            }
            catch (Exception ex)
            {
                logger.LogAuthorizationResponse("", ex);
            }
        }

        public override void ProcessAuthenticationResponse()
        {
            if (HttpContext.Current.Request["code"] != null)
            {
                if (Current.Request["code"] != null)
                {
                    logger.LogAuthenticationResponse(Current.Request.ToString(), null);
                    ContextToken.RequestToken = Current.Request["code"];
                    (ProviderFactory.GetProvider(PROVIDER_TYPE.FACEBOOK)).AuthorizeUser();
                    logger.Log(LogEventType.Info, "User successully logged in at Facebook");
                    SocialAuthUser.GetCurrentUser().HasUserLoggedIn = true;
                    SocialAuthUser.GetCurrentUser().Profile = GetProfile();
                    base.SetClaims();
                }
            }
        }

        #endregion

        #region DATA_RETRIEVAL_METHODS

        public override UserProfile GetProfile()
        {

            UserProfile profile = new UserProfile();
            UriBuilder ub;

            /******** retrieve standard Fields ************/
            ub = new UriBuilder(ProfileEndpoint + "?");
            ub.SetQueryparameter("access_token", ContextToken.AuthorizationToken);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ub.ToString());
            logger.LogContactsRequest(ub.ToString());
            using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
            using (Stream responseStream = webResponse.GetResponseStream())
                try
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        string output = reader.ReadToEnd();
                        JObject jsonObject = JObject.Parse(output);
                        profile.FirstName = jsonObject.SelectToken("first_name").ToString();
                        profile.LastName = jsonObject.SelectToken("last_name").ToString();
                        string[] locale = jsonObject.SelectToken("locale").ToString().Split(new char[] { '_' });
                        if (locale.Length > 0)
                        {
                            profile.Language = locale[0];
                            profile.Country = locale[1];
                        }
                        profile.ProfileURL = jsonObject.SelectToken("link").ToString();
                        profile.Email = HttpUtility.UrlDecode(jsonObject.SelectToken("email").ToString());
                        profile.DateOfBirth = jsonObject.SelectToken("birthday") != null ? jsonObject.SelectToken("birthday").ToString().Replace(@"""", "") : string.Empty;
                        profile.Gender = jsonObject.SelectToken("gender").ToString();
                    }

                }
                catch (Exception ex)
                {
                    logger.Log(LogEventType.Debug, "GetProfile(), profile request failed: " + ex.Message);
                    throw;
                }

            try
            {
                /******** retrieve standard picture ************/
                ub = new UriBuilder(ProfilePictureEndpoint + "?type=large");
                ub.SetQueryparameter("access_token", ContextToken.AuthorizationToken.Replace("access_token=", ""));
                request = (HttpWebRequest)HttpWebRequest.Create(ub.ToString());
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                    profile.ProfilePictureURL = webResponse.ResponseUri.AbsoluteUri;
                logger.LogProfileResponse(null);
                return profile;
            }
            catch (Exception ex)
            {
                logger.LogProfileResponse(ex);
                throw;
            }


        }

        public override List<Contact> GetContacts()
        {
            UriBuilder ub;
            /******** retrieve standard Fields ************/
            try
            {
                ub = new UriBuilder(ContactsEndpoint + "?");
                ub.SetQueryparameter("access_token", ContextToken.AuthorizationToken);
                logger.LogContactsRequest(ub.ToString());
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ub.ToString());
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    JObject jsonObject = JObject.Parse(reader.ReadToEnd());

                    var friends = from f in jsonObject["data"].Children()
                                  select new Contact
                                  {
                                      Name = (string)f["name"],
                                      ID = (string)f["id"],
                                      ProfileURL = "http://www.facebook.com/profile.php?id=" + (string)f["id"]
                                  };
                    logger.LogContactsResponse(null);
                    return friends.ToList<Contact>();
                }

            }
            catch (Exception ex)
            {
                logger.LogContactsResponse(ex);
                throw;
            }


        }

        public override string ExecuteFeed(string url)
        {
            UriBuilder ub;
            /******** retrieve standard Fields ************/
            try
            {
                ub = new UriBuilder(url + "?");
                ub.SetQueryparameter("access_token", ContextToken.AuthorizationToken);
                logger.LogContactsRequest(ub.ToString());
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ub.ToString());
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();
                }

            }
            catch (Exception ex)
            {
               
                throw;
            }
        }

        #endregion



    }
}
