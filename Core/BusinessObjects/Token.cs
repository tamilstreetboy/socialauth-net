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
using System.Web;

namespace Brickred.SocialAuth.NET.Core.BusinessObjects
{
    internal class Token
    {
        public PROVIDER_TYPE provider { get; set; }
        public string RequestToken { get; set; }
        public string AuthorizationToken { get; set; }
        public string CallbackURL { get; set; }
        public string Expiry { get; set; }
        public string AssocHandle { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string SessionGUID { get; set; }

        internal static Token GetCurrentToken(bool forceNew = false)
        {
            if (HttpContext.Current.Session["token"] == null || forceNew)
            {
                Token t = new Token();
                HttpContext.Current.Session.Add("token", t);
            }

            return (Token)HttpContext.Current.Session["token"];
        }

        internal static void DestroyCurrentToken()
        {
            HttpContext.Current.Session.Remove("token");

        }

    }


}
