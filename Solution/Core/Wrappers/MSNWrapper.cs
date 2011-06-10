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
using System.Collections.Specialized;
using System.Web;
using Brickred.SocialAuth.NET.Core.BusinessObjects;
using System.Net;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
namespace Brickred.SocialAuth.NET.Core.Wrappers
{
    class MSNWrapper : Provider, IProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(GoogleWrapper));

        #region CONFIGURATION_PROPERTIES
        
        public override string RequestTokenURL
        {
            get { return "https://consent.live.com/Connect.aspx"; }
        }

        public override string AuthorizationTokenURL
        {
            get { return ""; }
        }

        public override BusinessObjects.SIGNATURE_TYPE SignatureMethod
        {
            get { return BusinessObjects.SIGNATURE_TYPE.PLAINTEXT; }
        }

        public override BusinessObjects.TRANSPORT_METHOD TransportName
        {
            get { return BusinessObjects.TRANSPORT_METHOD.GET; }
        }

        public override string AccessTokenURL
        {
            get { return "https://consent.live.com/AccessToken.aspx"; }
        }

        public override string ContactsEndpoint
        {
            get { return "http://apis.live.net/V4.1/cid-{0}/Contacts/AllContacts?$type=portable"; }
        }

        public override string ProfileEndpoint
        {
            get { return "http://apis.live.net/V4.1/cid-{0}/Profiles/1-{1}"; }
        }

        public override string ProfilePictureEndpoint
        {
            get { throw new NotImplementedException(); }
        }

        public override BusinessObjects.PROVIDER_TYPE ProviderType
        {
            get { return BusinessObjects.PROVIDER_TYPE.MSN; }
        }

        #endregion

        #region OAUTH_WORKFLOW_METHODS


        public override void RequestUserAuthentication()
        {
            NameValueCollection data = new NameValueCollection();
            data.Clear();
            data.Add("wrap_client_id", Consumerkey);
            data.Add("wrap_callback", Current.Request.GetBaseURL() + "socialAuth/Validate.sauth");
            data.Add("wrap_scope", "WL_Contacts.View");

            string processedUrl = Utility.http_build_query(data);
            logger.LogAuthenticationRequest(processedUrl);
            Current.Response.Redirect(RequestTokenURL + "?" + processedUrl);// + "&wrap_callback=http://opensource.brickred.com/Demo/socialAuth/Validate.sauth");
        }

        public override void ProcessAuthenticationResponse()
        {
            if (HttpContext.Current.Request["wrap_verification_code"] != null)
            {
                logger.LogAuthenticationResponse(Current.Request.ToString(), null);
                //In Hybrid mode, token recevied is both authenticated and authorized

                SocialAuthUser.GetCurrentUser().contextToken.AuthenticationToken = Current.Request["wrap_verification_code"];
                (ProviderFactory.GetProvider(PROVIDER_TYPE.MSN)).AuthorizeUser();
                SocialAuthUser.GetCurrentUser().HasUserLoggedIn = true;

            }
            else
            {
                LoggerFactory.GetLogger(this.GetType()).Log(LogEventType.Info, "User declined request for sharing information");
                throw new Exception("User declined request for sharing information");
            }
        }

        public override void AuthorizeUser()
        {


            string postData = string.Format("wrap_client_id={0}&wrap_client_secret={1}&wrap_callback={2}&wrap_verification_code={3}&idtype={4}",
                    Consumerkey,
                    Consumersecret,
                  Current.Request.GetBaseURL() + "socialAuth/Validate.sauth",
                    ContextToken.AuthenticationToken,
                    "CID");
            byte[] postDataEncoded = System.Text.Encoding.UTF8.GetBytes(postData);

            WebRequest req = HttpWebRequest.Create(AccessTokenURL);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = postDataEncoded.Length;

            Stream requestStream = req.GetRequestStream();
            requestStream.Write(postDataEncoded, 0, postDataEncoded.Length);
            logger.LogAuthorizationRequest(AccessTokenURL);
            WebResponse res;

            try
            {
                res = req.GetResponse();
            }
            catch (Exception ex)
            {
                logger.LogAuthorizationResponse("", ex);
                throw;
            }
            string responseBody = null;

            using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8))
            {
                responseBody = sr.ReadToEnd();
            }

            NameValueCollection responseCollection = System.Web.HttpUtility.ParseQueryString(responseBody);
            SocialAuthUser.GetCurrentUser().contextToken.AccessToken = responseCollection["wrap_access_token"];
            SocialAuthUser.GetCurrentUser().Profile = new UserProfile() { ID = responseCollection["uid"] };
            GetProfile();

        }

        #endregion

        #region DATA_RETRIEVAL_METHODS

        public override BusinessObjects.UserProfile GetProfile()
        {
            string uid = SocialAuthUser.GetCurrentUser().Profile.ID;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format(ProfileEndpoint, uid, uid));
            request.Headers.Add(HttpRequestHeader.Authorization, "WRAP access_token=" + ContextToken.AccessToken);
            request.Accept = "application/json";
            request.ContentType = "application/json";
            logger.LogProfileRequest(request.RequestUri.ToString());
            //request.Headers.Add(HttpRequestHeader.ContentType,"text/xml");
            try
            {
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string profileResponse = reader.ReadToEnd();
                    //SocialAuthUser.GetCurrentUser().Profile.Email = profileXmlDoc.Element(xna + "Address").Value;
                    /*
                     * {"BaseUri":"http:\/\/apis.live.net\/V4.1\/",
                     * "Id":"urn:uuid:1-600E02C5B57EE1DA",
                     * "SelfLink":"cid-600e02c5b57ee1da\/Profiles\/1-600e02c5b57ee1da",
                     * "Title":"Profile","Updated":"\/Date(1302243069000)\/","Addresses":[{"CountryRegion":"India","Type":1}],
                     * "AllContactsLink":"http:\/\/contacts.apis.live.net\/V4.1\/cid-600e02c5b57ee1da\/Contacts\/AllContacts",
                     * "BirthMonth":3,"Cid":"600E02C5B57EE1DA","Emails":[{"Address":"deepakaggarwal7@hotmail.com","Type":3}],
                     * "Fashion":0,"Gender":0,"Humor":0,
                     * "MyActivitiesLink":"http:\/\/activities.apis.live.net\/V4.1\/cid-600e02c5b57ee1da\/MyActivities","RelationshipStatus":0,"StatusMessageLink":"http:\/\/psm.apis.live.net\/V4.1\/cid-600e02c5b57ee1da\/StatusMessage",
                     * "ThumbnailImageLink":"http:\/\/blufiles.storage.msn.com\/y1miVGtv2dR8hZYzvfaDRSlLgK_cd_pbBwQr-JJcmiWJo6jSuLpr4O7Q7GAPq_PTPqWSaQnB1p0jvtsNyO8Xavxcw","UxLink":"http:\/\/cid-600e02c5b57ee1da.profile.live.com\/"}
                     */
                    if (SocialAuthUser.GetCurrentUser().Profile == null)
                        SocialAuthUser.GetCurrentUser().Profile = new UserProfile();

                    UserProfile profile = SocialAuthUser.GetCurrentUser().Profile;

                    JObject profileJson = JObject.Parse(profileResponse);
                    profile.FirstName = Convert.ToString(profileJson.SelectToken("FirstName")).Replace("\"", "");
                    profile.LastName = Convert.ToString(profileJson.SelectToken("LastName")).Replace("\"", "");
                    profile.Country = Convert.ToString(profileJson.SelectToken("Location")).Replace("\"", "");
                    profile.ProfilePictureURL = Convert.ToString(profileJson.SelectToken("ThumbnailImageLink")).Replace("\"", "");
                    //profile.ProfileURL = request.RequestUri.ToString();
                    foreach (var email in profileJson.SelectToken("Emails").ToList())
                        if (email.SelectToken("Type").ToString() == "1")
                        {
                            profile.Email = email.SelectToken("Address").ToString().Replace("\"", "");
                            break;
                        }
                    string gender = Convert.ToString(profileJson.SelectToken("Gender")).Replace("\"", "");
                    profile.Gender = (gender == "0") ? "" : (gender == "1") ? "Male" : "Female";


                    logger.LogProfileRequest(null);
                    base.SetClaims();
                }
            }
            catch (Exception ex)
            {
                logger.LogProfileResponse(ex);
                throw;
            }
            return new UserProfile();
        }

        public override List<BusinessObjects.Contact> GetContacts()
        {
            string uid = SocialAuthUser.GetCurrentUser().Profile.ID;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format(ContactsEndpoint, uid));
            request.Headers.Add(HttpRequestHeader.Authorization, "WRAP access_token=" + ContextToken.AccessToken);
            request.Accept = "application/json";
            request.ContentType = "application/json";

            /*
             * {"entries":[{"birthday":"2009-03-03","connected":"false",
             *                    "emails":[{"type":"home","value":"deepak123@hotmail.com"}],
             *          "id":"urn:uuid:I2WJZOM5LJTUTO6VL62LF276HY",
             *                  "name":{"familyName":"Aggarwal","formatted":"Deepak Aggarwal","givenName":"Deepak"},
             *          "updated":"2011-04-08T07:28:58"},{"birthday":"0001-01-01","connected":"false",
             *          "id":"urn:uuid:AXUGRWBABMJU7INHZTPH5YIVPE","name":{"familyName":"User","formatted":"Test  User","givenName":"Test "},"updated":"2011-04-08T07:30:11"}],"itemsPerPage":2,"startIndex":0,"totalResults":2}
             */

            //request.Headers.Add(HttpRequestHeader.ContentType,"text/xml");
            logger.LogContactsRequest(request.RequestUri.ToString());
            try
            {
                List<Contact> contacts = new List<Contact>();

                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string rawContacts = reader.ReadToEnd();
                    JObject contactsJson = JObject.Parse(rawContacts);
                    contactsJson.SelectToken("entries").ToList().ForEach(x =>
                        {
                            contacts.Add(new Contact()
                            {
                                Name = Convert.ToString(x.SelectToken("name").SelectToken("givenName")).Replace("\"", "") + " "
                                        + Convert.ToString(x.SelectToken("name").SelectToken("familyName")).Replace("\"", ""),
                                Email = Convert.ToString(x.SelectToken("emails")) == "[]" ? "" : x.SelectToken("emails").Single().SelectToken("value").ToString().Replace("\"", "")
                            });
                        }
                        );
                }
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
