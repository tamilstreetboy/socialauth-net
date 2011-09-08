using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Brickred.SocialAuth.NET.Core.BusinessObjects;

namespace Brickred.SocialAuth.NET.Core
{
    internal interface IOAuth1_0a
    {
        
        //Communication Methods (Private)
        event Action<QueryParameters> BeforeRequestingRequestToken; //HOOK
        void RequestForRequestToken(); //(A)
        void HandleRequestTokenGrant(QueryParameters response); //(B)
        event Action<QueryParameters> BeforeDirectingUserToServiceProvider; //HOOK
        void DirectUserToServiceProvider(); //(C)
        void HandleUserReturnCallback(QueryParameters response); //(D) 
        event Action<QueryParameters> BeforeRequestingAccessToken; //HOOK
        void RequestForAccessToken(); //(E)
        void HandleAccessTokenResponse(QueryParameters response); //(F)
        WebResponse ExecuteFeed(string feedURL, IProvider provider, Token connectionToken, TRANSPORT_METHOD transportMethod);

    }

}

/**********  OAUTH 1.0a Process Flow **********
(A)----> Request For Access Token
(B)<---- Request Token Grant
(C)----> Direct User To Service Provider
(D)<---- Direct User To Consumer
(E)----> Request For Access Token
(F)<---- Access Token Granted
************************************************/