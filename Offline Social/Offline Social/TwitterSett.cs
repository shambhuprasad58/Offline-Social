using Hammock.Authentication.OAuth;
using Hammock.Web;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offline_Social
{
    public class AppSettings
    {
        // twitter
        public static string RequestTokenUri = "https://api.twitter.com/oauth/request_token";
        public static string AuthorizeUri = "https://api.twitter.com/oauth/authorize";
        public static string AccessTokenUri = "https://api.twitter.com/oauth/access_token";
        public static string CallbackUri = "http://www.iitkgp.ac.in/";   // we've mentioned Google.com as our callback URL.

        //#error TODO REGISTER YOUR APP WITH TWITTER TO GET YOUR KEYS AND FILL THEM IN HERE
        //public static string consumerKey = "AwvCb0sHBtih9ohZiIZ2tA";
        //public static string consumerKeySecret = "f4sChtn4Wkkx5sxbs8izdP6z628czh8qlZyUk6xyN6s";
        public static string consumerKey = "NRKJAyuSeonKGtqdFQy4yQ";
        public static string consumerKeySecret = "wXufa6si8limUNoO5jFts8awpy5qiFOrlggxiuLVj0";
        

        public static string oAuthVersion = "1.0a";
    }
    public class MainUtil
    {
        public static Dictionary<string, string> GetQueryParameters(string response)
        {
            Dictionary<string, string> nameValueCollection = new Dictionary<string, string>();
            string[] items = response.Split('&');

            foreach (string item in items)
            {
                if (item.Contains("="))
                {
                    string[] nameValue = item.Split('=');
                    if (nameValue[0].Contains("?"))
                        nameValue[0] = nameValue[0].Replace("?", "");
                    nameValueCollection.Add(nameValue[0], System.Net.HttpUtility.UrlDecode(nameValue[1]));
                }
            }
            return nameValueCollection;
        }

        internal static T GetKeyValue<T>(string key)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                return (T)IsolatedStorageSettings.ApplicationSettings[key];
            else
                return default(T);
        }

        internal static void SetKeyValue<T>(string key, T value)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                IsolatedStorageSettings.ApplicationSettings[key] = value;
            else
                IsolatedStorageSettings.ApplicationSettings.Add(key, value);
            IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }
    public class OAuthUtil
    {
        internal static OAuthWebQuery GetRequestTokenQuery()
        {
            var oauth = new OAuthWorkflow
            {
                ConsumerKey = AppSettings.consumerKey,
                ConsumerSecret = AppSettings.consumerKeySecret,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                RequestTokenUrl = AppSettings.RequestTokenUri,
                Version = AppSettings.oAuthVersion,
                CallbackUrl = AppSettings.CallbackUri
            };

            var info = oauth.BuildRequestTokenInfo(WebMethod.Get);
            var objOAuthWebQuery = new OAuthWebQuery(info, false);
            objOAuthWebQuery.HasElevatedPermissions = true;
            objOAuthWebQuery.SilverlightUserAgentHeader = "Hammock";
            return objOAuthWebQuery;
        }

        internal static OAuthWebQuery GetAccessTokenQuery(string requestToken, string RequestTokenSecret, string oAuthVerificationPin)
        {
            var oauth = new OAuthWorkflow
            {
                AccessTokenUrl = AppSettings.AccessTokenUri,
                ConsumerKey = AppSettings.consumerKey,
                ConsumerSecret = AppSettings.consumerKeySecret,
                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                Token = requestToken,
                Verifier = oAuthVerificationPin,
                Version = AppSettings.oAuthVersion
            };

            var info = oauth.BuildAccessTokenInfo(WebMethod.Post);
            var objOAuthWebQuery = new OAuthWebQuery(info, false);
            objOAuthWebQuery.HasElevatedPermissions = true;
            objOAuthWebQuery.SilverlightUserAgentHeader = "Hammock";
            return objOAuthWebQuery;
        }
    }

}
