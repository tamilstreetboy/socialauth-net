using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Brickred.SocialAuth.NET.Core.BusinessObjects;
using System.Net;




namespace Brickred.SocialAuth.NET.Core
{
    public interface IOAuth2_0
    {

        event Action<QueryParameters> BeforeDirectingUserToServiceProvider; //HOOK
        void DirectUserToServiceProvider(); // (A)(B)
        void HandleAuthorizationCode(QueryParameters responseCollection); // (C)
        event Action<QueryParameters> BeforeRequestingAccessToken; //HOOK
        string RequestForAccessToken(); // (D)
        void HandleAccessTokenResponse(string response); // (E)
        WebResponse ExecuteFeed(string feedURL, IProvider provider, Token connectionToken, TRANSPORT_METHOD transportMethod);
    }
}


/**************FLOW****************
     +----------+
     | resource |
     |   owner  |
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