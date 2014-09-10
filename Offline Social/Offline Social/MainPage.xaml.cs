using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Offline_Social.Resources;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Microsoft.Phone.Scheduler;
using DataAccess;
using System.IO;
using System.Text;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;

namespace Offline_Social
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
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



        public UploadService serviceUpload;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            statusUpdateDb = new StatusUpdateDataContext(StatusUpdateDataContext.DBConnectionString);

            // Data context and observable collection are children of the main page.
            this.DataContext = this;

            serviceUpload = new UploadService();
            serviceUpload.dispatcher = Dispatcher;
            
            serviceUpload.RefreshTransfers();

            var statusItemsInDB = from statusUpdateItem statusItem in statusUpdateDb.StatusUpdateItems
                                  select statusItem;

            // Execute the query and place the results into a collection.
            StatusUpdateItems = new ObservableCollection<statusUpdateItem>(statusItemsInDB);

            queuedCount.Text = StatusUpdateItems.Count.ToString();

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };

            timer.Tick += (sender, args) => { updateTile(); };

            timer.Start();


            StartPeriodicTask();

            //SendPost();

            /*serverFolder folderInfo = new serverFolder();
            if (folderInfo.status == 0)
            {
                folderInfo.serverFolderUpdateInfo(1);
                ServerFolderHandler serverHandler = new ServerFolderHandler();
                serverHandler.dispatcher = Dispatcher;
                serverHandler.StartUpload();
            }
            else
            {
                if (folderInfo.status == 1)
                {
                    ServerFolderHandler serverHandler = new ServerFolderHandler();
                    serverHandler.dispatcher = Dispatcher;
                    serverHandler.RefreshTransfers();
                }
            }
            */

        }

        private void updateTile()
        {
            var statusItemsInDB = from statusUpdateItem statusItem in statusUpdateDb.StatusUpdateItems
                                  select statusItem;

            // Execute the query and place the results into a collection.
            StatusUpdateItems = new ObservableCollection<statusUpdateItem>(statusItemsInDB);

            queuedCount.Text = StatusUpdateItems.Count.ToString();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings.Save();
            Application.Current.Terminate();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Define the query to gather all of the to-do items.
            var statusItemsInDB = from statusUpdateItem statusItem in statusUpdateDb.StatusUpdateItems
                                  select statusItem;

            // Execute the query and place the results into a collection.
            StatusUpdateItems = new ObservableCollection<statusUpdateItem>(statusItemsInDB);

            queuedCount.Text = StatusUpdateItems.Count.ToString();

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };

            timer.Tick += (sender, args) => { updateTile(); };

            timer.Start();
            // Call the base method.
            base.OnNavigatedTo(e);
        }
        

  /*      void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            queuedupdatespanel.Tap += queuedupdatespanel_Tap;
            newupdatepanel.Tap += newupdatepanel_Tap;
            newfeedpanel.Tap += newfeedpanel_Tap;
            morepanel.Tap += morepanel_Tap;
            Storyboard sb = new Storyboard();
            //sb.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimation fadeInAnimation1 = new DoubleAnimation();
            BackEase ease1 = new BackEase();
            ease1.EasingMode = EasingMode.EaseInOut;
            ease1.Amplitude = 1.2;

            fadeInAnimation1.From = -500;
            fadeInAnimation1.To = 0;
            fadeInAnimation1.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            fadeInAnimation1.EasingFunction = ease1;
            Storyboard.SetTarget(fadeInAnimation1, newfeedpaneltransform);
            Storyboard.SetTargetProperty(fadeInAnimation1, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation1);
            DoubleAnimation fadeInAnimation2 = new DoubleAnimation();
            fadeInAnimation2.From = -500;
            fadeInAnimation2.To = 0;
            fadeInAnimation2.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            fadeInAnimation2.EasingFunction = ease1;
            Storyboard.SetTarget(fadeInAnimation2, newupdatepaneltransform);
            Storyboard.SetTargetProperty(fadeInAnimation2, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation2);
            DoubleAnimation fadeInAnimation3 = new DoubleAnimation();
            fadeInAnimation3.From = -500;
            fadeInAnimation3.To = 0;
            fadeInAnimation3.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            fadeInAnimation3.EasingFunction = ease1;
            Storyboard.SetTarget(fadeInAnimation3, queuedupdatespaneltransform);
            Storyboard.SetTargetProperty(fadeInAnimation3, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation3);
            DoubleAnimation fadeInAnimation4 = new DoubleAnimation();
            fadeInAnimation4.From = -500;
            fadeInAnimation4.To = 0;
            fadeInAnimation4.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            fadeInAnimation4.EasingFunction = ease1;
            Storyboard.SetTarget(fadeInAnimation4, morepaneltransform);
            Storyboard.SetTargetProperty(fadeInAnimation4, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation4);
            sb.Begin();

        }

        public Storyboard clickAnimation(TranslateTransform element1, TranslateTransform element2, TranslateTransform element3, TranslateTransform element4)
        {


            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
            NavigationService.Navigated += NavigationService_Navigated;
            queuedupdatespanel.Tap -= queuedupdatespanel_Tap;
            newupdatepanel.Tap -= newupdatepanel_Tap;
            newfeedpanel.Tap -= newfeedpanel_Tap;
            morepanel.Tap -= morepanel_Tap;
            Storyboard sb = new Storyboard();
            //sb.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimation fadeInAnimation1 = new DoubleAnimation();
            BackEase ease1 = new BackEase();
            ease1.EasingMode = EasingMode.EaseInOut;
            ease1.Amplitude = 1.2;

            fadeInAnimation1.From = 0;
            fadeInAnimation1.To = -500;
            fadeInAnimation1.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            fadeInAnimation1.EasingFunction = ease1;
            Storyboard.SetTarget(fadeInAnimation1, element1);
            Storyboard.SetTargetProperty(fadeInAnimation1, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation1);
            DoubleAnimation fadeInAnimation2 = new DoubleAnimation();
            fadeInAnimation2.From = 0;
            fadeInAnimation2.To = 500;
            fadeInAnimation2.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            fadeInAnimation2.EasingFunction = ease1;
            Storyboard.SetTarget(fadeInAnimation2, element2);
            Storyboard.SetTargetProperty(fadeInAnimation2, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation2);
            DoubleAnimation fadeInAnimation3 = new DoubleAnimation();
            fadeInAnimation3.From = 0;
            fadeInAnimation3.To = 500;
            fadeInAnimation3.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            fadeInAnimation3.EasingFunction = ease1;
            Storyboard.SetTarget(fadeInAnimation3, element3);
            Storyboard.SetTargetProperty(fadeInAnimation3, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation3);
            DoubleAnimation fadeInAnimation4 = new DoubleAnimation();
            fadeInAnimation4.From = 0;
            fadeInAnimation4.To = 500;
            fadeInAnimation4.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            fadeInAnimation4.EasingFunction = ease1;
            Storyboard.SetTarget(fadeInAnimation4, element4);
            Storyboard.SetTargetProperty(fadeInAnimation4, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation4);
            return sb;
        }

        private void NewFeedsSB_Completed(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/FeedsBrowser.xaml", UriKind.Relative));
        }

        public void verticalTileAnimation1()
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation fadeInAnimation1 = new DoubleAnimation();
            //fadeInAnimation1.BeginTime = Timeline.BeginTimeProperty;
            fadeInAnimation1.From = 0;
            fadeInAnimation1.To = 100;
            fadeInAnimation1.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation1, newfeedimagetransform1);
            Storyboard.SetTargetProperty(fadeInAnimation1, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation1);
            DoubleAnimation fadeInAnimation2 = new DoubleAnimation();
            fadeInAnimation2.From = -200;
            fadeInAnimation2.To = -100;
            fadeInAnimation2.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation2, newfeedimagetransform2);
            Storyboard.SetTargetProperty(fadeInAnimation2, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation2);

            DoubleAnimation fadeInAnimation3 = new DoubleAnimation();
            //fadeInAnimation1.BeginTime = Timeline.BeginTimeProperty;
            fadeInAnimation3.From = 0;
            fadeInAnimation3.To = 100;
            fadeInAnimation3.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation3, newupdateimagetransform1);
            Storyboard.SetTargetProperty(fadeInAnimation3, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation3);
            DoubleAnimation fadeInAnimation4 = new DoubleAnimation();
            fadeInAnimation4.From = -200;
            fadeInAnimation4.To = -100;
            fadeInAnimation4.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation4, newupdateimagetransform2);
            Storyboard.SetTargetProperty(fadeInAnimation4, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation4);


            DoubleAnimation fadeInAnimation5 = new DoubleAnimation();
            //fadeInAnimation1.BeginTime = Timeline.BeginTimeProperty;
            fadeInAnimation5.From = 0;
            fadeInAnimation5.To = 100;
            fadeInAnimation5.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation5, queuedupdatesimagetransform1);
            Storyboard.SetTargetProperty(fadeInAnimation5, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation5);
            DoubleAnimation fadeInAnimation6 = new DoubleAnimation();
            fadeInAnimation6.From = -200;
            fadeInAnimation6.To = -100;
            fadeInAnimation6.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation6, queuedupdatesimagetransform2);
            Storyboard.SetTargetProperty(fadeInAnimation6, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation6);


            DoubleAnimation fadeInAnimation7 = new DoubleAnimation();
            //fadeInAnimation1.BeginTime = Timeline.BeginTimeProperty;
            fadeInAnimation7.From = 0;
            fadeInAnimation7.To = 100;
            fadeInAnimation7.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation7, moreimagetransform1);
            Storyboard.SetTargetProperty(fadeInAnimation7, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation7);
            DoubleAnimation fadeInAnimation8 = new DoubleAnimation();
            fadeInAnimation8.From = -200;
            fadeInAnimation8.To = -100;
            fadeInAnimation8.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation8, moreimagetransform2);
            Storyboard.SetTargetProperty(fadeInAnimation8, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation8);


            sb.Completed += VerticalAnimation1Sb_Completed;
            sb.Begin();
        }

        public void verticalTileAnimation2()
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation fadeInAnimation1 = new DoubleAnimation();
            //fadeInAnimation1.BeginTime = Timeline.BeginTimeProperty;
            fadeInAnimation1.From = -100;
            fadeInAnimation1.To = 0;
            fadeInAnimation1.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation1, newfeedimagetransform2);
            Storyboard.SetTargetProperty(fadeInAnimation1, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation1);
            DoubleAnimation fadeInAnimation2 = new DoubleAnimation();
            fadeInAnimation2.From = -100;
            fadeInAnimation2.To = 0;
            fadeInAnimation2.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation2, newfeedimagetransform1);
            Storyboard.SetTargetProperty(fadeInAnimation2, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation2);

            DoubleAnimation fadeInAnimation3 = new DoubleAnimation();
            //fadeInAnimation1.BeginTime = Timeline.BeginTimeProperty;
            fadeInAnimation3.From = -100;
            fadeInAnimation3.To = 0;
            fadeInAnimation3.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation3, newupdateimagetransform2);
            Storyboard.SetTargetProperty(fadeInAnimation3, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation3);
            DoubleAnimation fadeInAnimation4 = new DoubleAnimation();
            fadeInAnimation4.From = -100;
            fadeInAnimation4.To = 0;
            fadeInAnimation4.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation4, newupdateimagetransform1);
            Storyboard.SetTargetProperty(fadeInAnimation4, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation4);


            DoubleAnimation fadeInAnimation5 = new DoubleAnimation();
            //fadeInAnimation1.BeginTime = Timeline.BeginTimeProperty;
            fadeInAnimation5.From = -100;
            fadeInAnimation5.To = 0;
            fadeInAnimation5.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation5, queuedupdatesimagetransform2);
            Storyboard.SetTargetProperty(fadeInAnimation5, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation5);
            DoubleAnimation fadeInAnimation6 = new DoubleAnimation();
            fadeInAnimation6.From = -100;
            fadeInAnimation6.To = 0;
            fadeInAnimation6.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation6, queuedupdatesimagetransform1);
            Storyboard.SetTargetProperty(fadeInAnimation6, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation6);


            DoubleAnimation fadeInAnimation7 = new DoubleAnimation();
            //fadeInAnimation1.BeginTime = Timeline.BeginTimeProperty;
            fadeInAnimation7.From = -100;
            fadeInAnimation7.To = 0;
            fadeInAnimation7.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation7, moreimagetransform2);
            Storyboard.SetTargetProperty(fadeInAnimation7, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation7);
            DoubleAnimation fadeInAnimation8 = new DoubleAnimation();
            fadeInAnimation8.From = -100;
            fadeInAnimation8.To = 0;
            fadeInAnimation8.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation8, moreimagetransform1);
            Storyboard.SetTargetProperty(fadeInAnimation8, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation8);

            sb.Completed += VerticalAnimation2Sb_Completed;
            sb.Begin();
        }

        private void VerticalAnimation2Sb_Completed(object s, EventArgs e)
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };

            timer.Tick += (sender, args) => { timer.Stop(); verticalTileAnimation1(); };

            timer.Start();
        }

        private void VerticalAnimation1Sb_Completed(object s, EventArgs e)
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };

            timer.Tick += (sender, args) => { timer.Stop(); verticalTileAnimation2(); };

            timer.Start();
        }

        private void queuedupdatespanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Storyboard sb = clickAnimation(queuedupdatespaneltransform, newfeedpaneltransform, newupdatepaneltransform, morepaneltransform);
            sb.Completed += QueueUpdatesSB_Completed;
            sb.Begin();
        }

        private void QueueUpdatesSB_Completed(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/QueuedUpdates.xaml", UriKind.Relative));
        }


        private void newfeedpanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Storyboard sb = clickAnimation(newfeedpaneltransform, queuedupdatespaneltransform, newupdatepaneltransform, morepaneltransform);
            sb.Completed += NewFeedsSB_Completed;
            sb.Begin();
        }

        private void newupdatepanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Storyboard sb = clickAnimation(newupdatepaneltransform, newfeedpaneltransform, queuedupdatespaneltransform, morepaneltransform);
            sb.Completed += NewUpdateSB_Completed;
            sb.Begin();
        }

        private void NewUpdateSB_Completed(object sender, EventArgs e)
        { 
            NavigationService.Navigate(new Uri("/Page0.xaml", UriKind.Relative));
        }

        private void morepanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Storyboard sb = clickAnimation(morepaneltransform, newfeedpaneltransform, queuedupdatespaneltransform, newupdatepaneltransform);
            sb.Completed += MoreSB_Completed;
            sb.Begin();
        }

        private void MoreSB_Completed(object sender, EventArgs e)
        {
            try
            {
                ScheduledActionService.Remove("PeriodicTaskDemo");
                Dispatcher.BeginInvoke(() => MessageBox.Show("Turn off the background agent successfully"));
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("doesn't exist"))
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show("Since then the background agent success is not running"));
                }
            }
            catch (SchedulerServiceException)
            {

            }
        }
        */
        public Storyboard clickAnimate()
        {
            while (NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();
            NavigationService.Navigated += NavigationService_Navigated;

            newStatus1.Tap-=StackPanel_Tap_1;
            newStatus2.Tap-=StackPanel_Tap_1;
            liveFeeds1.Tap-=StackPanel_Tap_2;
            liveFeeds2.Tap-=StackPanel_Tap_2;
            aboutUs.Tap-=StackPanel_Tap_3;
            help.Tap-=StackPanel_Tap_4;
            queued.Tap -= StackPanel_Tap_5;

            int animationDuration = 600;

            Storyboard sb = new Storyboard();
            //sb.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimation fadeInAnimation1 = new DoubleAnimation();
            fadeInAnimation1.From = 0;
            fadeInAnimation1.To = -800;
            fadeInAnimation1.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration));
            Storyboard.SetTarget(fadeInAnimation1, newStatus1Transform);
            Storyboard.SetTargetProperty(fadeInAnimation1, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation1);


            DoubleAnimation fadeInAnimation2 = new DoubleAnimation();
            fadeInAnimation2.From = 0;
            fadeInAnimation2.To = -800;
            fadeInAnimation2.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration));
            Storyboard.SetTarget(fadeInAnimation2, newStatus2Transform);
            Storyboard.SetTargetProperty(fadeInAnimation2, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation2);


            DoubleAnimation fadeInAnimation3 = new DoubleAnimation();
            fadeInAnimation3.From = 0;
            fadeInAnimation3.To = 300;
            fadeInAnimation3.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration));
            Storyboard.SetTarget(fadeInAnimation3, liveFeeds1Transform);
            Storyboard.SetTargetProperty(fadeInAnimation3, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation3);


            DoubleAnimation fadeInAnimation4 = new DoubleAnimation();
            fadeInAnimation4.From = 0;
            fadeInAnimation4.To = 300;
            fadeInAnimation4.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration));
            Storyboard.SetTarget(fadeInAnimation4, liveFeeds2Transform);
            Storyboard.SetTargetProperty(fadeInAnimation4, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation4);


            DoubleAnimation fadeInAnimation5 = new DoubleAnimation();
            fadeInAnimation5.From = 0;
            fadeInAnimation5.To = 500;
            fadeInAnimation5.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration));
            Storyboard.SetTarget(fadeInAnimation5, aboutUsTransform);
            Storyboard.SetTargetProperty(fadeInAnimation5, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation5);


            DoubleAnimation fadeInAnimation6 = new DoubleAnimation();
            fadeInAnimation6.From = 0;
            fadeInAnimation6.To = -300;
            fadeInAnimation6.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration));
            Storyboard.SetTarget(fadeInAnimation6, helpTransform);
            Storyboard.SetTargetProperty(fadeInAnimation6, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation6);


            DoubleAnimation fadeInAnimation7 = new DoubleAnimation();
            fadeInAnimation7.From = 0;
            fadeInAnimation7.To = 300;
            fadeInAnimation7.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration));
            Storyboard.SetTarget(fadeInAnimation7, queuedTransform);
            Storyboard.SetTargetProperty(fadeInAnimation7, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation7);

            return sb;
        }

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            newStatus1.Tap += StackPanel_Tap_1;
            newStatus2.Tap += StackPanel_Tap_1;
            liveFeeds1.Tap += StackPanel_Tap_2;
            liveFeeds2.Tap += StackPanel_Tap_2;
            aboutUs.Tap += StackPanel_Tap_3;
            help.Tap += StackPanel_Tap_4;
            queued.Tap += StackPanel_Tap_5;

            Storyboard sb = new Storyboard();
            //sb.RepeatBehavior = RepeatBehavior.Forever;
            DoubleAnimation fadeInAnimation1 = new DoubleAnimation();
            fadeInAnimation1.From = -800;
            fadeInAnimation1.To = 0;
            fadeInAnimation1.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation1, newStatus1Transform);
            Storyboard.SetTargetProperty(fadeInAnimation1, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation1);


            DoubleAnimation fadeInAnimation2 = new DoubleAnimation();
            fadeInAnimation2.From = -800;
            fadeInAnimation2.To = 0;
            fadeInAnimation2.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation2, newStatus2Transform);
            Storyboard.SetTargetProperty(fadeInAnimation2, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation2);


            DoubleAnimation fadeInAnimation3 = new DoubleAnimation();
            fadeInAnimation3.From = 300;
            fadeInAnimation3.To = 0;
            fadeInAnimation3.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation3, liveFeeds1Transform);
            Storyboard.SetTargetProperty(fadeInAnimation3, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation3);


            DoubleAnimation fadeInAnimation4 = new DoubleAnimation();
            fadeInAnimation4.From = 300;
            fadeInAnimation4.To = 0;
            fadeInAnimation4.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation4, liveFeeds2Transform);
            Storyboard.SetTargetProperty(fadeInAnimation4, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation4);


            DoubleAnimation fadeInAnimation5 = new DoubleAnimation();
            fadeInAnimation5.From = 500;
            fadeInAnimation5.To = 0;
            fadeInAnimation5.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation5, aboutUsTransform);
            Storyboard.SetTargetProperty(fadeInAnimation5, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation5);


            DoubleAnimation fadeInAnimation6 = new DoubleAnimation();
            fadeInAnimation6.From = -300;
            fadeInAnimation6.To = 0;
            fadeInAnimation6.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation6, helpTransform);
            Storyboard.SetTargetProperty(fadeInAnimation6, new PropertyPath("X"));
            sb.Children.Add(fadeInAnimation6);


            DoubleAnimation fadeInAnimation7 = new DoubleAnimation();
            fadeInAnimation7.From = 300;
            fadeInAnimation7.To = 0;
            fadeInAnimation7.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            Storyboard.SetTarget(fadeInAnimation7, queuedTransform);
            Storyboard.SetTargetProperty(fadeInAnimation7, new PropertyPath("Y"));
            sb.Children.Add(fadeInAnimation7);
            sb.Begin();
        }


        private void StackPanel_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Storyboard sb = clickAnimate();
            sb.Completed += sb_newUpdate_Completed;
            sb.Begin();
        }

        private void sb_newUpdate_Completed(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/pivot.xaml", UriKind.Relative));
        }

        private void StackPanel_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Storyboard sb = clickAnimate();
            sb.Completed += sb_liveFeeds_Completed;
            sb.Begin();
        }

        private void sb_liveFeeds_Completed(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/FeedsBrowser.xaml", UriKind.Relative));
        }

        private void StackPanel_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Storyboard sb = clickAnimate();
            sb.Completed += sb_aboutUs_Completed;
            sb.Begin();
        }

        private void sb_aboutUs_Completed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
            //StopPeriodicTask();
        }

        private void StackPanel_Tap_4(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Storyboard sb = clickAnimate();
            sb.Completed += sb_help_Completed;
            sb.Begin();
        }

        private void sb_help_Completed(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
            //StopPeriodicTask();
            //throw new NotImplementedException();
        }

        private void StackPanel_Tap_5(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Storyboard sb = clickAnimate();
            sb.Completed += sb_queued_Completed;
            sb.Begin();
        }

        private void sb_queued_Completed(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/QueuedUpdates.xaml", UriKind.Relative));
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


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}