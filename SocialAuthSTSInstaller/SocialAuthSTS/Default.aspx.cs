//-----------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//
//-----------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Web.UI;

using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Web;
using Brickred.SocialAuth.NET.Core.BusinessObjects;

/// <summary>
/// The Default Page Class
/// </summary>
public partial class _Default : Page
{
    /// <summary>
    /// Performs WS-Federation Passive Protocol processing. 
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        string action = Request.QueryString[WSFederationConstants.Parameters.Action];

        try
        {
            if (action == WSFederationConstants.Actions.SignIn)
            {
                // Process signin request.
                SignInRequestMessage requestMessage = (SignInRequestMessage)WSFederationMessage.CreateFromUri(Request.Url);
                if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                {
                    SecurityTokenService sts = new CustomSecurityTokenService(CustomSecurityTokenServiceConfiguration.Current);
                    SignInResponseMessage responseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMessage, User, sts);
                    FederatedPassiveSecurityTokenServiceOperations.ProcessSignInResponse(responseMessage, Response);
                }
                else
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else if (action == WSFederationConstants.Actions.SignOut)
            {
                // Process signout request.
                SignOutRequestMessage requestMessage = (SignOutRequestMessage)WSFederationMessage.CreateFromUri(Request.Url);
                FederatedPassiveSecurityTokenServiceOperations.ProcessSignOutRequest(requestMessage, User, requestMessage.Reply, Response);
            }
            else if (action == null && SocialAuthUser.IsLoggedIn())
            {
                string originalUrl = SocialAuthUser.GetCurrentUser().GetConnection(SocialAuthUser.CurrentProvider).GetConnectionToken().UserReturnURL;

                //replace ru value
                int wctxBeginsFrom = originalUrl.IndexOf("wctx=");
                int wctxEndsAt = originalUrl.IndexOf("&wct=");
                string wctxContent = originalUrl.Substring(wctxBeginsFrom + 5, wctxEndsAt - (wctxBeginsFrom + 5));
                originalUrl = originalUrl.Replace(wctxContent, Server.UrlEncode(wctxContent));

                //replace wtrealm value
                int wtrealmBeginsFrom = originalUrl.IndexOf("wtrealm=");
                int wtrealmEndsAt = originalUrl.IndexOf("&", wtrealmBeginsFrom);
                string wtrealmContent = originalUrl.Substring(wtrealmBeginsFrom + 8, wtrealmEndsAt - (wtrealmBeginsFrom + 8));
                originalUrl = originalUrl.Replace(wtrealmContent, Server.UrlEncode(wtrealmContent));

                SignInRequestMessage requestMessage = (SignInRequestMessage)WSFederationMessage.CreateFromUri(new Uri(originalUrl));
                if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                {
                    SecurityTokenService sts = new CustomSecurityTokenService(CustomSecurityTokenServiceConfiguration.Current);
                    SignInResponseMessage responseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(requestMessage, User, sts);
                    FederatedPassiveSecurityTokenServiceOperations.ProcessSignInResponse(responseMessage, Response);
                }

            }
            else
            {
                throw new InvalidOperationException(
                    String.Format(CultureInfo.InvariantCulture,
                                   "The action '{0}' (Request.QueryString['{1}']) is unexpected. Expected actions are: '{2}' or '{3}'.",
                                   String.IsNullOrEmpty(action) ? "<EMPTY>" : action,
                                   WSFederationConstants.Parameters.Action,
                                   WSFederationConstants.Actions.SignIn,
                                   WSFederationConstants.Actions.SignOut));
            }
        }
        catch (Exception exception)
        {
            throw new Exception("An unexpected error occurred when processing the request. See inner exception for details.", exception);
        }
    }
}
