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
using OAuth;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Web;
using System.Xml.Linq;

namespace Brickred.SocialAuth.NET.Core.Wrappers
{
    class YahooWrapper : Provider, IProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(YahooWrapper));

        #region CONFIGURATION_PROPERTIES

        public override BusinessObjects.PROVIDER_TYPE ProviderType
        {
            get { return BusinessObjects.PROVIDER_TYPE.YAHOO; }
        }

        public override string RequestTokenURL
        {
            get { return "https://open.login.yahooapis.com/openid/op/auth"; }
        }

        public override string AuthorizationTokenURL
        {
            get { return ""; }
        }

        public override SIGNATURE_TYPE SignatureMethod
        {
            get { return SIGNATURE_TYPE.HMACSHA1; }
        }

        public override string ProfileEndpoint
        {
            get { return "http://social.yahooapis.com/v1/user/{0}/profile"; }
        }

        public override BusinessObjects.TRANSPORT_METHOD TransportName
        {
            get { return TRANSPORT_METHOD.GET; }
        }

        public override string AccessTokenURL
        {
            get { return "https://api.login.yahoo.com/oauth/v2/get_token"; }
        }

        public override string ContactsEndpoint
        {
            get { return "http://social.yahooapis.com/v1/user/{0}/contacts"; }
        }

        public override string ProfilePictureEndpoint
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region OAUTH_WORKFLOW_METHODS

        public override void RequestUserAuthentication()
        {

            NameValueCollection data = new NameValueCollection();
            data.Clear();

            ////TODO: Perform a google Discovery
            data.Add("openid.ns", "http://specs.openid.net/auth/2.0");
            data.Add("openid.mode", "checkid_setup");
            data.Add("openid.claimed_id", "http://specs.openid.net/auth/2.0/identifier_select");
            data.Add("openid.identity", "http://specs.openid.net/auth/2.0/identifier_select");
            data.Add("openid.return_to", Current.Request.GetBaseURL() + "socialAuth/Validate.sauth");
            data.Add("openid.realm", Current.Request.GetBaseURL());
            data.Add("openid.ns.ax", "http://openid.net/srv/ax/1.0");
            data.Add("openid.ax.mode", "fetch_request");
            data.Add("openid.ax.type.country", "http://axschema.org/contact/country/home");
            data.Add("openid.ax.type.email", "http://axschema.org/contact/email");
            data.Add("openid.ax.type.fullname", "http://axschema.org/namePerson");
            data.Add("openid.ax.type.language", "http://axschema.org/pref/language");
            data.Add("openid.ax.type.gender", "http://axschema.org/schema/gender");
            data.Add("openid.ax.type.image", "http://axschema.org/media/image/default");
            data.Add("openid.ax.required", "country,email,fullname,language,gender,image");

            //add Oauth Protocols to make this call hybrid
            data.Add("openid.ns.ext2", "http://specs.openid.net/extensions/oauth/1.0");
            data.Add("openid.ext2.consumer", Consumerkey);

            //Redirect user
            string processedLoginUrl = Utility.http_build_query(data);
            logger.LogAuthenticationRequest(processedLoginUrl);
            Current.Response.Redirect(RequestTokenURL + "?" + processedLoginUrl);
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
                ContextToken.SessionGUID = HttpUtility.ParseQueryString(authToken)["xoauth_yahoo_guid"];
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
            if (HttpContext.Current.Request["openid.oauth.request_token"] != null)
            {
                logger.LogAuthenticationResponse(Current.Request.ToString(), null);
                //In Hybrid mode, token recevied is both authenticated and authorized

                SocialAuthUser.GetCurrentUser().contextToken.AuthenticationToken = Current.Request["openid.oauth.request_token"];
                SocialAuthUser.GetCurrentUser().contextToken.AuthorizationToken = Current.Request["openid.oauth.request_token"];
                string Email = Current.Request["openid.ax.value.email"];
                ////Access Token Required
                (ProviderFactory.GetProvider(PROVIDER_TYPE.YAHOO)).AuthorizeUser();
                SocialAuthUser.GetCurrentUser().Profile = (ProviderFactory.GetProvider(PROVIDER_TYPE.YAHOO)).GetProfile();
                SocialAuthUser.GetCurrentUser().Profile.Email = Email;
                SocialAuthUser.GetCurrentUser().HasUserLoggedIn = true;

            }
        }

        #endregion

        #region DATA_RETRIEVAL_METHODS

        public override BusinessObjects.UserProfile GetProfile()
        {
            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string outUrl = "";
            string queryString = "";

            string sig = oAuth.GenerateSignature(new Uri(string.Format(ProfileEndpoint,ContextToken.SessionGUID)),
               Consumerkey, Consumersecret,
               Utility.UrlEncode(ContextToken.AccessToken), Utility.UrlEncodeForSigningRequest(ContextToken.AccessTokenSecret),
                TransportName.ToString(), timeStamp, nonce,
                SignatureMethod, "", "", out outUrl, out queryString);

            sig = Utility.UrlEncodeForSigningRequest(sig);

            string rawProfileData = "";
            StringBuilder sb = new StringBuilder(string.Format(ProfileEndpoint,ContextToken.SessionGUID) + "?");
            sb.AppendFormat("oauth_consumer_key={0}&", Consumerkey);
            sb.AppendFormat("oauth_token={0}&", Utility.UrlEncodeForSigningRequest(ContextToken.AccessToken));
            sb.AppendFormat("oauth_signature_method={0}&", "HMAC-SHA1");
            sb.AppendFormat("oauth_signature={0}&", sig);
            sb.AppendFormat("oauth_timestamp={0}&", timeStamp);
            sb.AppendFormat("oauth_nonce={0}&", nonce);
            sb.AppendFormat("oauth_version=1.0");

            logger.LogProfileRequest(sb.ToString());

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(sb.ToString());
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                    rawProfileData = reader.ReadToEnd();

                //Parse raw response
                XDocument xDoc = XDocument.Parse(rawProfileData);
                UserProfile profile = new UserProfile();
                XNamespace xn = xDoc.Root.GetDefaultNamespace();
                profile.FirstName = xDoc.Root.Element(xn + "givenName").Value;
                profile.LastName = xDoc.Root.Element(xn + "familyName").Value;
                profile.DateOfBirth = xDoc.Root.Element(xn + "birthdate").Value;
                profile.Country = xDoc.Root.Element(xn + "location").Value;
                profile.ProfileURL = xDoc.Root.Element(xn + "profileUrl").Value;
                profile.ProfilePictureURL = xDoc.Root.Element(xn + "image").Element(xn + "imageUrl").Value;
                profile.Language = xDoc.Root.Element(xn + "lang").Value;
                profile.Gender = xDoc.Root.Element(xn + "gender").Value;
                logger.LogProfileResponse( null);
                return profile;
            }
            catch (Exception ex)
            {
                logger.LogProfileResponse( ex);
                throw;
            }


        }

        public override List<BusinessObjects.Contact> GetContacts()
        {
            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();
            string timeStamp = oAuth.GenerateTimeStamp();
            string outUrl = "";
            string queryString = "";

            string sig = oAuth.GenerateSignature(new Uri(string.Format(ContactsEndpoint,ContextToken.SessionGUID)),
               Consumerkey, Consumersecret,
               Utility.UrlEncode(ContextToken.AccessToken), Utility.UrlEncodeForSigningRequest(ContextToken.AccessTokenSecret),
                TransportName.ToString(), timeStamp, nonce,
                SignatureMethod, "", "", out outUrl, out queryString);

            sig = Utility.UrlEncodeForSigningRequest(sig);

            string rawContactsData = "";
            StringBuilder sb = new StringBuilder(string.Format(ContactsEndpoint,ContextToken.SessionGUID) + "?");
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
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                    rawContactsData = reader.ReadToEnd();

                //Extract information from XML
                List<Contact> contacts = new List<Contact>();
                XDocument xdoc = XDocument.Parse(rawContactsData);
                XNamespace xn = xdoc.Root.GetDefaultNamespace();
                XNamespace attxn = "http://www.yahooapis.com/v1/base.rng";

                xdoc.Root.Descendants(xdoc.Root.GetDefaultNamespace() + "contact").ToList().ForEach(x =>
                 {
                     IEnumerable<XElement> contactFields = x.Elements(xn + "fields").ToList();
                     foreach (var field in contactFields)
                     {
                         Contact contact = new Contact();

                         if (field.Attribute(attxn + "uri").Value.Contains("/yahooid/"))
                         {
                             //contact.Name = field.Element(xn + "value").Value;
                             //contact.Email = field.Element(xn + "value").Value + "@yahoo.com";
                         }
                         else if (field.Attribute(attxn + "uri").Value.Contains("/name/"))
                         {
                             //Contact c = contacts.Last<Contact>();
                             //c.Name = field.Element(xn + "value").Element(xn + "givenName").Value + " " + field.Element(xn + "value").Element(xn + "familyName").Value;
                             //contacts[contacts.Count - 1] = c;
                             //continue;
                         }
                         else if (field.Attribute(attxn + "uri").Value.Contains("/email/"))
                         {
                             contact.Name = field.Element(xn + "value").Value.Replace("@yahoo.com", "");
                             contact.Email = field.Element(xn + "value").Value;
                         }
                         if (!string.IsNullOrEmpty(contact.Name) && !contacts.Exists(y => y.Email == contact.Email))
                             contacts.Add(contact);
                     }

                 }
                     );
                logger.LogContactsResponse(null);
                return contacts;
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
