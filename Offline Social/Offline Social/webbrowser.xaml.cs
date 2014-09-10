using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using Hammock.Web;
using System.Text;

namespace Offline_Social
{
    public partial class webbrowser : PhoneApplicationPage
    {
        public webbrowser()
        {
            InitializeComponent();
            AuthenticationBrowser.ClearCookiesAsync();
            checkinput();
        }


        public string fbAccessToken { get; set; }

        public void checkinput()
        {
            if (App.loginselected == 0)
                NavigationService.GoBack();
            if (App.loginselected == 1)
                fbstartlogin();
            if (App.loginselected == 2)
                twstartlogin();
            if (App.loginselected == 3)
                ldstartlogin();

        }
        public string consumerKey = "dwche609gh27";                           //linkedin
        public string consumerSecret = "7GjE8nzq6TRPZ8cm";
        public string authcode = "";

        public Random rand = new Random();
        public string state = "";


        private void ldstartlogin()
        {
            state = rand.Next(100000000, 999999999).ToString() + rand.Next(100000000, 999999999).ToString();
            Uri ur = new Uri("https://www.linkedin.com/uas/oauth2/authorization?response_type=code&client_id=75ng9iwdzzhk2v&scope=r_basicprofile%20r_emailaddress%20r_network%20rw_nus%20w_messages&state=" + state + "&redirect_uri=http://www.iitkgp.ac.in/");
            AuthenticationBrowser.Navigate(ur);
            this.AuthenticationBrowser.Visibility = Visibility.Visible;
        }



        private void twstartlogin()
        {
            var requestTokenQuery = OAuthUtil.GetRequestTokenQuery();
            requestTokenQuery.RequestAsync(AppSettings.RequestTokenUri, null);
            requestTokenQuery.QueryResponse += new EventHandler<WebQueryResponseEventArgs>(requestTokenQuery_QueryResponse);

        }


        public void fbstartlogin()
        {
            var loginUrl = "http://www.facebook.com/dialog/oauth?client_id=628708203830784&redirect_uri=http%3A%2F%2Fwww.iitkgp.ac.in%2F&scope=publish_stream%2Cuser_status%2Cuser_photos%2Cphoto_upload%2cread_friendlists&response_type=token&display=touch";
            AuthenticationBrowser.Navigate(new Uri(loginUrl));
            AuthenticationBrowser.Visibility = Visibility.Visible;
        }


        private void BrowserNavigated(object sender, NavigationEventArgs e)
        {
            if (App.loginselected == 1)
            {
                if (string.IsNullOrEmpty(e.Uri.Fragment))
                    return;
                if (e.Uri.AbsoluteUri.Replace(e.Uri.Fragment, "") == "http://www.iitkgp.ac.in/?" || e.Uri.AbsoluteUri.Replace(e.Uri.Fragment, "") == "https://www.google.co.in/?")
                {
                    string text = HttpUtility.HtmlDecode(e.Uri.Fragment).TrimStart('#');
                    //char [] separators = {'&',';'};
                    var pairs = text.Split('&');
                    foreach (var pair in pairs)
                    {
                        var kvp = pair.Split('=');
                        if (kvp.Length == 2)
                        {
                            if (kvp[0] == "access_token")
                            {
                                fbAccessToken = kvp[1];
                                MessageBox.Show("Access granted");
                                App.fbaccesstokenkey = fbAccessToken;
                                App.writeBackToken();
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(fbAccessToken))
                    {
                        MessageBox.Show("Unable to authenticate facebook");
                    }

                }
                NavigationService.GoBack();
            }
            if (App.loginselected == 3)
            {
                if (e.Uri.ToString().StartsWith("http://www.iitkgp.ac.in/"))
                {
                    if (e.Uri.ToString().ToLower().Contains("error"))
                    {
                        MessageBox.Show("Unable to authenticate linkedin");
                    }
                    else
                    {
                        string ret = e.Uri.ToString();
                        string[] arr = ret.Split('?');
                        arr = arr[1].Split('&');
                        if (arr[0].Split('=')[0].Equals("code"))
                        {
                            if (arr[1].Split('=')[1].Equals(state))
                            {
                                authcode = arr[0].Split('=')[1];
                                getLdAccessToken();

                            }
                            else
                            {
                                MessageBox.Show("Unable to authenticate linkedin");
                                NavigationService.GoBack();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Unable to authenticate linkedin");
                            NavigationService.GoBack();
                        }
                    }
                }

            }
            if (App.loginselected == 2)
            {

                if (e.Uri.ToString().StartsWith(AppSettings.CallbackUri))
                {
                    if (e.Uri.ToString().ToLower().Contains("oauth_verifier"))
                    {
                        var AuthorizeResult = MainUtil.GetQueryParameters(e.Uri.ToString());
                        var VerifyPin = AuthorizeResult["oauth_verifier"];
                        this.AuthenticationBrowser.Visibility = Visibility.Collapsed;
                        var AccessTokenQuery = OAuthUtil.GetAccessTokenQuery(OAuthTokenKey, tokenSecret, VerifyPin);

                        AccessTokenQuery.QueryResponse += new EventHandler<WebQueryResponseEventArgs>(AccessTokenQuery_QueryResponse);
                        AccessTokenQuery.RequestAsync(AppSettings.AccessTokenUri, null);

                    }
                    else
                    {
                        MessageBox.Show("Unable to authenticate twitter");

                        NavigationService.GoBack();
                    }
                }

            }
        }

        // linkedin
        private void getLdAccessToken()
        {

            Uri myUri = new Uri("https://www.linkedin.com/uas/oauth2/accessToken");
            HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(myUri);
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), myRequest);
        }

        void GetRequestStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
            // End the stream request operation
            Stream postStream = myRequest.EndGetRequestStream(callbackResult);

            // Create the post data
            string postData = "grant_type=authorization_code&code=" + authcode + "&redirect_uri=http://www.iitkgp.ac.in/&client_id=75ng9iwdzzhk2v&client_secret=ExHPc8ZZNzuEhRZz";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Add the post data to the web request
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();

            // Start the web request
            myRequest.BeginGetResponse(new AsyncCallback(GetResponsetStreamCallback), myRequest);
        }

        public string ldaccessToken = "";
        void GetResponsetStreamCallback(IAsyncResult callbackResult)
        {
            HttpWebRequest request = (HttpWebRequest)callbackResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(callbackResult);
            using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream()))
            {
                string result = httpWebStreamReader.ReadToEnd();
                //For debug: show results
                result = result.Replace("\"", "");
                result = result.Replace("{", "");
                result = result.Replace("}", "");
                ldaccessToken = result.Split(':')[2];
            }
            Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show("Linkedin Access granted");
                App.ldaccesstokenkey = ldaccessToken;
                App.writeBackToken();
                NavigationService.GoBack(); 
            });
        }

        // linkedin

        // Twitter
        string OAuthTokenKey = string.Empty;
        string tokenSecret = string.Empty;
        string accessToken = string.Empty;
        string accessTokenSecret = string.Empty;
        string userID = string.Empty;
        string userScreenName = string.Empty;

        void requestTokenQuery_QueryResponse(object sender, WebQueryResponseEventArgs e)
        {
            try
            {
                StreamReader reader = new StreamReader(e.Response);
                string strResponse = reader.ReadToEnd();
                var parameters = MainUtil.GetQueryParameters(strResponse);
                OAuthTokenKey = parameters["oauth_token"];
                tokenSecret = parameters["oauth_token_secret"];
                var authorizeUrl = AppSettings.AuthorizeUri + "?oauth_token=" + OAuthTokenKey;

                Dispatcher.BeginInvoke(() =>
                {
                    this.AuthenticationBrowser.Navigate(new Uri(authorizeUrl, UriKind.RelativeOrAbsolute));
                });
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Twitter login error: "+ex.ToString());
                });
            }
        }

        void AccessTokenQuery_QueryResponse(object sender, WebQueryResponseEventArgs e)
        {
            try
            {
                StreamReader reader = new StreamReader(e.Response);
                string strResponse = reader.ReadToEnd();
                var parameters = MainUtil.GetQueryParameters(strResponse);
                accessToken = parameters["oauth_token"];
                accessTokenSecret = parameters["oauth_token_secret"];
                userID = parameters["user_id"];
                userScreenName = parameters["screen_name"];

                MainUtil.SetKeyValue<string>("AccessToken", accessToken);
                MainUtil.SetKeyValue<string>("AccessTokenSecret", accessTokenSecret);
                MainUtil.SetKeyValue<string>("ScreenName", userScreenName);
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Access Granted for " + userScreenName);
                    App.twaccesstokenkey = accessToken;
                    App.twaccesstokensecret = accessTokenSecret;
                    App.writeBackToken();
                    NavigationService.GoBack();
                });

            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("2)"+ex.ToString());
                });
            }
        }

        // Twitter
    }
}