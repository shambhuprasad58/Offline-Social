using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using System.IO;
using OfflineSocial;
using System.Threading;
using System.Collections;
using DataAccess;


namespace Offline_Social
{
    public partial class pivot : PhoneApplicationPage
    {
        public static string vstartdate;
        public static string vstarttime;
        public static string vexptime;
        public static string vexpdate;
        public static BitmapImage image = null;
        public static Stream imageStream = null;
        public bool canProceed = false;
        public static string status = "";
        public int statusLength = 0;
        public static string friendId = "";
        public static string imageName = "";
        public static Boolean imageStatus = false;

        PhotoChooserTask photoChooserTask;

        public pivot()
        {
            InitializeComponent();
            uiupdate();
            scheduleTD();

            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            updateFbFriendLongListSelector();
        }

        public void updateFbFriendLongListSelector()
        {
            List<AlphaKeyGroup<FbFriend>> DataSource = AlphaKeyGroup<FbFriend>.CreateGroups(App.fbFriendsData.friends,
                System.Threading.Thread.CurrentThread.CurrentUICulture,
                (FbFriend s) => { return s.name; }, true);
            FriendList.ItemsSource = DataSource;
        }

        private void Image_Chooser_OnClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            photoChooserTask.Show();
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                MessageBox.Show(e.ChosenPhoto.Length.ToString());
                //Code to display the photo on the page in an image control named myImage.
                System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                Chosen_Image.Source = bmp;
                image = bmp;
                imageStream = e.ChosenPhoto;
                imageName = e.OriginalFileName;
                imageStatus = true;
            }
        }

        private void scheduleTD()
        {
            startdate.Value = DateTime.Now.Date;
            expdate.Value = DateTime.Now.AddDays(1);
        }

        private void back_Tap(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }


        private void next_Tap(object sender, EventArgs e)
        {
            checklogselected();
            if (!canProceed)
                return;
            checkschedule();
            if (!(App.finalExpTime.CompareTo(App.finalStartTime) > 0 && App.finalExpTime.CompareTo(DateTime.Now) > 0))
            {
                MessageBox.Show("Check your start and expiry time");
                return;
            }
            if (statustextbox.Text == null || statustextbox.Text == "")
            {
                MessageBox.Show("Empty status not allowed");
                return;
            }
            NavigationService.Navigate(new Uri("/Page3.xaml", UriKind.Relative));
        }

        public void checkschedule()
        {
            DateTime startDay = (DateTime)startdate.Value;
            DateTime startTime = (DateTime)starttime.Value;
            App.finalStartTime = new DateTime(startDay.Year, startDay.Month, startDay.Day, startTime.Hour, startTime.Minute, 0, startTime.Kind);


            DateTime expDay = (DateTime)expdate.Value;
            DateTime expTime = (DateTime)exptime.Value;
            App.finalExpTime = new DateTime(expDay.Year, expDay.Month, expDay.Day, expTime.Hour, expTime.Minute, 0, expTime.Kind);


            //MessageBox.Show(App.finalStartTime.ToString());
            //MessageBox.Show(App.finalExpTime.ToString());
        }

        private void checklogselected()
        {
            if (App.fb_selected || App.ld_selected || App.tw_selected)
            {
                if (App.fb_selected)
                {
                    if (App.fbaccesstokenkey != "" && App.fbaccesstokenkey != null)
                        canProceed = true;
                    else
                    {
                        MessageBox.Show("You are not loggedin in Facebook, Either deselct or log in ");
                        canProceed = false;
                        return;
                    }
                }



                if (App.tw_selected)
                {
                    if (App.twaccesstokenkey != "" && App.twaccesstokenkey != null)
                        canProceed = true;
                    else
                    {
                        MessageBox.Show("You are not loggedin in Twitter, Either deselct or log in ");
                        canProceed = false;
                        return;
                    }
                }


                if (App.ld_selected)
                {
                    if (App.ldaccesstokenkey != "" && App.ldaccesstokenkey != null)
                        canProceed = true;
                    else
                    {
                        MessageBox.Show("You are not loggedin in Linkedin, Either deselct or log in ");
                        canProceed = false;
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show(" You have not selected any Social Network to Post in");
                canProceed = false;
            }

        }


        private void ldPanel_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BitmapImage tn = new BitmapImage();
            if (!App.ld_selected)
            {
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/ldselected.png", UriKind.Relative)).Stream);
                App.ld_selected = true;
            }
            else
            {
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/ld.png", UriKind.Relative)).Stream);
                App.ld_selected = false;
            }
            ld.Source = tn;
        }

        private void twPanel_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BitmapImage tn = new BitmapImage();
            if (!App.tw_selected)
            {
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/twselected.png", UriKind.Relative)).Stream);
                App.tw_selected = true;
            }
            else
            {
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/tw.png", UriKind.Relative)).Stream);
                App.tw_selected = false;
            }
            tw.Source = tn;
        }

        private void fbPanel_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BitmapImage tn = new BitmapImage();
            if (!App.fb_selected)
            {
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/fbselected.png", UriKind.Relative)).Stream);
                App.fb_selected = true;
            }
            else
            {
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/fb.png", UriKind.Relative)).Stream);
                App.fb_selected = false;
            }
            fb.Source = tn;
        }

        private void twLogin_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (App.twaccesstokenkey != null && App.twaccesstokensecret != "" && App.twaccesstokenkey != null && App.twaccesstokensecret != "")
            {
                App.twaccesstokenkey = "";
                App.twaccesstokensecret = "";
                App.writeBackToken();
                uiupdate();
            }
            else
            {
                App.loginselected = 2;
                NavigationService.Navigate(new Uri("/webbrowser.xaml", UriKind.Relative));
                NavigationService.Navigated += navigationService_Navigated;
            }
        }

        private void ldLogin_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (App.ldaccesstokenkey != "" && App.ldaccesstokenkey != null)
            {
                App.ldaccesstokenkey = "";
                App.writeBackToken();
                uiupdate();
            }
            else
            {
                App.loginselected = 3;
                NavigationService.Navigate(new Uri("/webbrowser.xaml", UriKind.Relative));
                NavigationService.Navigated += navigationService_Navigated;
            }
        }

        private void fbLogin_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (App.fbaccesstokenkey != null && App.fbaccesstokenkey != "")
            {
                App.fbaccesstokenkey = "";
                App.writeBackToken();
                uiupdate();
            }
            else
            {
                App.loginselected = 1;
                NavigationService.Navigate(new Uri("/webbrowser.xaml", UriKind.Relative));
                NavigationService.Navigated += navigationService_Navigated;
            }
        }

        public void uiupdate()
        {
            if (App.fbaccesstokenkey != "" && App.fbaccesstokenkey != null)
            {
                BitmapImage ui = new BitmapImage();
                ui.SetSource(Application.GetResourceStream(new Uri(@"Assets\fblogout.png", UriKind.Relative)).Stream);
                fblog.Source = ui;
                //retrieveFbFriendList();
            }
            else
            {
                BitmapImage ui = new BitmapImage();
                ui.SetSource(Application.GetResourceStream(new Uri(@"Assets\fblogin.png", UriKind.Relative)).Stream);
                fblog.Source = ui;
                App.fbFriendsData.friends = new List<FbFriend>();
                App.fbFriendsData.writeToFile();
                updateFbFriendLongListSelector();
            }
            if (App.twaccesstokenkey != "" && App.twaccesstokenkey != null)
            {
                BitmapImage ui = new BitmapImage();
                ui.SetSource(Application.GetResourceStream(new Uri(@"Assets\twlogout.png", UriKind.Relative)).Stream);
                twlog.Source = ui;
            }
            else
            {
                BitmapImage ui = new BitmapImage();
                ui.SetSource(Application.GetResourceStream(new Uri(@"Assets\twlogin.png", UriKind.Relative)).Stream);
                twlog.Source = ui;
            }

            if (App.ldaccesstokenkey != "" && App.ldaccesstokenkey != null)
            {
                BitmapImage ui = new BitmapImage();
                ui.SetSource(Application.GetResourceStream(new Uri(@"Assets\ldlogout.png", UriKind.Relative)).Stream);
                ldlog.Source = ui;
            }
            else
            {
                BitmapImage ui = new BitmapImage();
                ui.SetSource(Application.GetResourceStream(new Uri(@"Assets\ldlogin.png", UriKind.Relative)).Stream);
                ldlog.Source = ui;
            }
        }

        public void navigationService_Navigated(object sender, NavigationEventArgs e)
        {
            uiupdate();
        }


        private void statusChanged(object sender, TextChangedEventArgs e)
        {
            status = statustextbox.Text;
            chused.Text = status.Length + "";
            statusLength = status.Length;
        }

        private void statustextbox_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            if (statustextbox.SelectionStart == statustextbox.Text.Length)
            {
                scroller.ScrollToVerticalOffset(statustextbox.ActualHeight);

            }
        }

        
        public async void retrieveFbFriendList()
        {
            if (App.fbaccesstokenkey != null && App.fbaccesstokenkey != "")
            {
                dynamic friends = null;
                try
                {
                    var facebookClient = new Facebook.FacebookClient(App.fbaccesstokenkey);
                    var parameters = new Dictionary<string, object>();
                    parameters["access_token"] = App.fbaccesstokenkey;
                    //parameters["fields"] = "id,name";
                    //parameters["limit"] = "5000";
                    //parameters["offset"] = "0";


                    friends = await facebookClient.GetTaskAsync("/me/friends");

                    List<FbFriend> resultList = new List<FbFriend>();


                    // Thread.Sleep(2000);

                    foreach (var item in friends.data)
                    {
                        var newdata = item;
                        string item1 = newdata.id.ToString();
                        string item2 = newdata.name.ToString();
                        FbFriend newFriend = new FbFriend(item1, item2);
                        if (resultList.Contains(newFriend))
                        {
                            item1 = item1 + "duplicate";
                        }

                        resultList.Add(new FbFriend(item1, item2));
                    }
                    App.fbFriendsData.friends = resultList;
                    App.fbFriendsData.writeToFile();
                    Dispatcher.BeginInvoke(() =>
                    {
                        //MessageBox.Show("Retrieved Friend List, Count: " + resultList.Count, "Result", MessageBoxButton.OK);
                        List<AlphaKeyGroup<FbFriend>> DataSource = AlphaKeyGroup<FbFriend>.CreateGroups(resultList,
                            System.Threading.Thread.CurrentThread.CurrentUICulture,
                            (FbFriend s) => { return s.name; }, true);
                        FriendList.ItemsSource = DataSource;
                    });
                }
                catch (Exception ex)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (friends != null)
                            MessageBox.Show("Exception during post: " + friends.ToString(), "Error", MessageBoxButton.OK);
                        else
                            MessageBox.Show("Exception during post: " + ex.ToString(), "Error", MessageBoxButton.OK);
                    });
                }
            }
        }

        private void refresh_Tap(object sender, EventArgs e)
        {
            retrieveFbFriendList();
            friendId = "";
            FbFriendSelected.Text = "Me";
        }

        private void delete_Tap(object sender, EventArgs e)
        {
            retrieveFbFriendList();
            BitmapImage img = new BitmapImage(new Uri("/Assets/Icon_choose.jpg", UriKind.Relative));
            Chosen_Image.Source = img;
            image = null;
            imageStream = null;
            statustextbox.Text = "";
            chused.Text = "0";
            startdate.Value = DateTime.Now.Date;
            expdate.Value = DateTime.Now.AddDays(1);
            vstartdate = null;
            vstarttime = null;
            vexpdate = null;
            vexptime = null;
            status = "";
            statusLength = 0;
            canProceed = false;
            friendId = "";
            FbFriendSelected.Text = "Me";
        }

        private void Friend_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic selectedItem = FriendList.SelectedItem;
            friendId = selectedItem.id;
            FbFriendSelected.Text = selectedItem.name;
        }
    }

}