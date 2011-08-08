using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using OAuth;
using System.Net;
using System.IO;
using Brickred.SocialAuth.NET.Core.BusinessObjects;
using Newtonsoft.Json.Linq;

namespace Brickred.SocialAuth.NET.Core.Wrappers
{
    class TwitterWrapper : Provider, IProvider
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(TwitterWrapper));

        public override string RequestTokenURL
        {
            get { return "https://api.twitter.com/oauth/request_token"; }
        }

        public override string AuthorizationTokenURL
        {
            get { return "https://api.twitter.com/oauth/authorize"; }
        }

        public override BusinessObjects.SIGNATURE_TYPE SignatureMethod
        {
            get { return BusinessObjects.SIGNATURE_TYPE.HMACSHA1; }
        }

        public override BusinessObjects.TRANSPORT_METHOD TransportName
        {
            get { return BusinessObjects.TRANSPORT_METHOD.POST; }
        }

        public override string AccessTokenURL
        {
            get { return "https://api.twitter.com/oauth/access_token"; }
        }

        public override string ContactsEndpoint
        {
            get { return "http://api.twitter.com/1/friends/ids.json?screen_name={0}&cursor=-1"; }
        }

        public override string ProfileEndpoint
        {
            get { return "http://api.twitter.com/1/users/show.json?user_id="; }
        }

        public override string ProfilePictureEndpoint
        {
            get { throw new NotImplementedException(); }
        }

        public override BusinessObjects.PROVIDER_TYPE ProviderType
        {
            get { return BusinessObjects.PROVIDER_TYPE.TWITTER; }
        }

        public override void RequestUserAuthentication()
        {
            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();
            string timestamp = oAuth.GenerateTimeStamp();
            string outUrl = "";
            string queryString = "";
            string sig2 = oAuth.GenerateSignature(new Uri(RequestTokenURL),
               "GDdmIQH6jhtmLUypg82g", "MCD8BKwGdgPHvAuvgvz4EQpqDAtx89grbuNMRd7Eh98",
               string.Empty, string.Empty,
                "POST", "1272323042", "QP70eNmVz8jvdPevU3oJD2AfF7R7odC2XJcn4XlZJqk",
                SignatureMethod, "", Utility.UrlEncode("http://localhost:3005/the_dance/process_callback?service_provider_id=11"), out outUrl, out queryString);

            string sig = oAuth.GenerateSignature(new Uri(RequestTokenURL),
              Consumerkey, Consumersecret,
              string.Empty, string.Empty,
               TransportName.ToString(), timestamp, nonce,
               SignatureMethod, "", Utility.UrlEncode(Current.Request.GetBaseURL() + "SocialAuth/validate.sauth"), out outUrl, out queryString);

            sig = Utility.UrlEncodeForSigningRequest(sig);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(RequestTokenURL);
            request.Method = TransportName.ToString();
            request.Headers.Add("Authorization", @"OAuth oauth_nonce=""" + nonce + @""", oauth_callback=""" + Utility.UrlEncode(Current.Request.GetBaseURL() + "SocialAuth/validate.sauth") + @""", oauth_signature_method=""HMAC-SHA1"", oauth_timestamp=""" + timestamp + @""", oauth_consumer_key=""" + Consumerkey + @""", oauth_signature=""" + sig + @""", oauth_version=""1.0""");
            //request.ContentLength = 0;

            string oAuthResponse = "";

            using (HttpWebResponse content = (HttpWebResponse)request.GetResponse())
            using (StreamReader stream = new StreamReader(content.GetResponseStream()))
                oAuthResponse = stream.ReadToEnd();

            if (oAuthResponse.Contains("oauth_token_secret"))
            {
                var tokenParts = oAuthResponse.Split(new char[] { '&' });
                ContextToken.RequestToken = tokenParts[0].Split('=')[1];
                ContextToken.AccessTokenSecret = tokenParts[1].Split('=')[1];
                Current.Response.Redirect(AuthorizationTokenURL + "?oauth_token=" + ContextToken.RequestToken);
            }

        }

        public override void ProcessAuthenticationResponse()
        {
            string oauth_verifier = Current.Request.QueryString["oauth_verifier"];
            string oauth_token = Current.Request.QueryString["oauth_token"];
            //Exchange Request Token for an Access Token


            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();
            string timestamp = oAuth.GenerateTimeStamp();
            string outUrl = "";
            string queryString = "";
            string sig = oAuth.GenerateSignature(new Uri(AccessTokenURL),
              Consumerkey, Consumersecret,
              oauth_token, ContextToken.AccessTokenSecret,
               TransportName.ToString(), timestamp, nonce,
               SignatureMethod, "", "", out outUrl, out queryString);

            sig = Utility.UrlEncodeForSigningRequest(sig);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(AccessTokenURL);
            request.Method = TransportName.ToString();
            request.Headers.Add("Authorization", @"OAuth oauth_nonce=""" + nonce + @""", oauth_signature_method=""HMAC-SHA1"", oauth_timestamp=""" + timestamp + @""", oauth_consumer_key=""" + Consumerkey + @""", oauth_token=""" + oauth_token + @""", oauth_verifier=""" + oauth_verifier + @""", oauth_signature=""" + sig + @""", oauth_version=""1.0""");
            //request.ContentLength = 0;

            string oAuthResponse = "";

            using (HttpWebResponse content = (HttpWebResponse)request.GetResponse())
            using (StreamReader stream = new StreamReader(content.GetResponseStream()))
                oAuthResponse = stream.ReadToEnd();

            NameValueCollection TokenParts = Utility.GetQuerystringParameters(oAuthResponse);
            ContextToken.AccessToken = TokenParts["oauth_token"];
            ContextToken.AccessTokenSecret = TokenParts["oauth_token_secret"];
            SocialAuthUser.GetCurrentUser().Profile = new UserProfile();
            SocialAuthUser.GetCurrentUser().Profile.Email = TokenParts["screen_name"];
            SocialAuthUser.GetCurrentUser().Profile.ID = TokenParts["user_id"];
            SocialAuthUser.GetCurrentUser().HasUserLoggedIn = true;
            SocialAuthUser.GetCurrentUser().Profile = GetProfile();
        }

        public override void AuthorizeUser()
        {
            throw new NotImplementedException();
        }

        public override BusinessObjects.UserProfile GetProfile()
        {

            string profileURL = ProfileEndpoint + SocialAuthUser.GetCurrentUser().Profile.ID;
            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();
            string timestamp = oAuth.GenerateTimeStamp();
            string outUrl = "";
            string queryString = "";
            string sig = oAuth.GenerateSignature(new Uri(profileURL),
              Consumerkey, Consumersecret,
              ContextToken.AccessToken, ContextToken.AccessTokenSecret,
               TransportName.ToString(), timestamp, nonce,
               SignatureMethod, "", "", out outUrl, out queryString);

            sig = Utility.UrlEncodeForSigningRequest(sig);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(profileURL);
            request.Method = "GET";
            string requestHeader = @"OAuth oauth_nonce=""" + nonce + @""", oauth_signature_method=""HMAC-SHA1"", oauth_timestamp=""" + timestamp + @""", oauth_consumer_key=""" + Consumerkey + @""", oauth_token=""" + ContextToken.AccessToken + @""", oauth_signature=""" + sig + @""", oauth_version=""1.0""";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", requestHeader);
            //request.ContentLength = 0;

            string oAuthResponse = "";

            using (HttpWebResponse content = (HttpWebResponse)request.GetResponse())
            using (StreamReader stream = new StreamReader(content.GetResponseStream()))
                oAuthResponse = stream.ReadToEnd();

            JObject pObj = JObject.Parse(oAuthResponse.ToString());
            UserProfile profile = new UserProfile();
            if (pObj.SelectToken("id_str") != null)
            {
                profile.ID = pObj.SelectToken("id_str").ToString();
            }
            if (pObj.SelectToken("name") != null)
            {
                profile.FirstName = pObj.SelectToken("name").ToString().Replace("\"", "");
            }
            if (pObj.SelectToken("location") != null)
            {
                profile.Country = pObj.SelectToken("location").ToString();
            }
            if (pObj.SelectToken("screen_name") != null)
            {
                profile.Email = pObj.SelectToken("screen_name").ToString().Replace("\"", "");
            }
            if (pObj.SelectToken("lang") != null)
            {
                profile.Language = pObj.SelectToken("lang").ToString();
            }
            if (pObj.SelectToken("profile_image_url") != null)
            {
                profile.ProfilePictureURL = pObj.SelectToken("profile_image_url").ToString().Replace("\"","");
            }

            return profile;
        }

        public override List<BusinessObjects.Contact> GetContacts()
        {

            List<Contact> friendsList = new List<Contact>();
            string friendsUrl = string.Format(ContactsEndpoint, SocialAuthUser.GetCurrentUser().Profile.Email);
            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();
            string timestamp = oAuth.GenerateTimeStamp();
            string outUrl = "";
            string queryString = "";
            string sig = oAuth.GenerateSignature(new Uri(friendsUrl),
              Consumerkey, Consumersecret,
              ContextToken.AccessToken, ContextToken.AccessTokenSecret,
               TransportName.ToString(), timestamp, nonce,
               SignatureMethod, "", "", out outUrl, out queryString);

            sig = Utility.UrlEncodeForSigningRequest(sig);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(friendsUrl);
            request.Method = "GET";
            string requestHeader = @"OAuth oauth_nonce=""" + nonce + @""", oauth_signature_method=""HMAC-SHA1"", oauth_timestamp=""" + timestamp + @""", oauth_consumer_key=""" + Consumerkey + @""", oauth_token=""" + ContextToken.AccessToken + @""", oauth_signature=""" + sig + @""", oauth_version=""1.0""";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", requestHeader);
            //request.ContentLength = 0;

            string oAuthResponse = "";

            using (HttpWebResponse content = (HttpWebResponse)request.GetResponse())
            using (StreamReader stream = new StreamReader(content.GetResponseStream()))
                oAuthResponse = stream.ReadToEnd();

            var friends = JObject.Parse(oAuthResponse).SelectToken("ids").Children().ToList();
            string friendIDs = "";
            foreach (var s in friends)
                friendIDs += (s.ToString() + ",");

            List<string> sets = new List<string>();
            char[] arr = friendIDs.ToArray<char>();
            var iEnumerator = arr.GetEnumerator();
            int counter = 0;
            string temp = "";
            while (iEnumerator.MoveNext())
            {
                if (iEnumerator.Current.ToString() == ",")
                    counter += 1;
                if (counter == 100)
                {
                    sets.Add(temp);
                    temp = "";
                    counter = 0;
                    continue;
                }
                temp += iEnumerator.Current;
            }
            if (temp != "")
                sets.Add(temp);

            foreach (string set in sets)
            {

                friendsList.AddRange(Friends(set));
            }

            return friendsList;
        }


        private List<Contact> Friends(string friendUserIDs)
        {
            string lookupUrl = "http://api.twitter.com/1/users/lookup.json?user_id=" + friendUserIDs;
            OAuthBase oAuth = new OAuthBase();
            string nonce = oAuth.GenerateNonce();
            string timestamp = oAuth.GenerateTimeStamp();
            string outUrl = "";
            string queryString = "";
            string sig = oAuth.GenerateSignature(new Uri(lookupUrl),
              Consumerkey, Consumersecret,
              ContextToken.AccessToken, ContextToken.AccessTokenSecret,
               TransportName.ToString(), timestamp, nonce,
               SignatureMethod, "", "", out outUrl, out queryString);

            sig = Utility.UrlEncodeForSigningRequest(sig);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(lookupUrl);
            request.Method = "GET";
            string requestHeader = @"OAuth oauth_nonce=""" + nonce + @""", oauth_signature_method=""HMAC-SHA1"", oauth_timestamp=""" + timestamp + @""", oauth_consumer_key=""" + Consumerkey + @""", oauth_token=""" + ContextToken.AccessToken + @""", oauth_signature=""" + sig + @""", oauth_version=""1.0""";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", requestHeader);
            //request.ContentLength = 0;

            string oAuthResponse = "";

            using (HttpWebResponse content = (HttpWebResponse)request.GetResponse())
            using (StreamReader stream = new StreamReader(content.GetResponseStream()))
                oAuthResponse = stream.ReadToEnd();

            List<Contact> friends = new List<Contact>();
            JArray j = JArray.Parse(oAuthResponse);
            j.ToList().ForEach(f =>
            {
                friends.Add(
                  new Contact()
                 {
                     Name = (string)f["name"],
                     ID = (string)f["id_str"],
                     ProfileURL = "http://twitter.com/#!/" + (string)f["screen_name"]
                 });

            });
            return friends.ToList<Contact>();

        }
    }
}
