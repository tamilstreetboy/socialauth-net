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
                    return "User is not connected with any provider";
                else
                    return "User is not connected with " + providertype.ToString();
            }
        }
    }

    [Serializable]
    public class NewException : Exception, ISerializable
    {
        Exception ex;
        public NewException(Exception ex)
        {
            this.ex = ex;
        }
        public NewException()
        {

            // Add implementation.
        }
        public NewException(string message)
        {
            // Add implementation.
        }
        public NewException(string message, Exception inner)
        {

        }

        public override string StackTrace
        {
            get
            {
                return "ffffffffffffffffffff";
            }
        }
        // This constructor is needed for serialization.
        protected NewException(SerializationInfo info, StreamingContext context)
        {
            // Add implementation.
        }

        public override Exception GetBaseException()
        {
            return ex.InnerException;
        }
        public override string Message
        {
            get
            {
                return "asdasdasd";
            }
        }

    }
}