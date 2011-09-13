using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brickred.SocialAuth.NET.Core.BusinessObjects;
using System.IO;
using System.Collections.Specialized;
using System.Net;

namespace Brickred.SocialAuth.NET.Core
{
    public abstract class OAuthStrategyBase
    {
        protected IProvider provider;
        protected SocialAuthUser user;
        protected Token connectionToken = SocialAuthUser.GetCurrentConnectionToken();
        protected bool isSuccess = false;
        public abstract void Login();
        public abstract void LoginCallback(QueryParameters responseCollection, Action<bool> AuthenticationCompletionHandler);
        public abstract WebResponse ExecuteFeed(string feedURL, IProvider provider, Token connectionToken, TRANSPORT_METHOD transportMethod);
        

      
    }
}

