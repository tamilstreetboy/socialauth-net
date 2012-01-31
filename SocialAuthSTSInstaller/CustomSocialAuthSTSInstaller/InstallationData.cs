using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomSocialAuthSTSInstaller
{
    internal static class InstallationData
    {
        static InstallationData()
        {
            Providers = new List<Provider>();
        }

        internal static int CurrentIndex { get; set; }

        internal static List<Provider> Providers { get; set; }
        internal static bool InstallationCanceled { get; set; }
        internal static bool BrowseBackMode { get; set; }
        internal static bool DefaultProviders { get; set; }


        internal static void PopulateDefaultProviders()
        {
            //Default provider for twitter
            Providers.Add(new Provider()
            {
                WrapperName = Provider.Wrapper.TwitterWrapper,
                ConsumerKey = "E3hm7J9IQbWLijpiQG7W8Q",
                ConsumerSecret = "SGKNuXyybt0iDdgsuzVbFHOaemV7V6pr0wKwbaT2MH0"
            });

            //Default provider for facebook
            Providers.Add(new Provider()
            {
                WrapperName = Provider.Wrapper.FacebookWrapper,
                ConsumerKey = "152190004803645",
                ConsumerSecret = "64c94bd02180b0ade85889b44b2ba7c4"

            });
            //Default provider for google
            Providers.Add(new Provider()
            {
                WrapperName = Provider.Wrapper.GoogleWrapper,
                ConsumerKey = "opensource.brickred.com",
                ConsumerSecret = "YC06FqhmCLWvtBg/O4W/aJfj"
            });
            //Default provider for yahoo
            Providers.Add(new Provider()
            {
                WrapperName = Provider.Wrapper.YahooWrapper,
                ConsumerKey = "dj0yJmk9VTdaSUVTU3RrWlRzJmQ9WVdrOWNtSjZNMFpITm1VbWNHbzlNQS0tJnM9Y29uc3VtZXJzZWNyZXQmeD1iMA--",
                ConsumerSecret = "1db3d0b897dac60e151aa9e2499fcb2a6b474546"
            });
            //Default provider for MSN
            Providers.Add(new Provider()
            {
                WrapperName = Provider.Wrapper.MSNWrapper,
                ConsumerKey = "000000004403D60E",
                ConsumerSecret = "cYqlii67pTvgPD4pdB7NUVC7L4MIHCcs"
            });
            //Default provider for Linkedin
            Providers.Add(new Provider()
            {
                WrapperName = Provider.Wrapper.LinkedInWrapper,
                ConsumerKey = "9-mmqg28fpMocVuAg87exH-RXKs70yms52GSFIqkZN25S3m96kdPGBbuSxdSBIyL",
                ConsumerSecret = "e6NBqhDYE1fX17RwYGW5vMp25Cvh7Sbw9t-zMYTIW_T5LytY5OwJ12snh_YftgE4"
            });
            //Default provider for MySpace
            Providers.Add(new Provider()
            {
                WrapperName = Provider.Wrapper.MySpaceWrapper,
                ConsumerKey = "29db395f5ee8426bb90b1db65c91c956",
                ConsumerSecret = "0fdccc829c474e42867e16b68cda37a4c4b7b08eda574fe6a959943e3e9be709"
            });
        }
    }
}
