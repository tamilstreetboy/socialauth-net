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
using Brickred.SocialAuth.NET.Core.BusinessObjects;
using System.Diagnostics;
using System.Web;

namespace Brickred.SocialAuth.NET.Core
{
    //This interface needs to be implemented in Wrapper for any logger used
    public interface ILogger
    {
        void Log(LogEventType eventType, string errorDescription, System.Exception exception);
        void Log(LogEventType eventType, string errorDescription);

        //Shortcut methods for logging in SocialAuthcontext
        void LogAuthenticationRequest(string url);
        void LogAuthenticationResponse(string response, Exception ex);
        void LogAuthorizationRequest(string url);
        void LogAuthorizationResponse(string response, Exception ex);
        void LogProfileRequest(string url);
        void LogProfileResponse(Exception ex);
        void LogContactsRequest(string url);
        void LogContactsResponse(Exception ex);

    }

    //Type of Logging Events
    public enum LogEventType
    {
        Debug,
        Error,
        Fatal,
        Info,
        Warn
    }

    //Factory that returns the type of logger required. Currently hardcoded to always return Log4Net
    //but can be updated to load required Logger from config file
    public static class LoggerFactory
    {

        private static readonly string defaultLoggerType;

        static LoggerFactory()
        {
            //Read from config which Logger we want to use. As of now we are using only Log4Net,
            //Hence, an instace of log4net is returned directly.
            defaultLoggerType = "Log4Net";
        }

        public static ILogger GetLogger(Type sourceType)
        {
            switch (defaultLoggerType)
            {
                case "Log4Net":
                    return new Log4NetLoggerWrapper(sourceType);

                default:
                    throw new Exception("Invalid Logger type");
            }
        }
    }

    //Wrapper for Log4Net logging mechanism
    public class Log4NetLoggerWrapper : ILogger
    {
        //Calling method's class type
        Type sourceType;


        //Constructor
        internal Log4NetLoggerWrapper(Type sourceType)
        {
            this.sourceType = sourceType;
        }

        //Master Method which eventually calls Log4Net logging method
        public void Log(LogEventType eventType, string errorDescription, Exception exception)
        {

            if (!Utility.GetConfiguration().Logging.Enabled)
                return;


            log4net.ILog logger = log4net.LogManager.GetLogger(sourceType);
            if (SocialAuthUser.GetCurrentUser() != null)
                log4net.ThreadContext.Properties["SocialAuthUserSessionId"] = SocialAuthUser.GetCurrentUser().Identifier;// HttpContext.Current.Session.SessionID;

            switch (eventType)
            {
                case LogEventType.Debug:
                    if (exception != null)
                    {
                        logger.Debug(errorDescription, exception);
                    }
                    else
                    {
                        logger.Debug(errorDescription);
                    }
                    break;
                case LogEventType.Error:
                    if (exception != null)
                    {
                        logger.Error(errorDescription, exception);
                    }
                    else
                    {
                        logger.Error(errorDescription);
                    }
                    break;
                case LogEventType.Fatal:
                    if (exception != null)
                    {
                        logger.Fatal(errorDescription, exception);
                    }
                    else
                    {
                        logger.Fatal(errorDescription);
                    }
                    break;
                case LogEventType.Info:
                    if (exception != null)
                    {
                        logger.Info(errorDescription, exception);
                    }
                    else
                    {
                        logger.Info(errorDescription);
                    }
                    break;
                case LogEventType.Warn:
                    if (exception != null)
                    {
                        logger.Warn(errorDescription, exception);
                    }
                    else
                    {
                        logger.Warn(errorDescription);
                    }
                    break;
            }

        }


        //****Overloaded methods for Logging****//

        //Call this method to insert custom log message
        public void Log(LogEventType eventType, string errorDescription)
        {
            //Console.WriteLine(message);
            Log(eventType, errorDescription, null);
        }

        //Call this method before maing Token Request (before redirecting user to provider for login)
        public void LogAuthenticationRequest(string url)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(sourceType);
            if (logger.IsDebugEnabled)
                Log(LogEventType.Debug, "AuthenticateUser(), User redirected for login to: " + url);
            else
                Log(LogEventType.Info, "User redirected for login to: " + SocialAuthUser.GetCurrentUser().contextToken.provider.ToString());

        }

        //Call this method when response is recevied for Token request, Null exception infers success.
        public void LogAuthenticationResponse(string response, Exception ex)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(sourceType);
            if (ex == null)
            {
                if (logger.IsDebugEnabled)
                    Log(LogEventType.Debug, "AuthenticateUser(), response recieved: " + response);
            }
            else
            {
                if (logger.IsDebugEnabled)
                    Log(LogEventType.Error, "AuthenticateUser(), login request failed with error: " + ex.Message);
                else
                    Log(LogEventType.Info, "User login request failed");
            }
        }

        //Call this method when request is made for AccessToken
        public void LogAuthorizationRequest(string url)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(sourceType);
            if (logger.IsDebugEnabled)
                Log(LogEventType.Debug, "AuthorizeUser() call made to:" + url);
        }

        //Call this method when response is recevied for AccessToken request. Null exception infers success.
        public void LogAuthorizationResponse(string response, Exception ex)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(sourceType);
            if (ex == null)
            {
                if (logger.IsDebugEnabled)
                    Log(LogEventType.Debug, "AuthorizeUser(), success response recieved: " + response);
            }
            else
            {
                if (logger.IsDebugEnabled)
                    Log(LogEventType.Error, "AuthorizeUser() request failed with error: " + ex.Message);
                else
                    Log(LogEventType.Info, "Authorization request failed.");
            }
        }

        //Call thismethod when request is to be made for retriving user's profile
        public void LogProfileRequest(string url)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(sourceType);
            if (logger.IsDebugEnabled)
                Log(LogEventType.Debug, "GetProfile() call made to:" + url);
            else
                Log(LogEventType.Info, "Requesting user's Profile");
        }

        //Call this method when Response is recevied for Profile request. Null exception infers success.
        public void LogProfileResponse(Exception ex)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(sourceType);
            if (ex == null)

                Log(LogEventType.Info, "GetProfile(), Profile recevied successfully");
            else
            {
                if (logger.IsDebugEnabled)
                    Log(LogEventType.Error, "GetProfile(), Request failed with error: " + ex.Message);
                else
                    Log(LogEventType.Info, "Profile request failed");
            }
        }

        //Call this method when request is made for user's contacts
        public void LogContactsRequest(string url)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(sourceType);
            if (logger.IsDebugEnabled)
                Log(LogEventType.Debug, "GetContacts() call made to:" + url);
            else
                Log(LogEventType.Info, "Requesting user's Contacts");
        }

        //Call this method when response os recevied for contacts request. Null exception infers success.
        public void LogContactsResponse(Exception ex)
        {
            log4net.ILog logger = log4net.LogManager.GetLogger(sourceType);
            if (ex == null)

                Log(LogEventType.Info, "GetContacts(), Contacts recevied successfully");
            else
            {
                if (logger.IsDebugEnabled)
                    Log(LogEventType.Error, "GetContacts(), Request failed with error: " + ex.Message);
                else
                    Log(LogEventType.Info, "Contacts request failed");
            }
        }

    }

}
