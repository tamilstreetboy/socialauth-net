using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Brickred.SocialAuth.NET.Core.BusinessObjects;

namespace Brickred.SocialAuth.NET.Core
{
    public interface IOAuth1_0Hybrid
    {

        //Communication Methods (Private)
        void PerformDiscovery();
        event Action<QueryParameters> BeforeDirectingUserToServiceProvider; //HOOK
        void DirectUserToServiceProvider(); //(C)
        void HandleUserReturnCallback(QueryParameters response); //(D) 
        event Action<QueryParameters> BeforeRequestingAccessToken; //HOOK
        void RequestForAccessToken(); //(E)
        void HandleAccessTokenResponse(QueryParameters response); //(F)
        WebResponse ExecuteFeed(string feedURL, IProvider provider, Token connectionToken, TRANSPORT_METHOD transportMethod);
    }
}
