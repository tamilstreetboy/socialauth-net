using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomSocialAuthSTSInstaller
{
    class Provider
    {
        internal Wrapper WrapperName { get; set; }
        internal string ConsumerKey { get; set; }
        internal string ConsumerSecret { get; set; }
        internal bool ScopeTypeCustom { get; set; }
        internal string ScopeType { get; set; }

        internal enum Wrapper { TwitterWrapper, FacebookWrapper, GoogleWrapper, LinkedInWrapper, MSNWrapper, MySpaceWrapper, YahooWrapper };

        internal static Wrapper GetWrapperFromString(string wrapperName)
        {
            switch (wrapperName.ToLower())
            {
                case "twitter":
                    return Wrapper.TwitterWrapper;
                case "facebook":
                    return Wrapper.FacebookWrapper;
                case "google":
                    return Wrapper.GoogleWrapper;
                case "linkedin":
                    return Wrapper.LinkedInWrapper;
                case "msn":
                    return Wrapper.MSNWrapper;
                case "myspace":
                    return Wrapper.MySpaceWrapper;
                case "yahoo":
                    return Wrapper.YahooWrapper;
                default:
                    throw new Exception(string.Format("Wrapper not found with given name {0}", wrapperName));
            }
        }
    }
}
