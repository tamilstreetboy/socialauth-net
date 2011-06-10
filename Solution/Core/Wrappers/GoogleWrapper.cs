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
using OAuth;
using System.Web;
using System.Net;
using System.IO;
using Brickred.SocialAuth.NET.Core.BusinessObjects;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace Brickred.SocialAuth.NET.Core.Wrappers
{
    public class GoogleWrapper : Provider, IProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GoogleWrapper));
        #region CONFIGURATION_PROPERTIES

        public override BusinessObjects.PROVIDER_TYPE ProviderType
        {
            get { return BusinessObjects.PROVIDER_TYPE.GOOGLE; }
        }

        public override string ProfilePictureEndpoint
        {
            get { throw new NotImplementedException(); }
        }

        public override TRANSPORT_METHOD TransportName
        {
            get { return TRANSPORT_METHOD.GET; }
        }

        public override string RequestTokenURL
        {
            get { return "https://www.google.com/accounts/o8/ud"; }
        }

        public override string AuthorizationTokenURL
        {
            get { throw new NotImplementedException(); }
        }

        public override string AccessTokenURL
        {
            get { return "https://www.google.com/accounts/OAuthGetAccessToken"; ;}
        }

        public override SIGNATURE_TYPE SignatureMethod
        {
            get { return SIGNATURE_TYPE.HMACSHA1; }
        }

        public override string ContactsEndpoint
        {
            get { return "http://www.google.com/m8/feeds/contacts/default/full/"; }
        }

        public override string ProfileEndpoint
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region OAUTH_WORKFLOW_METHODS

        public override void RequestUserAuthentication()
        {

            NameValueCollection data = new NameValueCollection();
            data.Clear();

            //TODO: Perform a google Discovery
            data.Add("openid.ns", "http://specs.openid.net/auth/2.0");
            data.Add("openid.mode", "associate");
            data.Add("openid.assoc_type", "HMAC-SHA1");
            data.Add("openid.session_type", "no-encryption");
            string assocHandle = Utility.getAssociationHandle("https://www.google.com/accounts/o8/ud" + "?" + Utility.http_build_query(data));
            data.Clear();
            data.Add("openid.ns", "http://specs.openid.net/auth/2.0");
            data.Add("openid.claimed_id", "http://specs.openid.net/auth/2.0/identifier_select");
            data.Add("openid.identity", "http://specs.openid.net/auth/2.0/identifier_select");
            data.Add("openid.return_to", Current.Request.GetBaseURL() + "socialAuth/Validate.sauth");
            data.Add("openid.realm", Current.Request.GetBaseURL());
            data.Add("openid.assoc_handle", assocHandle);
            data.Add("openid.mode", "checkid_setup");
            SocialAuthUser.GetCurrentUser().contextToken.AssocHandle = assocHandle;
            data.Add("openid.ns.pape", "http://specs.openid.net/extensions/pape/1.0");
            data.Add("openid.ns.max_auth_age", "0");
            data.Add("openid.ns.ax", "http://openid.net/srv/ax/1.0");
            data.Add("openid.ax.mode", "fetch_request");
            data.Add("openid.ax.type.country", "http://axschema.org/contact/country/home");
            data.Add("openid.ax.type.email", "http://axschema.org/contact/email");
            data.Add("openid.ax.type.firstname", "http://axschema.org/namePerson/first");
            data.Add("openid.ax.type.language", "http://axschema.org/pref/language");
            data.Add("openid.ax.type.lastname", "http://axschema.org/namePerson/last");
            data.Add("openid.ax.required", "country,email,firstname,language,lastname");
            //ADDING OAUTH PROTOCOLS
            data.Add("openid.ns.ext2", "http://specs.openid.net/extensions/oauth/1.0");
            data.Add("openid.ext2.consumer", Consumerkey);
            data.Add("openid.ext2.scope", "http://www.google.com/m8/feeds/");
            string processedUrl = Utility.http_build_query(data);
            logger.LogAuthenticationRequest(processedUrl);
            Current.Response.Redirect(RequestTokenURL + "?" + processedUrl);

        }

        public override void AuthorizeUser()
        {
            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string outUrl = "";
            string queryString = "";
            string sig = oAuth.GenerateSignature(new Uri(AccessTokenURL),
               Consumerkey, Consumersecret,
               Utility.UrlEncode(ContextToken.AuthorizationToken), string.Empty,
                TransportName.ToString(), timeStamp, nonce,
                SignatureMethod, "", "", out outUrl, out queryString);

            sig = Utility.UrlEncodeForSigningRequest(sig);
            string authToken = Utility.HttpTransferEncode(ContextToken.AuthorizationToken);

            StringBuilder sb = new StringBuilder(AccessTokenURL + "?");
            sb.AppendFormat("oauth_consumer_key={0}&", Consumerkey);
            sb.AppendFormat("oauth_token={0}&", Utility.UrlEncodeForSigningRequest(ContextToken.AuthorizationToken));
            sb.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
            sb.AppendFormat("oauth_signature={0}&", sig);
            sb.AppendFormat("oauth_timestamp={0}&", timeStamp);
            sb.AppendFormat("oauth_nonce={0}&", nonce);
            sb.AppendFormat("oauth_version=1.0");
            logger.LogAuthorizationRequest(sb.ToString());
            try
            {

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(sb.ToString());
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                    authToken = reader.ReadToEnd();
                ContextToken.AccessToken = HttpUtility.ParseQueryString(authToken)["oauth_token"];
                ContextToken.AccessTokenSecret = HttpUtility.ParseQueryString(authToken)["oauth_token_secret"];
                logger.LogAuthorizationResponse(authToken, null);
            }
            catch (Exception ex)
            {
                logger.LogAuthorizationResponse("", ex);
                throw;
            }
        }

        public override void ProcessAuthenticationResponse()
        {
            //Authenticated
            if (HttpContext.Current.Request["openid.ext2.request_token"] != null)
            {
                logger.LogAuthenticationResponse(Current.Request.ToString(), null);
                //In Hybrid mode, token recevied is both authenticated and authorized

                SocialAuthUser.GetCurrentUser().contextToken.AuthenticationToken = Current.Request["openid.ext2.request_token"];
                SocialAuthUser.GetCurrentUser().contextToken.AuthorizationToken = Current.Request["openid.ext2.request_token"];
                SocialAuthUser.GetCurrentUser().Profile = new UserProfile();
                SocialAuthUser.GetCurrentUser().Profile.FirstName = Current.Request["openid.ext1.value.firstname"];
                SocialAuthUser.GetCurrentUser().Profile.LastName = Current.Request["openid.ext1.value.lastname"];
                SocialAuthUser.GetCurrentUser().Profile.Email = Current.Request["openid.ext1.value.email"];
                SocialAuthUser.GetCurrentUser().Profile.Country = Current.Request["openid.ext1.value.country"];
                SocialAuthUser.GetCurrentUser().Profile.Language = Current.Request["openid.ext1.value.language"];
                logger.Log(LogEventType.Info, "User's profile extracted from authentication response.");
                //Access Token Required
                (ProviderFactory.GetProvider(PROVIDER_TYPE.GOOGLE)).AuthorizeUser();
                SocialAuthUser.GetCurrentUser().HasUserLoggedIn = true;

            }
            else
            {
                LoggerFactory.GetLogger(this.GetType()).Log(LogEventType.Info, "User declined request for sharing information");
                throw new Exception("User declined request for sharing information");
            }
        }

        #endregion

        #region DATA_RETRIEVAL_METHODS

        public override BusinessObjects.UserProfile GetProfile()
        {
            throw new NotImplementedException();
        }

        public override List<Contact> GetContacts()
        {
            OAuthBase oAuth = new OAuthBase();
            string nonce = "3543bfafc7e949982c06765580b4e876"; //oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string outUrl = "";
            string queryString = "";

            string sig = oAuth.GenerateSignature(new Uri("http://www.google.com/m8/feeds/contacts/default/full/?max-results=1000"),
               Consumerkey, Consumersecret,
               Utility.UrlEncode(ContextToken.AccessToken), Utility.UrlEncodeForSigningRequest(ContextToken.AccessTokenSecret),
                TransportName.ToString(), timeStamp, nonce,
               SignatureMethod, "", "", out outUrl, out queryString);

            sig = Utility.UrlEncodeForSigningRequest(sig);

            string rawContactsData = "";
            StringBuilder sb = new StringBuilder("http://www.google.com/m8/feeds/contacts/default/full/?max-results=1000&");
            sb.AppendFormat("oauth_consumer_key={0}&", Consumerkey);
            sb.AppendFormat("oauth_token={0}&", Utility.UrlEncodeForSigningRequest(ContextToken.AccessToken));
            sb.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
            sb.AppendFormat("oauth_signature={0}&", sig);
            sb.AppendFormat("oauth_timestamp={0}&", timeStamp);
            sb.AppendFormat("oauth_nonce={0}&", nonce);
            sb.AppendFormat("oauth_version=1.0");

            logger.LogContactsRequest(sb.ToString());

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(sb.ToString());
                request.Headers.Add("GData-Version: 2");
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                    rawContactsData = reader.ReadToEnd();

                XDocument contactsXML = XDocument.Parse(rawContactsData);
                XNamespace xn = "http://schemas.google.com/g/2005";
                var contacts = from c in contactsXML.Descendants(contactsXML.Root.GetDefaultNamespace() + "entry")
                               select new Contact()
                               {
                                   ID = c.Element(contactsXML.Root.GetDefaultNamespace() + "id").Value,
                                   Name = c.Element(contactsXML.Root.GetDefaultNamespace() + "title").Value,
                                   Email = c.Element(xn + "email").Attribute("address").Value
                               };
                logger.LogContactsResponse(null);
                return contacts.ToList<Contact>();
            }
            catch (Exception ex)
            {
                logger.LogContactsResponse(ex);
                throw;
            }
        }

        #endregion




       
    }
}
