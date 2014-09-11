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
using log4net;


namespace Brickred.SocialAuth.NET.Core.Wrappers
{
    public class AmazonWrapper : Provider, IProvider
    {
        #region IProvider Members

        private OAuthStrategyBase _AuthenticationStrategy = null;

        //****** PROPERTIES
        private static readonly ILog logger = log4net.LogManager.GetLogger("AmazonWrapper");
        public override PROVIDER_TYPE ProviderType { get { return PROVIDER_TYPE.AMAZON; } }
        public override string UserLoginEndpoint { get { return "https://www.amazon.com/ap/oa"; } set { } }
        public override string AccessTokenEndpoint { get { return "https://api.amazon.com/auth/o2/token"; } }
        public override OAuthStrategyBase AuthenticationStrategy
        {
            get
            {
                if (_AuthenticationStrategy == null)
                {
                    _AuthenticationStrategy = new OAuth2_0server(this);
                    ((OAuth2_0server)_AuthenticationStrategy).AccessTokenRequestType = TRANSPORT_METHOD.POST;
                }
                return _AuthenticationStrategy;
            }
        }
        public override string ProfileEndpoint { get { return "https://api.amazon.com/user/profile"; } }

        //public override string ContactsEndpoint { get { return ""; } }
        //public override string ProfilePictureEndpoint { get { return ""; } }

        public override string ContactsEndpoint { get { throw new NotImplementedException(); } }
        public override string ProfilePictureEndpoint { get { throw new NotImplementedException(); } }

        public override SIGNATURE_TYPE SignatureMethod { get { throw new NotImplementedException(); } }
        public override TRANSPORT_METHOD TransportName { get { return TRANSPORT_METHOD.POST; } }

        public override string DefaultScope { get { return "profile"; } }


        //****** OPERATIONS
        public override UserProfile GetProfile()
        {
            Token token = ConnectionToken;
            OAuthStrategyBase strategy = AuthenticationStrategy;
            string response = "";


            //If token already has profile for this provider, we can return it to avoid a call
            if (token.Profile.IsSet)
            {
                logger.Debug("Profile successfully returned from session");
                return token.Profile;
            }

            try
            {
                logger.Debug("Executing Profile feed");
                Stream responseStream = strategy.ExecuteFeed(ProfileEndpoint, this, token, TRANSPORT_METHOD.GET).GetResponseStream();
                response = new StreamReader(responseStream).ReadToEnd();
            }
            catch
            {
                throw;
            }

            try
            {

                JObject jsonObject = JObject.Parse(response);
                token.Profile.ID = jsonObject.Get("user_id");
                token.Profile.FirstName = jsonObject.Get("name");
                token.Profile.DisplayName = token.Profile.FullName;
                token.Profile.Email = HttpUtility.UrlDecode(jsonObject.Get("email"));
                token.Profile.IsSet = true;
                logger.Info("Profile successfully received");
                //Session token updated with profile
            }
            catch (Exception ex)
            {
                logger.Error(ErrorMessages.ProfileParsingError(response), ex);
                throw new DataParsingException(ErrorMessages.ProfileParsingError(response), ex);
            }

            return token.Profile;
        }

        public override List<Contact> GetContacts()
        {
            throw new NotImplementedException();
        }
        public override WebResponse ExecuteFeed(string feedUrl, TRANSPORT_METHOD transportMethod)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
