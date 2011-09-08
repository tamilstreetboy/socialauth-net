using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brickred.SocialAuth.NET.Core.BusinessObjects;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Web;

namespace Brickred.SocialAuth.NET.Core
{


    internal class OAuth2_0server : OAuthStrategyBase, IOAuth2_0
    {
        public OAuth2_0server(IProvider provider)
        {
            this.provider = provider;
            this.logger = LoggerFactory.GetLogger(ProviderFactory.GetProvider(provider.ProviderType).GetType());
        }

        //Called Before Directing User
        public override void Login()
        {
            DirectUserToServiceProvider(); //(A) (B)
        }
        //Called After Directing User
        public override void LoginCallback(QueryParameters responseCollection, Action<bool> OnAuthenticationCompletion)
        {
            HandleAuthorizationCode(responseCollection); //(C)
            string response = RequestForAccessToken(); // (D)
            HandleAccessTokenResponse(response); //(E)

            //Authentication Process is through. Inform Consumer.
            provider.AuthenticationCompleting(isSuccess); // Let Provider Know authentication process is through
            OnAuthenticationCompletion(isSuccess); // Authentication process complete. Call final method
        }

        #region Oauth2_0Implementation

        public void DirectUserToServiceProvider()
        {
            UriBuilder ub = new UriBuilder(provider.UserLoginEndpoint);
            ub.SetQueryparameter("client_id", provider.Consumerkey);
            ub.SetQueryparameter("redirect_uri", connectionToken.ProviderCallbackUrl);
            ub.SetQueryparameter("response_type", "code");
            ub.SetQueryparameter("scope", provider.GetScope());
            //logger.LogAuthenticationRequest(ub.ToString());
            SocialAuthUser.Redirect(ub.ToString());
        }

        public void HandleAuthorizationCode(QueryParameters responseCollection)
        {
            if (responseCollection["code"] != null)
            {
                connectionToken.Code = responseCollection["code"];
            }
        }

        public string RequestForAccessToken()
        {
            UriBuilder ub = new UriBuilder(provider.AccessTokenEndpoint);
            ub.SetQueryparameter("client_id", provider.Consumerkey);
            ub.SetQueryparameter("client_secret", provider.Consumersecret);
            ub.SetQueryparameter("code", connectionToken.Code);
            ub.SetQueryparameter("redirect_uri", connectionToken.ProviderCallbackUrl);
            ub.SetQueryparameter("grant_type", "authorization_code");
            //logger.LogAuthorizationRequest(ub.ToString());
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ub.ToString());

            string authToken = "";
            try
            {
                logger.LogOauthRequest("Requesting Access Token at " + ub.ToString());
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                    authToken = reader.ReadToEnd();
                return authToken;
            }
            catch (Exception ex)
            {
                throw new OAuthException("There was an error while retrieving Access Token from " + ub.ToString() + "!", ex);
            }
        }

        public void HandleAccessTokenResponse(string response)
        {
            if (response.StartsWith("{")) // access token is returned in JSON format
            {

                //  {"access_token":"asasdasdAA","expires_in":3600,"scope":"wl.basic","token_type":"bearer"}
                JObject accessTokenJson = JObject.Parse(response);
                connectionToken.AccessToken = accessTokenJson.SelectToken("access_token").ToString().Replace("\"", "");
                connectionToken.ExpiresOn = DateTime.Now.AddSeconds(int.Parse(accessTokenJson.SelectToken("expires_in").ToString().Replace("\"", "")) - 20);
                //put in raw list
                foreach (var t in accessTokenJson.AfterSelf())
                    connectionToken.ResponseCollection.Add(t.Type.ToString(), t.ToString());
                logger.LogOauthSuccess("Access Token Recieved");
                isSuccess = true;
            }
            else // access token is returned as part of Query String
            {

                QueryParameters responseCollection = Utility.GetQuerystringParameters(response);
                string keyForExpiry = responseCollection.Single(x => x.Name.Contains("expir")).Name;
                string keyForAccessToken = responseCollection.Single(x => x.Name.Contains("token")).Name;

                connectionToken.AccessToken = responseCollection[keyForAccessToken].Replace("\"", "");
                connectionToken.ExpiresOn = connectionToken.ExpiresOn = DateTime.Now.AddSeconds(int.Parse(responseCollection[keyForExpiry].Replace("\"", "")) - 20);
                //put in raw list
                responseCollection.ToList().ForEach(x => connectionToken.ResponseCollection.Add(x.Name, x.Value));
                logger.LogOauthSuccess("Access Token Recieved");
                isSuccess = true;
                //logger.LogAuthorizationResponse(response, null);
            }
        }

        #endregion

        public override WebResponse ExecuteFeed(string feedURL, IProvider provider, Token connectionToken, TRANSPORT_METHOD transportMethod)
        {
            UriBuilder ub;

            /******** retrieve standard Fields ************/
            ub = new UriBuilder(feedURL);
            //if (oauthParameters != null)
            //    foreach (var param in oauthParameters)
            //        ub.SetQueryparameter(param.Name, param.Value);

            ub.SetQueryparameter("access_token", connectionToken.AccessToken);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(ub.ToString());
            request.Method = transportMethod.ToString();
            //logger.LogContactsRequest(ub.ToString());
            WebResponse wr;
            try
            {
                logger.LogOauthRequest("Requesting Feed at " + feedURL + " using " + transportMethod.ToString());
                wr = (WebResponse)request.GetResponse();
                logger.LogOauthSuccess("Feed successfully executed");
            }
            catch (Exception ex)
            {
                logger.LogOauthRequestFailure(ex, new QueryParameters() { new QueryParameter("access_token", connectionToken.AccessToken) });
                throw new OAuthException("There was an error while executing " + feedURL +"!", ex);
            }

            return wr;
        }


        #region IOAuth2_0 Members

        public event Action<QueryParameters> BeforeDirectingUserToServiceProvider = delegate { };
        public event Action<QueryParameters> BeforeRequestingAccessToken = delegate { };

        #endregion
    }
}



/**************FLOW****************
     +----------+
     | resource |
     |   owner  |
     |          |
     +----------+
          ^
          |
         (B)
     +----|-----+          Client Identifier      +---------------+
     |         -+----(A)--- & Redirect URI ------>|               |
     |  User-   |                                 | Authorization |
     |  Agent  -+----(B)-- User authenticates --->|     Server    |
     |          |                                 |               |
     |         -+----(C)-- Authorization Code ---<|               |
     +-|----|---+                                 +---------------+
       |    |                                         ^      v
      (A)  (C)                                        |      |
       |    |                                         |      |
       ^    v                                         |      |
     +---------+                                      |      |
     |         |>---(D)-- Client Credentials, --------’      |
     |         |          Authorization Code,                |
     | Client  |            & Redirect URI                   |
     |         |                                             |
     |         |<---(E)----- Access Token -------------------’
     +---------+       (w/ Optional Refresh Token)
*********************************/