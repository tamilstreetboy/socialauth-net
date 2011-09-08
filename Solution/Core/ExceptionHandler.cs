using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Brickred.SocialAuth.NET.Core.BusinessObjects;

namespace Brickred.SocialAuth.NET.Core
{

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


    public class OAuthException : Exception
    {
        string message = "";
        public OAuthException(string message, Exception ex)
        {
            this.message = message;
            
            if (ex.Message.Contains("400")) //BAD REQUEST
            {
                message += Environment.NewLine + "Bad Request! Please ensure:" + Environment.NewLine + "(1) All required parameters are passed";
                message += Environment.NewLine + "(2) Signature is Url Encoded";
                message += Environment.NewLine + "(3) Authorization header is properly set";
            }
            else if (ex.Message.Contains("401")) //UNAUTHORIZED
            {
                message += Environment.NewLine + "Unauthorized! Please ensure:" + Environment.NewLine + "(1) All required parameters are passed";
                message += Environment.NewLine + "(2) Signature is Url Encoded";
                message += Environment.NewLine + "(3) Authorization header is properly set";
            }
            else if (ex.Message.Contains("403")) //FORBIDDEN
            {
                message += Environment.NewLine + "Forbidden! " + Environment.NewLine  + ex.Message;
            }
            else if (ex.Message.Contains("404")) //NOT FOUND
            {
                message += Environment.NewLine + " Requested URL could not be found at provider";
            }
            else if (ex.Message.Contains("500")) //INTERNAL SERVER ERROR
            {
                message += Environment.NewLine + " Something is broken at provider. Request broke with error message:" + ex.Message;
            }
            else if (ex.Message.Contains("502")) //SERVICE UNAVAILABLE
            {
                message += Environment.NewLine + " Possibly provider is down. Request broke with errror: " + ex.Message;
            }
            else if (ex.Message.Contains("503")) //SERVICE UNAVAILABLE
            {
                message += Environment.NewLine + " Request broke with error message: " + ex.Message;
            }
            else
            {
                message = ex.Message;
            }

        }

        public override string Message
        {
            get
            {
                return message;
            }
        }
    }

}