#define DEBUG_AGENT

using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using System.Collections.Generic;
using System.Windows.Threading;
using System;
using RestSharp;
using Hammock.Authentication.OAuth;
using Hammock.Web;
using DataAccess;
using System.Net;
using Microsoft.Phone.Shell;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScheduledTaskAgent1
{
    public class ScheduledAgent : ScheduledTaskAgent, INotifyPropertyChanged
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        /// 
        public static string error = "";

        private StatusUpdateDataContext statusUpdateDb;

        // Define an observable collection property that controls can bind to.
        private ObservableCollection<statusUpdateItem> _statusUpdateItems;
        public ObservableCollection<statusUpdateItem> StatusUpdateItems
        {
            get
            {
                return _statusUpdateItems;
            }
            set
            {
                if (_statusUpdateItems != value)
                {
                    _statusUpdateItems = value;
                    NotifyPropertyChanged("StatusUpdateItems");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        

        static ScheduledAgent()
        {
            

            // Data context and observable collection are children of the main page.
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        /// 
        public ScheduledTask Task;
        public int scheduledUpdates, completedUpdates;
        protected override void OnInvoke(ScheduledTask task)
        {
            Task = task;
            scheduledUpdates = 0;
            completedUpdates = 0;
            statusUpdateDb = new StatusUpdateDataContext(StatusUpdateDataContext.DBConnectionString);
            // Define the query to gather all of the to-do items.
            var statusItemsInDB = from statusUpdateItem statusItem in statusUpdateDb.StatusUpdateItems
                                  select statusItem;
            NetworkInformationUtility.GetNetworkTypeCompleted += GetNetworkTypeCompleted;

            NetworkInformationUtility.GetNetworkTypeAsync(3000); // Timeout of 3 seconds
            // Execute the query and place the results into a collection.
            StatusUpdateItems = new ObservableCollection<statusUpdateItem>(statusItemsInDB);
            //TODO: Add code to perform your task in background
            string toastMessage = "Started";
            ShellToast toast = new ShellToast();
            toast.Title = "Started";
            toast.Content = toastMessage;
            toast.Show();
            //var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            //timer.Tick += (sender2, args) => { stopPosting(); timer.Stop(); };
            //timer.Start();
            //var timer2 = new DispatcherTimer { Interval = TimeSpan.FromSeconds(24) };
            //timer2.Tick += (sender3, args3) => { notify(); timer2.Stop(); };
            //timer2.Start();
            

            //postOnFb();
            //postOnLinkedin();
            //postOnTwitter();
            //NotifyComplete();
            
        }

        public void startPosting()
        {
            int i = 0;
            foreach(statusUpdateItem item in StatusUpdateItems)
            {
                i++;
                string status = item.status;
                string fbtoken = item.fbAccessToken;
                string twtoken = item.twAccessToken;
                string twsecret = item.twAccessTokenSecret;
                string ldtoken = item.ldAccessToken;
                if (item.startTime.CompareTo(DateTime.Now) <= 0 && item.endTime.CompareTo(DateTime.Now) >= 0)
                {
                    if (item.selected % 10 == 1)
                        postOnFb(status, fbtoken);
                    if (item.selected % 100 > 1)
                        postOnTwitter(status, twtoken, twsecret);
                    if (item.selected > 11)
                        postOnLinkedin(status, ldtoken);
                    statusUpdateDb.StatusUpdateItems.DeleteOnSubmit(item);
                    statusUpdateDb.SubmitChanges();
                }
                if (item.endTime.CompareTo(DateTime.Now) < 0)
                {
                    string toastMessage2 = "End Time Exceeded";
                    ShellToast toast2 = new ShellToast();
                    toast2.Title = "End";
                    toast2.Content = toastMessage2;
                    toast2.Show();
                    statusUpdateDb.StatusUpdateItems.DeleteOnSubmit(item);
                    statusUpdateDb.SubmitChanges();
                }
            }
            if (i == 0)
            {
                notify();
            }
        }

        private void notify()
        {
            //TileUpdate tile = new TileUpdate();
            //tile.btnIconicTile_Click(StatusUpdateItems.Count);
            string toastMessage = "Notify Started";
            ShellToast toast = new ShellToast();
            toast.Title = ".";
            toast.Content = toastMessage;
            toast.Show();
            #if DEBUG_AGENT
                        ScheduledActionService.LaunchForTest(Task.Name, TimeSpan.FromSeconds(60));
            #endif
            NotifyComplete();
        }



        private void GetNetworkTypeCompleted(object sender, NetworkTypeEventArgs networkTypeEventArgs)
        {
            if (networkTypeEventArgs.HasTimeout)
            {
                string toastMessage = "network timeout";
                ShellToast toast = new ShellToast();
                toast.Title = ".";
                toast.Content = toastMessage;
                toast.Show();
                notify();
            }
            else if (networkTypeEventArgs.HasInternet)
            {
                startPosting();
            }
            else
            {
                string toastMessage = "no network";
                ShellToast toast = new ShellToast();
                toast.Title = ".";
                toast.Content = toastMessage;
                toast.Show();
                notify();

            }

            // Always dispatch on the UI thread




        }

        ///->>>>>>>>> post
        ///
        /// 
        // Facebook ---------------------------------------------------------------------------------------------->>
        private void postOnFb(string status, string fbtoken)
        {
            string toastMessage = "FB Started";
            ShellToast toast = new ShellToast();
            toast.Title = ".";
            toast.Content = toastMessage;
            toast.Show();
            scheduledUpdates++;
            this.PostToWall(status, fbtoken);
        }
        Facebook.FacebookClient facebookClient;
        private async void PostToWall(string status, string fbtoken)
        {
            try
            {
                facebookClient = new Facebook.FacebookClient(fbtoken);
                var parameters = new Dictionary<string, object>();
                var parameters1 = new Dictionary<string, object>();

                parameters["message"] = status;

                dynamic fbPostTaskResult = await facebookClient.PostTaskAsync("/me/feed", parameters);
                var result = (IDictionary<string, object>)fbPostTaskResult;

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    string toastMessage = "Posted Open Graph Action, id: " + (string)result["id"];
                    ShellToast toast = new ShellToast();
                    toast.Title = "Background Agent Sample";
                    toast.Content = toastMessage;
                    toast.Show();
                    completedUpdates++;
                    if (completedUpdates == scheduledUpdates)
                    {
                        notify();
                    }
                    //NotifyComplete();
                    //MessageBox.Show("Posted Open Graph Action, id: " + (string)result["id"], "Result", MessageBoxButton.OK);
                });
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    string toastMessage = "" + ex.ToString();
                    ShellToast toast = new ShellToast();
                    toast.Title = ".";
                    toast.Content = toastMessage;
                    toast.Show();
                    error = error + toastMessage;
                    completedUpdates++;
                    if (completedUpdates == scheduledUpdates)
                    {
                        notify();
                    }
                    //NotifyComplete();
                    //MessageBox.Show("Exception during post: " + ex.ToString(), "Error", MessageBoxButton.OK);
                });
            }
        }


        // Facebook



        // Linkedin ----------------------------------------------------------------------------------------------->>
        public string ldresponse = "";
        private void postOnLinkedin(string status, string ldtoken)
        {
            string toastMessage = "LD Started";
            ShellToast toast = new ShellToast();
            toast.Title = ".";
            toast.Content = toastMessage;
            toast.Show();
            scheduledUpdates++;
            var shareMsg = new
            {
                comment = status,
                //content = new
                //{
                //    title = textbox.Text,
                //    submitted_url ="Via Social",
                //    //submitted_image_url =" "
                //},
                visibility = new
                {
                    code = "anyone"
                }
            };
            String requestUrl = "https://api.linkedin.com/v1/people/~/shares?oauth2_access_token=" + ldtoken;

            RestSharp.RestClient client = new RestSharp.RestClient();
            IRestRequest request = (IRestRequest)new RestSharp.RestRequest(requestUrl, Method.POST);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("x-li-format", "json");

            request.RequestFormat = DataFormat.Json;
            request.AddBody(shareMsg);
            var asyncHandle2 = client.ExecuteAsync<RESponse>(request, response =>
            {
                ldresponse = response.Content;
                string toastMessage2 = ldresponse;
                ShellToast toast2 = new ShellToast();
                toast2.Title = "Background Agent Sample";
                toast2.Content = toastMessage2;
                toast2.Show();
                completedUpdates++;
                if (completedUpdates == scheduledUpdates)
                {
                    notify();
                }
                //MessageBox.Show(ldresponse);
            });
        }

        public class RESponse
        {
            string resp { get; set; }
        }

        // Linkedin 
        public static string statusstring;
        // Twitter ------------------------------------------------------------------------------------------------->>

        public void postOnTwitter(string status, string twtoken, string twsecret)
        {
            string toastMessage = "TW Started";
            ShellToast toast = new ShellToast();
            toast.Title = ".";
            toast.Content = toastMessage;
            toast.Show();
            scheduledUpdates++;
            var credentials = new OAuthCredentials
            {
                Type = OAuthType.ProtectedResource,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                ConsumerKey = AppSettings.consumerKey,
                ConsumerSecret = AppSettings.consumerKeySecret,
                Token = twtoken,
                TokenSecret = twsecret,
                Version = "1.0"
            };

            var restClient = new Hammock.RestClient
            {
                Authority = "https://api.twitter.com",
                HasElevatedPermissions = true
            };

            var restRequest = new Hammock.RestRequest
            {
                Credentials = credentials,
                Path = "/1.1/statuses/update.json",
                Method = WebMethod.Post
            };
            Random rd = new Random();
            statusstring = rd.Next(10000).ToString() + rd.Next(100000).ToString();
            restRequest.AddParameter("status", status);
            restClient.BeginRequest(restRequest, new Hammock.RestCallback(PostTweetRequestCallback));
        }
        private void PostTweetRequestCallback(Hammock.RestRequest request, Hammock.RestResponse response, object obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string toastMessage = "TWEET_POSTED_SUCCESSFULLY";
                    ShellToast toast = new ShellToast();
                    toast.Title = "Background Agent Sample";
                    toast.Content = toastMessage;
                    toast.Show();
                    completedUpdates++;
                    if (completedUpdates == scheduledUpdates)
                    {
                        notify();
                    }
                    //MessageBox.Show("TWEET_POSTED_SUCCESSFULLY");
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    string toastMessage = "TWEET_POST_ERR_UPDATE_LIMIT";
                    ShellToast toast = new ShellToast();
                    toast.Title = "Background Agent Sample";
                    toast.Content = toastMessage;
                    toast.Show();
                    completedUpdates++;
                    if (completedUpdates == scheduledUpdates)
                    {
                        notify();
                    }
                    //MessageBox.Show("TWEET_POST_ERR_UPDATE_LIMIT");
                }
                else
                {
                    string toastMessage = "TWEET_POST_ERR_FAILED";
                    ShellToast toast = new ShellToast();
                    toast.Title = "Background Agent Sample";
                    toast.Content = toastMessage;
                    toast.Show();
                    completedUpdates++;
                    if (completedUpdates == scheduledUpdates)
                    {
                        notify();
                    }
                    //MessageBox.Show("TWEET_POST_ERR_FAILED");
                }

            });
        }
        // Twitter ----------------------------------------------------------------------------------------------------->>

        ///




    }



}