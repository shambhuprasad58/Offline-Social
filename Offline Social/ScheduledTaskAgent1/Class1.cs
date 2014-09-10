using Hammock.Authentication.OAuth;
using Hammock.Web;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTaskAgent1
{
    public class AppSettings
    {
        // twitter
        public static string RequestTokenUri = "https://api.twitter.com/oauth/request_token";
        public static string AuthorizeUri = "https://api.twitter.com/oauth/authorize";
        public static string AccessTokenUri = "https://api.twitter.com/oauth/access_token";
        public static string CallbackUri = "http://cse.iitkgp.ac.in/~shambhup/";   // we've mentioned Google.com as our callback URL.

        //#error TODO REGISTER YOUR APP WITH TWITTER TO GET YOUR KEYS AND FILL THEM IN HERE
        public static string consumerKey = "AwvCb0sHBtih9ohZiIZ2tA";
        public static string consumerKeySecret = "f4sChtn4Wkkx5sxbs8izdP6z628czh8qlZyUk6xyN6s";

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

    }
    
}
