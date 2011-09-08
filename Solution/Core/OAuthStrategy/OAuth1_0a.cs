using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Brickred.SocialAuth.NET.Core.BusinessObjects;

namespace Brickred.SocialAuth.NET.Core
{

    public class OAuth1_0a : OAuthStrategyBase, IOAuth1_0a
    {

        public OAuth1_0a(IProvider provider)
        {
            this.provider = provider;
            this.logger = LoggerFactory.GetLogger(ProviderFactory.GetProvider(provider.ProviderType).GetType());
        }

        public override void Login()
        {
            RequestForRequestToken(); //(A)
            //HandleRequestTokenGrant(response); //(B) Called From within above
            DirectUserToServiceProvider(); //(C)
        }
        public override void LoginCallback(QueryParameters responseCollection, Action<bool> AuthenticationCompletionHandler)
        {
            HandleUserReturnCallback(responseCollection); //(D) 
            RequestForAccessToken(); //(E)
            //HandleAccessTokenResponse(); //(F) Called from within above

            //Authentication Process is through. Inform Consumer. [Set isSuccess on successful authentication]
            provider.AuthenticationCompleting(isSuccess); // Let Provider Know authentication process is through
            AuthenticationCompletionHandler(isSuccess); // Authentication process complete. Call final method

        }



        #region IOAuth1_0a Members

        public event Action<QueryParameters> BeforeRequestingRequestToken = delegate { };
        public event Action<QueryParameters> BeforeRequestingAccessToken = delegate { };

        public void RequestForRequestToken()
        {
            QueryParameters oauthParameters = new QueryParameters();
            string signature = "";
            OAuthHelper oauthHelper = new OAuthHelper();


            //Twitter Test @ https://dev.twitter.com/docs/auth/oauth
            //oauthParameters.Add("oauth_callback", "http://localhost:3005/the_dance/process_callback?service_provider_id=11");
            //oauthParameters.Add("oauth_consumer_key", "GDdmIQH6jhtmLUypg82g");
            //oauthParameters.Add("oauth_nonce", "QP70eNmVz8jvdPevU3oJD2AfF7R7odC2XJcn4XlZJqk");
            //oauthParameters.Add("oauth_signature_method", "HMAC-SHA1");
            //oauthParameters.Add("oauth_timestamp", "1272323042");
            //oauthParameters.Add("oauth_version", "1.0");
            //signature = oauthHelper.GenerateSignature(new Uri(provider.RequestTokenEndpoint), oauthParameters, "GDdmIQH6jhtmLUypg82g", "MCD8BKwGdgPHvAuvgvz4EQpqDAtx89grbuNMRd7Eh98", provider.SignatureMethod, provider.TransportName, string.Empty);

            ////1. Setup request parameters
            oauthParameters.Add("oauth_consumer_key", provider.Consumerkey);
            oauthParameters.Add("oauth_signature_method", provider.SignatureMethod.ToString());
            oauthParameters.Add("oauth_timestamp", oauthHelper.GenerateTimeStamp());
            oauthParameters.Add("oauth_nonce", oauthHelper.GenerateNonce());
            oauthParameters.Add("oauth_version", "1.0");
            oauthParameters.Add("oauth_callback", connectionToken.Domain + "SocialAuth/validate.sauth");

            //2. Notify Consumer (optionally user may wish to add extra parameters)
            BeforeRequestingRequestToken(oauthParameters); // hook called

            //3. Generate Signature
            signature = oauthHelper.GenerateSignature(new Uri(provider.RequestTokenEndpoint), oauthParameters, provider.Consumerkey, provider.Consumersecret, provider.SignatureMethod, provider.TransportName, string.Empty);
            oauthParameters.Add("oauth_signature", signature);


            //4.Connect and obtain Token
            logger.LogOauthRequest("RequestToken request at " + provider.RequestTokenEndpoint);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(provider.RequestTokenEndpoint);
            request.Method = provider.TransportName.ToString();
            request.Headers.Add("Authorization", oauthHelper.GetAuthorizationHeader(oauthParameters));
            request.ContentLength = 0;
            //request.ContentType = "application/x-www-form-urlencoded";

            string response = "";

            try
            {


                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    response = reader.ReadToEnd();
                    if (response.Contains("oauth_token_secret"))
                    {
                        logger.LogOauthRequest("Request Token Recevied");
                        var responseCollection = Utility.GetQuerystringParameters(response);
                        HandleRequestTokenGrant(responseCollection);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogOauthRequestFailure(ex, oauthParameters);
                throw new OAuthException("There was an error while retrieving request token from " + provider.RequestTokenEndpoint, ex);

            }
        }

        public void HandleRequestTokenGrant(QueryParameters responseCollection)
        {
            connectionToken.RequestToken = responseCollection["oauth_token"];
            connectionToken.TokenSecret = responseCollection["oauth_token_secret"];
            connectionToken.ResponseCollection.AddRange(responseCollection, false);
        }

        public event Action<QueryParameters> BeforeDirectingUserToServiceProvider = delegate { };

        public void DirectUserToServiceProvider()
        {
            QueryParameters oauthParameters = new QueryParameters();
            oauthParameters.Add(new QueryParameter("oauth_token", connectionToken.RequestToken));
            BeforeDirectingUserToServiceProvider(oauthParameters);
            SocialAuthUser.Redirect(provider.UserLoginEndpoint + "?" + oauthParameters.ToString());
        }

        public void HandleUserReturnCallback(QueryParameters response)
        {
            if (response.HasName("oauth_verifier"))
            {
                connectionToken.OauthVerifier = response["oauth_verifier"];
                connectionToken.AuthorizationToken = response["oauth_token"];
            }
        }



        public void RequestForAccessToken()
        {
            QueryParameters oauthParameters = new QueryParameters();
            string signature = "";
            OAuthHelper oauthHelper = new OAuthHelper();

            ////1. Generate Signature
            oauthParameters.Add("oauth_consumer_key", provider.Consumerkey);
            oauthParameters.Add("oauth_token", connectionToken.RequestToken);
            oauthParameters.Add("oauth_signature_method", provider.SignatureMethod.ToString());
            oauthParameters.Add("oauth_timestamps", oauthHelper.GenerateTimeStamp());
            oauthParameters.Add("oauth_nonce", oauthHelper.GenerateNonce());
            oauthParameters.Add("oauth_version", "1.0");
            oauthParameters.Add("oauth_verifier", connectionToken.OauthVerifier);
            signature = oauthHelper.GenerateSignature(new Uri(provider.AccessTokenEndpoint), oauthParameters, provider.Consumerkey, provider.Consumersecret, provider.SignatureMethod, provider.TransportName, connectionToken.TokenSecret);
            oauthParameters.Add("oauth_signature", signature);

            //2. Notify Consumer (if applicable)
            BeforeRequestingAccessToken(oauthParameters); // hook called

            //3.Connect and obtain Token
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(provider.AccessTokenEndpoint);
            request.Method = provider.TransportName.ToString();
            request.Headers.Add("Authorization", oauthHelper.GetAuthorizationHeader(oauthParameters));
            request.ContentLength = 0;
            string response = "";

            try
            {
                logger.LogOauthRequest("Requesting Access Token at " + provider.AccessTokenEndpoint);
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = webResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    response = reader.ReadToEnd();
                    if (response.Contains("oauth_token_secret"))
                    {
                        var responseCollection = Utility.GetQuerystringParameters(response);
                        HandleAccessTokenResponse(responseCollection);
                        isSuccess = true;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogOauthRequestFailure(ex, oauthParameters);
                throw new OAuthException("There was an error while retrieving access token from " + provider.AccessTokenEndpoint, ex);
            }


        }

        public void HandleAccessTokenResponse(QueryParameters responseCollection)
        {
            connectionToken.AccessToken = responseCollection["oauth_token"];
            connectionToken.TokenSecret = responseCollection["oauth_token_secret"];
            connectionToken.ResponseCollection.AddRange(responseCollection, true);
            logger.LogOauthRequest("Access Token Recevied");
        }

        public override WebResponse ExecuteFeed(string feedURL, IProvider provider, Token connectionToken, TRANSPORT_METHOD transportMethod)
        {
            string signature = "";
            OAuthHelper oauthHelper = new OAuthHelper();
            QueryParameters oauthParams = new QueryParameters();
            oauthParams.Add("oauth_consumer_key", provider.Consumerkey);
            oauthParams.Add("oauth_nonce", oauthHelper.GenerateNonce());
            oauthParams.Add("oauth_signature_method", provider.SignatureMethod.ToString());
            oauthParams.Add("oauth_timestamp", oauthHelper.GenerateTimeStamp());
            oauthParams.Add("oauth_token", connectionToken.AccessToken);
            oauthParams.Add("oauth_version", "1.0");


            ////1. Generate Signature
            signature = oauthHelper.GenerateSignature(new Uri(feedURL), oauthParams, provider.Consumerkey, provider.Consumersecret, provider.SignatureMethod, TRANSPORT_METHOD.GET, connectionToken.TokenSecret);
            oauthParams.Add("oauth_signature", signature);


            //3.Connect and Execute Feed

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(feedURL);
            request.Method = transportMethod.ToString();
            request.Headers.Add("Authorization", oauthHelper.GetAuthorizationHeader(oauthParams));
            //request.ContentType = "application/atom+xml";
            request.ContentLength = 0;
            WebResponse wr;
            try
            {
                logger.LogOauthRequest("Requesting Feed at " + feedURL + " using " + transportMethod.ToString());
                wr = (WebResponse)request.GetResponse();
                logger.LogOauthSuccess("Feed successfully executed");
            }
            catch (Exception ex)
            {
                logger.LogOauthRequestFailure(ex, oauthParams);
                throw new OAuthException("There was an error while executing " + feedURL, ex);
            }
            return wr;
        }

        #endregion
    }
}
