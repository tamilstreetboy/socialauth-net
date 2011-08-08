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
using System.Runtime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.Web;
using System.Net;
using System.Collections.Specialized;
using System.Web.Configuration;
using Brickred.SocialAuth.NET.Core.BusinessObjects;

namespace Brickred.SocialAuth.NET.Core
{
    public class Utility
    {

        public static T DeepClone<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                ms.Position = 0;
                bf.Serialize(ms, obj);
                ms.Position = 0;
                return (T)bf.Deserialize(ms);
            }

        }

        public static List<Type> GetAllImplementors(Type T)
        {
            List<Type> types = new List<Type>();
            types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => T.IsAssignableFrom(x) && x.IsClass).ToList<Type>();
            return types;
        }

        public static T GetInstance<T>(string typeName)
        {
            List<Type> types = (Assembly.GetExecutingAssembly().GetTypes().Where(
                x => x.Name == typeName && typeof(T).IsAssignableFrom(x))).ToList();

            if (types.Count > 0)
                return (T)Activator.CreateInstance(types[0]);

            return default(T);
        }

        public static string UrlEncodeForSigningRequest(string value)
        {
            string unreservedChars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            var result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                    result.Append(symbol);
                else
                    result.Append(HttpUtility.UrlEncode(symbol.ToString()).ToUpper());

                //result.Append('%' + String.Format("{0:X2}", (int)symbol));
            }

            return result.ToString();
        }

        public static string HttpTransferEncode(string value)
        {
            string result = "";

            Dictionary<string, string> ReplacementChart = new Dictionary<string, string>();
            ReplacementChart.Add("?", "%3F");
            ReplacementChart.Add("~", "%7E");
            ReplacementChart.Add("#", "%23");
            ReplacementChart.Add(":", "%3A");
            ReplacementChart.Add(";", "%3B");
            ReplacementChart.Add("&", "%26");
            ReplacementChart.Add("+", "%2B");
            ReplacementChart.Add("* ", "%2A");
            ReplacementChart.Add("\"", "%22");
            ReplacementChart.Add(">", "%3C");
            ReplacementChart.Add("<", "%3E");
            ReplacementChart.Add("|", "%7C");
            ReplacementChart.Add("%", "%25");
            ReplacementChart.Add("/", "%2F");
            ReplacementChart.Add("=", "%3D");

            foreach (char c in value.ToArray())
            {
                if (ReplacementChart.ContainsKey(c.ToString()))
                    result += ReplacementChart[c.ToString()];
                else
                    result += c;

            }
            return result;

        }

        public static string DoubleEncode(string value)
        {
            string result = "";

            Dictionary<string, string> ReplacementChart = new Dictionary<string, string>();
            ReplacementChart.Add("?", "%253F");
            ReplacementChart.Add("~", "%257E");
            ReplacementChart.Add("#", "%2523");
            ReplacementChart.Add(":", "%253A");
            ReplacementChart.Add(";", "%253B");
            ReplacementChart.Add("&", "%2526");
            ReplacementChart.Add("+", "%252B");
            ReplacementChart.Add("* ", "%252A");
            ReplacementChart.Add("\"", "%2522");
            ReplacementChart.Add(">", "%253C");
            ReplacementChart.Add("<", "%253E");
            ReplacementChart.Add("|", "%257C");
            ReplacementChart.Add("%", "%2525");
            ReplacementChart.Add("/", "%252F");
            ReplacementChart.Add("=", "%253D");

            foreach (char c in value.ToArray())
            {
                if (ReplacementChart.ContainsKey(c.ToString()))
                    result += ReplacementChart[c.ToString()];
                else
                    result += c;

            }
            return result;

        }

        private static string WebRequest(string http_method, string url, string postData, NameValueCollection headers)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = http_method.ToUpper();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = "Identify your application please.";
            webRequest.Timeout = 20000;
            if (headers != null) webRequest.Headers.Add(headers);
            if (http_method == "POST" || http_method == "DELETE")
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";
                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }
            responseData = WebResponseGet(webRequest);
            webRequest = null;
            return responseData;
        }

        private static string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }

        public static string getAssociationHandle(string url)
        {
            string assoc_handle = "";
            string data = WebRequest("GET", url, string.Empty, null);
            if (data.Length > 0)
            {
                string[] lines = data.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Substring(0, 13) == "assoc_handle:")
                    {
                        assoc_handle = line.Substring(13);
                        break;
                    }
                }
            }
            return assoc_handle;
        }

        public static string http_build_query(NameValueCollection data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in data.Keys)
            {
                sb.Append("&").Append(UrlEncode(key)).Append("=").Append(UrlEncode(data[key]));
            }
            return sb.ToString().Substring(1);
        }

        public static string UrlEncode(string value)
        {
            const string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        public static System.Web.Configuration.AuthenticationMode GetAuthenticationMode()
        {
            return ((AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication")).Mode;
        }

        public static SocialAuthConfiguration GetConfiguration()
        {
            SocialAuthConfiguration config = System.Configuration.ConfigurationManager.GetSection("SocialAuthConfiguration") as SocialAuthConfiguration;
            return config;
        }

        internal static OPERATION_MODE OperationMode()
        {
            OPERATION_MODE mode = OPERATION_MODE.NOT_SUPPORTED;

            if (GetAuthenticationMode() == AuthenticationMode.Forms)
                mode = OPERATION_MODE.FORMS_SECURITY_CUSTOM_SCREEN;
            else if (GetAuthenticationMode() == AuthenticationMode.None && GetConfiguration().Authentication.Enabled)
            {
                if (String.IsNullOrEmpty(GetConfiguration().Authentication.LoginUrl))
                    mode = OPERATION_MODE.SOCIALAUTH_SECURITY_SOCIALAUTH_SCREEN;
                else
                    mode = OPERATION_MODE.SOCIALAUTH_SECURITY_CUSTOM_SCREEN;
            }
            else if (GetAuthenticationMode() == AuthenticationMode.None && !GetConfiguration().Authentication.Enabled)
                mode = OPERATION_MODE.CUSTOM_SECURITY_CUSTOM_SCREEN;

            return mode;
        }

        internal static NameValueCollection GetQuerystringParameters(string querystring)
        {
            NameValueCollection parts = new NameValueCollection();
            if (querystring.Contains("?"))
                querystring = querystring.Substring(querystring.IndexOf("?") + 1);

            var queryParts = querystring.Split(new char[] { '&' });
            foreach (var queryPart in queryParts)
            {
                string[] keyValue = queryPart.ToString().Split(new char[] { '=' });
                parts.Add(keyValue[0], keyValue[1]);
            }
            return parts;


        }
    }
}

