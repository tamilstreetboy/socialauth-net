Thanks for downloading SocialAuth.Net 2.4!!


*********************
VERY IMPORTANT NOTE
*********************
1. If you've upgraded from a previous version and have been using Google Wrapper, you'd need to change the name of wrapper from GoogleWrapper to GoogleHybridWrapper in your config as we've upgraded Google Wrapper to use their OAuth 2.0 flow. However, previous OAuth 1.0 Hybrid is still there For more details refer to 

2. Also, we've upgraded LinkedIn to use OAuth2.0. If you do not make any changes, your application will automatically start using LinkedIn OAuth2.0. Unlike Google, you do not need to make any changes. If you still intend to use LinkedIn 1.0, Please change your wrapper in config to LinkedIn1Wrapper (from LinkedInWrapper).


***********************
Features of 2.4
***********************
1. Twitter updated to 1.1
2. Google OAuth 2.0 integration [Refer: https://code.google.com/p/socialauth-net/wiki/google_provider_invalid_token]
3. LinkedIn OAuth 2.0 integration (Refer point 2 in previous Important Note section)
4. Control over base URL improved [Refer: https://code.google.com/p/socialauth-net/wiki/WebConfig_Reference?ts=1374413176&updated=WebConfig_Reference#Controlling_URL]
5. Authentication Strategy updated to support POST versions (Facebook supports GET based OAuth strategy while some providers like Google and LinkedIn support POST based strategy)
6. An issue with unable to Tweet with space in between resolve

***********************
Issues Resolved
***********************

1. Many will find their Google issues resolved as they can now use their accounts registered for OAuth2.0 with SocialAuth.NET
2. Downloading token has HTML appended in demo application is resolved (issue 184)
3. Issue in adding Tweet with spaces on Twitter is resolved



Please feel free to add Suggestions/Bugs at:
https://code.google.com/p/socialauth-net/issues/list


Hint: Nest release is going to be a big one!! Get control over SocialAuth.NET like never before.
