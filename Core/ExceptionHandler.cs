using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Brickred.SocialAuth.NET.Core.BusinessObjects;

namespace Brickred.SocialAuth.NET.Core
{
    /// <summary>
    /// Exception raised when user tries to perform an operation with a provider, he is not connected with!
    /// </summary>
    public class InvalidSocialAuthConnectionException : Exception
    {
        PROVIDER_TYPE providertype;
        
        public InvalidSocialAuthConnectionException(PROVIDER_TYPE providertype = PROVIDER_TYPE.NOT_SPECIFIED)
        {
            this.providertype = providertype;
        }
        
        public override string Message
        {
            get
            {
                if (providertype == PROVIDER_TYPE.NOT_SPECIFIED)
                    return "Can not get results as User is not connected with any provider.";
                else
                    return "Can not get results from " + providertype.ToString() + " as User is not connected with it.";
            }
        }
    }

    /// <summary>
    /// Exception raised when there is some problem in executing OAuth command
    /// </summary>
    public class OAuthException : Exception
    {
        string message = "";
        Exception ex = null;

        public OAuthException(string message, Exception ex)
        {
            this.message = message;
            this.ex = ex;
        }

        public OAuthException(string message)
        {
            this.message = message;
        }

        public override string Message
        {

            get
            {
                string additionalDetails = "";
                if (ex != null)
                    if (ex.Message.Contains("400")) //BAD REQUEST
                    {
                        additionalDetails += Environment.NewLine + "Please ensure all required parameters are passed, Signature is Url Encoded and Authorization header is properly set!";
                    }
                    else if (ex.Message.Contains("401")) //UNAUTHORIZED
                    {
                        additionalDetails += Environment.NewLine + "Unauthorized! Please ensure:" + Environment.NewLine + "(1) All required parameters are passed";
                        additionalDetails += Environment.NewLine + "(2) Signature is Url Encoded";
                        additionalDetails += Environment.NewLine + "(3) Authorization header is properly set";
                    }
                    else if (ex.Message.Contains("403")) //FORBIDDEN
                    {
                        additionalDetails += Environment.NewLine + "Forbidden! " + Environment.NewLine + ex.Message;
                    }
                    else if (ex.Message.Contains("404")) //NOT FOUND
                    {
                        additionalDetails += Environment.NewLine + " Requested URL could not be found at provider";
                    }
                    else if (ex.Message.Contains("500")) //INTERNAL SERVER ERROR
                    {
                        additionalDetails += Environment.NewLine + " Something is broken at provider. Request broke with error message:" + ex.Message;
                    }
                    else if (ex.Message.Contains("502")) //SERVICE UNAVAILABLE
                    {
                        additionalDetails += Environment.NewLine + " Possibly provider is down. Request broke with errror: " + ex.Message;
                    }
                    else if (ex.Message.Contains("503")) //SERVICE UNAVAILABLE
                    {
                        additionalDetails += Environment.NewLine + " Request broke with error message: " + ex.Message;
                    }
                    else
                    {
                        additionalDetails = ex.Message;
                    }

                return message + additionalDetails;
            }
        }


    }

    public class DataParsingException : Exception
    {

    }
}