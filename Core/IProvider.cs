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
using Brickred.SocialAuth.NET.Core.BusinessObjects;

namespace Brickred.SocialAuth.NET.Core
{
    /// <summary>
    /// Defines properties and methods interfacing a provider
    /// </summary>
    internal interface IProvider
    {
        //OpenID/OAth implementation Endpoints 
        string AssocHandleURL { get;  }
        string RequestTokenURL { get;  }
        string AuthorizationTokenURL { get;  }
        string AccessTokenURL { get;  }

        //Data access endpoints
        string ProfileEndpoint { get;  }
        string ContactsEndpoint { get;  }
        string ProfilePictureEndpoint { get; }

        //Configuration Properties
        string Consumerkey { get; set; }
        string Consumersecret { get; set; }
        SIGNATURE_TYPE SignatureMethod { get;  }
        TRANSPORT_METHOD TransportName { get;  }
      
        //Methods()
        void RequestUserAuthentication();
        void ProcessAuthenticationResponse();
        void AuthorizeUser();
        UserProfile GetProfile();
        List<Contact> GetContacts();
        void Logout();

        

    }
}
