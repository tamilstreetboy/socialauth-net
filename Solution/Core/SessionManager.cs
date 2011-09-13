using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brickred.SocialAuth.NET.Core.BusinessObjects;
using System.Web;


namespace Brickred.SocialAuth.NET.Core
{


    class SocialAuthSession
    {
        internal Action callback;
        internal Guid userGUID;
        internal List<Token> connectedTokens;
        internal Token inProgressToken;
        internal Action callbackAction
        {
            get
            {
                if (callback == null)
                    return new Action(() => { });
                else
                    return callback;
            }
            set
            {
                callback = value;
            }
        }
    }



    public class SessionManager
    {



        static SocialAuthSession userSession
        {
            get
            {

                if (HttpContext.Current.Session["socialauthsession"] == null)
                    HttpContext.Current.Session["socialauthsession"] = new SocialAuthSession()
                                {
                                    userGUID = Guid.NewGuid(),
                                    connectedTokens = new List<Token>()
                                };
                return (SocialAuthSession)HttpContext.Current.Session["socialauthsession"];
            }
        }


        internal static void SetCallback(Action callback)
        {
            userSession.callback = callback;
        }

        internal static void ExecuteCallback()
        {
            userSession.callbackAction.Invoke();
        }

        internal static void AddConnectionToken(Token token)
        {
            Token t = userSession.connectedTokens.Find(x => x.Provider == token.Provider);
            if (t != null)
                userSession.connectedTokens.Remove(t);
            userSession.connectedTokens.Add(token);
        }

        internal static void RemoveConnectionToken(PROVIDER_TYPE providerType)
        {
            userSession.connectedTokens.RemoveAll(x => x.Provider == providerType);

        }

        internal static void RemoveAllConnections()
        {
            userSession.connectedTokens.RemoveAll(x => true);
        }

        internal static List<PROVIDER_TYPE> GetConnectedProviders()
        {
            return userSession.connectedTokens.Select((x) => { return x.Provider; }).ToList();
        }

        internal static int ConnectionsCount
        {
            get
            {
                return userSession.connectedTokens.Count();
            }
        }

        internal static bool IsConnected
        {
            get
            {
                return userSession.connectedTokens.Count(x => !string.IsNullOrEmpty(x.AccessToken)) > 0;
            }
        }

        internal static bool IsConnectedWith(PROVIDER_TYPE providerType)
        {
            return (userSession.connectedTokens.Exists(x => x.Provider == providerType && !string.IsNullOrEmpty(x.AccessToken)));

        }

        internal static void AbandonSession()
        {
            HttpContext.Current.Session.Abandon();
        }

        internal static IProvider GetCurrentConnection()
        {
            if (ConnectionsCount > 0)
            {
                var lastConnection = userSession.connectedTokens.Last();
                return ProviderFactory.GetProvider(lastConnection.Provider);
            }
            else
            {
                return null;
            }
        }

        internal static IProvider GetConnection(PROVIDER_TYPE providerType)
        {
            IProvider provider = null;
            //There are no connections
            var lastConnection = userSession.connectedTokens.Find(x => x.Provider == providerType);
            if (lastConnection != null)
            {
                provider = ProviderFactory.GetProvider(lastConnection.Provider);
                return provider;
            }
            else
                return null;

        }

        internal static Token GetConnectionToken(PROVIDER_TYPE providerType)
        {
            var connectionToken = userSession.connectedTokens.Find(x => x.Provider == providerType);
            return connectionToken;
        }

        internal static Guid GetUserSessionGUID()
        {
            return userSession.userGUID;
        }


    }
}
