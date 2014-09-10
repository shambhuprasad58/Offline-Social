using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Offline_Social
{
    public partial class FeedsBrowser : PhoneApplicationPage
    {
        private Uri FBHomeUri = new Uri("http://m.facebook.com", UriKind.Absolute);
        private Uri TwitterHomeUri = new Uri("http://mobile.twitter.com", UriKind.Absolute);
        private Uri LinkedinHomeUri = new Uri("http://www.linkedin.com", UriKind.Absolute);

        public FeedsBrowser()
        {
            InitializeComponent();
            fbBrowser.Navigate(FBHomeUri);
            twitterBrowser.Navigate(TwitterHomeUri);
            linkedinBrowser.Navigate(LinkedinHomeUri);
        }

        private void HomeButtonClick(object sender, EventArgs e)
        {
            int index = pivot.SelectedIndex;
            if (index == 0)
            {
                fbBrowser.Navigate(FBHomeUri);
            }
            if (index == 1)
            {
                twitterBrowser.Navigate(TwitterHomeUri);
            }
            if (index == 2)
            {
                linkedinBrowser.Navigate(LinkedinHomeUri);
            }
        }

        private void BackButtonClick(object sender, EventArgs e)
        {
            int index = pivot.SelectedIndex;
            if (index == 0)
            {
                fbBrowser.GoBack();
            }
            if (index == 1)
            {
                twitterBrowser.GoBack();
            }
            if (index == 2)
            {
                linkedinBrowser.GoBack();
            }
        }

        private Uri currentFBUri = new Uri("http://m.facebook.com", UriKind.Absolute);
        private Uri currentTwitterUri = new Uri("http://mobile.twitter.com", UriKind.Absolute);
        private Uri currentLinkedinUri = new Uri("http://www.linkedin.com", UriKind.Absolute);

        private void RefreshButtonClick(object sender, EventArgs e)
        {
            int index = pivot.SelectedIndex;
            if (index == 0)
            {
                if (currentFBUri == null)
                    currentFBUri = new Uri("http://m.facebook.com", UriKind.Absolute);
                fbBrowser.Navigate(currentFBUri);
            }
            if (index == 1)
            {
                if (currentTwitterUri == null)
                    currentTwitterUri = new Uri("http://mobile.twitter.com", UriKind.Absolute);
                twitterBrowser.Navigate(currentTwitterUri);
            }
            if (index == 2)
            {
                if (currentLinkedinUri == null)
                    currentLinkedinUri = new Uri("http://www.linkedin.com", UriKind.Absolute);
                linkedinBrowser.Navigate(currentLinkedinUri);
            }
        }

        private void linkedinBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            currentLinkedinUri = e.Uri;
        }

        private void twitterBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            currentTwitterUri = e.Uri;
        }

        private void fbBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            currentFBUri = e.Uri;
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            int index = pivot.SelectedIndex;
            if (index == 0)
            {
                fbBrowser.GoForward();
            }
            if (index == 1)
            {
                twitterBrowser.GoForward();
            }
            if (index == 2)
            {
                linkedinBrowser.GoForward();
            }
        }
    }
}