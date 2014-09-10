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
    public partial class Page2 : PhoneApplicationPage
    {
        public Page2()
        {
            InitializeComponent();
        }
        public static string status = "";
        public int statusLength = 0;
        private void back_Tap(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void next_Tap(object sender, EventArgs e)
        {
            if(statustextbox.Text == null || statustextbox.Text == "")
            {
                MessageBox.Show("Empty status not allowed");
                return;
            }
            NavigationService.Navigate(new Uri("/Page3.xaml", UriKind.Relative));
        }

        private void statusChanged(object sender, TextChangedEventArgs e)
        {
            string twthr = "";
            status = statustextbox.Text;
            if (App.tw_selected == true)
                twthr = "/140";
            chused.Text = status.Length + "" + twthr;
            if (status.Length > 140 && status.Length > statusLength && App.tw_selected)
                MessageBox.Show("Twitter max status legnth excedded");
            statusLength = status.Length;
        }

        private void statustextbox_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            if (statustextbox.SelectionStart == statustextbox.Text.Length)
            {
                scroller.ScrollToVerticalOffset(statustextbox.ActualHeight);

            }
        }
    }
}