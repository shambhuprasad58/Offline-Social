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
using Facebook;
using DataAccess;

namespace Offline_Social
{
    public partial class Page0 : PhoneApplicationPage
    {
        public Page0()
        {
            InitializeComponent();
            uiupdate();
        }
        UserName names = new UserName();

        
        public void fb_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //MessageBox.Show(App.fbaccesstokenkey);
            BitmapImage tn = new BitmapImage();
            if (!App.fb_selected)
            {
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/fb2.png", UriKind.Relative)).Stream);
                App.fb_selected = true;
            }
            else
            {
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/fb.png", UriKind.Relative)).Stream);
                App.fb_selected = false;
            }
            fb.Source = tn;

        }

        public void twitter_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //MessageBox.Show(App.twaccesstokenkey);
            BitmapImage tn = new BitmapImage();
            if (!App.tw_selected)
            {
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/tw2.png", UriKind.Relative)).Stream);
                App.tw_selected = true;
            }
            else
            {
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/tw.png", UriKind.Relative)).Stream);
                App.tw_selected = false;
            }
            twitter.Source = tn;
        }


        public void ld_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //MessageBox.Show(App.ldaccesstokenkey);
            BitmapImage tn = new BitmapImage();
            if (!App.ld_selected)
            {
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/ld2.png", UriKind.Relative)).Stream);
                App.ld_selected = true;
            }
            else
            {
                tn.SetSource(Application.GetResourceStream(new Uri(@"Assets/ld.png", UriKind.Relative)).Stream);
                App.ld_selected = false;
            }
            ld.Source = tn;
        }

        private void next_Tap(object sender, EventArgs e)
        {
            checklogselected();
            if(canProceed)
                NavigationService.Navigate(new Uri("/Page1.xaml", UriKind.Relative));
        }
        public bool canProceed = false;
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

        private void back_Tap(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void fblogintap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.loginselected = 1;
            NavigationService.Navigate(new Uri("/webbrowser.xaml", UriKind.Relative));
            NavigationService.Navigated += navigationService_Navigated;
        }

        private void ldlogintap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.loginselected = 3;
            NavigationService.Navigate(new Uri("/webbrowser.xaml", UriKind.Relative));
            NavigationService.Navigated += navigationService_Navigated;

        }

        private void twlogintap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.loginselected = 2;
            NavigationService.Navigate(new Uri("/webbrowser.xaml", UriKind.Relative));
            NavigationService.Navigated += navigationService_Navigated;
        }
        
        public void uiupdate ()
        {
            if (App.fbaccesstokenkey != "" && App.fbaccesstokenkey != null)
            {
                BitmapImage ui = new BitmapImage();
                ui.SetSource(Application.GetResourceStream(new Uri(@"assets\logout.png", UriKind.Relative)).Stream);
                fblog.Source = ui;
            }
            if (App.twaccesstokenkey != "" && App.twaccesstokenkey != null)
            {
                BitmapImage ui = new BitmapImage();
                ui.SetSource(Application.GetResourceStream(new Uri(@"assets\logout.png", UriKind.Relative)).Stream);
                twlog.Source = ui;
            }
            if (App.ldaccesstokenkey != "" && App.ldaccesstokenkey != null)
            {
                BitmapImage ui = new BitmapImage();
                ui.SetSource(Application.GetResourceStream(new Uri(@"assets\logout.png", UriKind.Relative)).Stream);
                ldlog.Source = ui;
            }
        }

       

        public void navigationService_Navigated(object sender, NavigationEventArgs e)
        {
            uiupdate();
        }


        public void getFbName()
        {
            if (App.fbaccesstokenkey != null && App.fbaccesstokenkey != "")
            {
                var fb = new FacebookClient(App.fbaccesstokenkey);

                fb.GetCompleted += (o, e) =>
                    {
                        if (e.Error == null)
                        {
                            var result = (IDictionary<string, object>)e.GetResultData();
                            var firstName = (string)result["first_name"];
                            var lastName = (string)result["last_name"];
                            names.fbname = firstName + " " + lastName;
                            names.writeToFile();
                        }
                        else
                        {
                            //Dispatcher.BeginInvoke(() => MessageBox.Show(args.Error.Message));
                        }
                    };

                fb.GetTaskAsync("me");
            }
            
        }

    }
}