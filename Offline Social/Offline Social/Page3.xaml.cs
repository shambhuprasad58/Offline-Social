using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RestSharp;
using Hammock.Authentication.OAuth;
using Hammock.Web;
using Hammock;
using System.Text;
using ScheduledTaskAgent1;
using DataAccess;
using Microsoft.Phone.Scheduler;
using System.Threading;
using System.IO.IsolatedStorage;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Facebook;
using System.IO;
using System.Windows.Media.Imaging;
//using TweetSharp;
using LinqToTwitter;


namespace Offline_Social
{
    public partial class Page3 : PhoneApplicationPage, INotifyPropertyChanged
    {
        private static BitmapImage image = pivot.image;
        private static byte[] byteImage;
        private static Stream imageStream = pivot.imageStream;
        private static string friendId = pivot.friendId;
        private static Boolean imageStatus = pivot.imageStatus;
        private static string imageName = pivot.imageName;

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



        public Page3()
        {
            InitializeComponent();
            statusUpdateDb = new StatusUpdateDataContext(StatusUpdateDataContext.DBConnectionString);

            // Data context and observable collection are children of the main page.
            this.DataContext = this;

            if (imageStatus)
            {
                byteImage = ImageToBytes(image);
                Chosen_Image.Source = image;
            }
            statuspreview();

        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Define the query to gather all of the to-do items.
            var statusItemsInDB = from statusUpdateItem statusItem in statusUpdateDb.StatusUpdateItems
                                  select statusItem;

            // Execute the query and place the results into a collection.
            StatusUpdateItems = new ObservableCollection<statusUpdateItem>(statusItemsInDB);

            
            // Call the base method.
            base.OnNavigatedTo(e);
        }

        private void GetNetworkTypeCompleted(object sender, NetworkTypeEventArgs networkTypeEventArgs)
        {
            string message;
            if (networkTypeEventArgs.HasTimeout)
            {
                message = "Timeout occurred";
                Dispatcher.BeginInvoke(() => MessageBox.Show(message));
                CustomMessageBox();
            }
            else if (networkTypeEventArgs.HasInternet)
            {
                message = "The Internet connection type is: " + networkTypeEventArgs.Type.ToString();
                Dispatcher.BeginInvoke(() => MessageBox.Show(message));

                live_update();

            }
            else
            {
                message = "There is no Internet connection";
                Dispatcher.BeginInvoke(() => MessageBox.Show(message));
                CustomMessageBox();
            }

            // Always dispatch on the UI thread
            

            
        }

        public void CustomMessageBox()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                CustomMessageBox messageBox = new CustomMessageBox()
                {
                    Caption = "Queue update on server?",
                    Message = "You can queue update on our server for quick update or via half an hour scheduler on phone",
                    LeftButtonContent = "Yes",
                    RightButtonContent = "No"
                };

                int selected = 0;
                if (App.fb_selected)
                    selected += 1;
                if (App.tw_selected)
                    selected += 10;
                if (App.ld_selected)
                    selected += 100;
                statusUpdateItem newitem = new statusUpdateItem
                {
                    status = statusString,
                    startTime = App.finalStartTime,
                    endTime = App.finalExpTime,
                    selected = selected,
                    picFolder = "",
                    uploadStatus = 0,
                    fbAccessToken = App.fbaccesstokenkey,
                    twAccessToken = App.twaccesstokenkey,
                    twAccessTokenSecret = App.twaccesstokensecret,
                    ldAccessToken = App.ldaccesstokenkey
                };
                messageBox.Dismissed += (s1, e1) =>
                {
                    switch (e1.Result)
                    {
                        case CustomMessageBoxResult.LeftButton:
                            UploadService serverUpload = new UploadService();
                            serverUpload.dispatcher = Dispatcher;
                            serverUpload.item = newitem;
                            serverUpload.StartUpload();
                            break;
                        case CustomMessageBoxResult.RightButton:
                            StatusUpdateItems.Add(newitem);
                            statusUpdateDb.StatusUpdateItems.InsertOnSubmit(newitem);
                            statusUpdateDb.SubmitChanges();

                            break;
                        case CustomMessageBoxResult.None:
                            StatusUpdateItems.Add(newitem);
                            statusUpdateDb.StatusUpdateItems.InsertOnSubmit(newitem);
                            statusUpdateDb.SubmitChanges();

                            StartPeriodicTask();
                            break;
                        default:
                            StatusUpdateItems.Add(newitem);
                            statusUpdateDb.StatusUpdateItems.InsertOnSubmit(newitem);
                            statusUpdateDb.SubmitChanges();

                            StartPeriodicTask();
                            break;
                    }
                };

                messageBox.Show();

            });
        }


        public void live_update()
        {
            if (App.tw_selected)
                postOnTwitter(statusString);
            if (App.ld_selected)
                postOnLinkedin(statusString);
            if (App.fb_selected)
                postOnFb(statusString);
        }


        public string statusString { get; set; }

        private void statuspreview()
        {
            textbox.Text = pivot.status;
        }

        private void back_Tap(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void post_Tap(object sender, EventArgs e)
        {
            statusString = textbox.Text;
            statusString = statusString.Replace("\r", "\n");

            if (App.finalStartTime.CompareTo(DateTime.Now) <= 0 && App.finalExpTime.CompareTo(DateTime.Now) >= 0)
            {
                if (App.NetworkCheckEventFlag)
                {
                    NetworkInformationUtility.GetNetworkTypeCompleted += GetNetworkTypeCompleted;
                    App.NetworkCheckEventFlag = false;
                }

                NetworkInformationUtility.GetNetworkTypeAsync(3000); // Timeout of 3 seconds
            }
            else
            {
                if (App.finalExpTime.CompareTo(DateTime.Now) < 0)
                {
                    MessageBox.Show("Check the expiry time !");
                    return;
                }
                else
                {
                    CustomMessageBox();
                }
            }

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

            //MessageBox.Show("Post button was clicked !");
        }
        private void StartPeriodicTask()
        {
            PeriodicTask periodicTask = new PeriodicTask("PeriodicTaskDemo");
            periodicTask.Description = "Are presenting a periodic task";
            try
            {
                ScheduledActionService.Add(periodicTask);
                ScheduledActionService.LaunchForTest("PeriodicTaskDemo", TimeSpan.FromSeconds(3));
                Dispatcher.BeginInvoke(() => MessageBox.Show("Open the background agent success"));
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("exists already"))
                {
                    //Dispatcher.BeginInvoke(() => MessageBox.Show("Since then the background agent success is already running"));
                }
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show("Background processes for this application has been prohibited. Enable it in settings to enjoy all features."));
                }
                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type has already been added."))
                {
                    //Dispatcher.BeginInvoke(() => MessageBox.Show("You open the daemon has exceeded the hardware limitations"));
                }
            }
            catch (SchedulerServiceException)
            {

            }
        }
        private void StopPeriodicTask()
        {
            try
            {
                ScheduledActionService.Remove("PeriodicTaskDemo");
                //Dispatcher.BeginInvoke(() => MessageBox.Show("Turn off the background agent successfully"));
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("doesn't exist"))
                {
                    //Dispatcher.BeginInvoke(() => MessageBox.Show("Since then the background agent success is not running"));
                }
            }
            catch (SchedulerServiceException)
            {

            }
        }


        // Facebook ---------------------------------------------------------------------------------------------->>
        private void postOnFb(string status)
        {
            this.PostToWall(status);
        }
        Facebook.FacebookClient facebookClient;
        private async void PostToWall(string status)
        {
            try
            {
                facebookClient = new Facebook.FacebookClient(App.fbaccesstokenkey);
                var parameters = new Dictionary<string, object>();
                //var parameters1 = new Dictionary<string, object>();

                parameters["message"] = status;
                parameters["caption"] = string.Empty;
                parameters["description"] = string.Empty;
                parameters["req_perms"] = "publish_stream";
                parameters["scope"] = "publish_stream";
                parameters["type"] = "normal";
                dynamic fbPostTaskResult = null;
                if (imageStatus)
                {
                    var mediaObject = new FacebookMediaObject
                    {
                        ContentType = "image/jpeg",
                        FileName = "pokemon.jpg"
                    }.SetValue(byteImage);
                    parameters["source"] = mediaObject;
                    fbPostTaskResult = await facebookClient.PostTaskAsync("/me/photos", parameters);
                }
                else
                {
                    fbPostTaskResult = await facebookClient.PostTaskAsync("/me/feed", parameters);
                }
                var result = (IDictionary<string, object>)fbPostTaskResult;

                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Posted Open Graph Action, id: " + (string)result["id"], "Result", MessageBoxButton.OK);
                });
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Exception during post: " + ex.ToString(), "Error", MessageBoxButton.OK);
                });
            }
        }


        public static byte[] ImageToBytes(BitmapImage img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                WriteableBitmap btmMap = new WriteableBitmap(img);
                System.Windows.Media.Imaging.Extensions.SaveJpeg(btmMap, ms, img.PixelWidth, img.PixelHeight, 0, 100);
                img = null;
                return ms.ToArray();
            }
        }


        // Facebook



        // Linkedin ----------------------------------------------------------------------------------------------->>
        public string ldresponse = "";
        private void postOnLinkedin(string status)
        {
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

            String requestUrl = "https://api.linkedin.com/v1/people/~/shares?oauth2_access_token=" + App.ldaccesstokenkey;

            RestSharp.RestClient client = new RestSharp.RestClient();
            IRestRequest request = (IRestRequest)new RestSharp.RestRequest(requestUrl, Method.POST);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("x-li-format", "json");

            request.RequestFormat = DataFormat.Json;
            request.AddBody(shareMsg);
            var asyncHandle2 = client.ExecuteAsync<RESponse>(request, response =>
            {
                ldresponse = response.Content;
                Dispatcher.BeginInvoke(() => MessageBox.Show(ldresponse));
            });
        }

        public class RESponse
        {
            string resp { get; set; }
        }

        // Linkedin 

        // Twitter ------------------------------------------------------------------------------------------------->>

/*        public void postOnTwitter(string status)
        {
            var credentials = new OAuthCredentials
            {
                Type = OAuthType.ProtectedResource,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                ConsumerKey = AppSettings.consumerKey,
                ConsumerSecret = AppSettings.consumerKeySecret,
                Token = App.twaccesstokenkey,
                TokenSecret = App.twaccesstokensecret,
                Version = "1"
            };

            var restClient = new Hammock.RestClient
            {
                Authority = "https://api.twitter.com",
                HasElevatedPermissions = true
            };

            var restRequest = new Hammock.RestRequest
            {
                Credentials = credentials,
                Path = "/1.1/statuses/update_with_media.json",
                Method = WebMethod.Post
            };

            restRequest.AddParameter("status", status);
            restClient.AddFile("media[]", "julia_pic", imageStream, "image/jpg");
            restClient.BeginRequest(restRequest, new Hammock.RestCallback(PostTweetRequestCallback));
        }
        private void PostTweetRequestCallback(Hammock.RestRequest request, Hammock.RestResponse response, object obj)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show("TWEET_POSTED_SUCCESSFULLY"));
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show("TWEET_POST_ERR_UPDATE_LIMIT"));
                }
                else
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show("TWEET_POST_ERR_FAILED"));
                }

            });
        }
 * 
 * 
 * */

        private async void postOnTwitter(string status)
        {
            try
            {
                var auth = new SingleUserAuthorizer
                {
                    CredentialStore = new SingleUserInMemoryCredentialStore
                    {
                        ConsumerKey = AppSettings.consumerKey,
                        ConsumerSecret = AppSettings.consumerKeySecret,
                        AccessToken = App.twaccesstokenkey,
                        AccessTokenSecret = App.twaccesstokensecret
                    }
                };
                var twitterCtx = new TwitterContext(auth);
                Status tweet = null;
                if (imageStatus)
                {
                    tweet = await twitterCtx.TweetWithMediaAsync(
                       status, false, byteImage);
                }
                else
                {
                    tweet = await twitterCtx.TweetAsync(status);
                }

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (tweet != null)
                    {
                        Dispatcher.BeginInvoke(() => MessageBox.Show("TWEET_POSTED_SUCCESSFULLY"));
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => MessageBox.Show("TWEET_POST_ERR_FAILED"));
                    }

                });
            }
            catch(Exception ex)
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show("TWEET_POST_ERR_FAILED"));
            }


            //var service = new TwitterService(AppSettings.consumerKey, AppSettings.consumerKeySecret);
            //service.AuthenticateWith(App.twaccesstokenkey, App.twaccesstokensecret);
            //Dictionary<string, Stream> images = new Dictionary<string, Stream> { { "julia", imageStream } };
            //service.SendTweetWithMedia(new SendTweetWithMediaOptions { Status = status, Images = images}, (tweet,response)=>
            //    {
            //        Deployment.Current.Dispatcher.BeginInvoke(() =>
            //        {
            //            if (response.StatusCode == HttpStatusCode.OK)
            //            {
            //                Dispatcher.BeginInvoke(() => MessageBox.Show("TWEET_POSTED_SUCCESSFULLY"));
            //            }
            //            else if (response.StatusCode == HttpStatusCode.Forbidden)
            //            {
            //                Dispatcher.BeginInvoke(() => MessageBox.Show("TWEET_POST_ERR_UPDATE_LIMIT"));
            //            }
            //            else
            //            {
            //                Dispatcher.BeginInvoke(() => MessageBox.Show("TWEET_POST_ERR_FAILED"));
            //            }

            //        });
            //    }
            //);
            
        }
        // Twitter ----------------------------------------------------------------------------------------------------->>


    }
}