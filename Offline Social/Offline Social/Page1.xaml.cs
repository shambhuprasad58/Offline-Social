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
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {

            InitializeComponent();
            network_selected();
            scheduleTD();
        }

        private void scheduleTD()
        {
           startdate.Value= DateTime.Now.Date;
           expdate.Value = DateTime.Now.AddDays(1);
        }

        private void network_selected()
        {
            if (App.fb_selected)
                fbsmall.Visibility = Visibility.Visible;
            if (App.tw_selected)
                twsmall.Visibility = Visibility.Visible;
            if (App.ld_selected)
                ldsmall.Visibility = Visibility.Visible;

        }


        private void back_Tap(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void next_Tap(object sender, EventArgs e)
        {
            checkschedule();
            if (App.finalExpTime.CompareTo(App.finalStartTime) > 0 && App.finalExpTime.CompareTo(DateTime.Now) > 0)
                NavigationService.Navigate(new Uri("/Page2.xaml", UriKind.Relative));
            else
            {
                MessageBox.Show("Check your start and expiry time");
            }
        }


        public static string vstartdate;
        public static string vstarttime;
        public static string vexptime;
        public static string vexpdate;


        public void checkschedule()
        {
            DateTime startDay = (DateTime)startdate.Value;
            DateTime startTime = (DateTime)starttime.Value;
            App.finalStartTime = new DateTime(startDay.Year, startDay.Month, startDay.Day, startTime.Hour, startTime.Minute, 0, startTime.Kind);


            DateTime expDay = (DateTime)expdate.Value;
            DateTime expTime = (DateTime)exptime.Value;
            App.finalExpTime= new DateTime(expDay.Year, expDay.Month, expDay.Day, expTime.Hour, expTime.Minute, 0, expTime.Kind);
            
          
            //MessageBox.Show(App.finalStartTime.ToString());
            //MessageBox.Show(App.finalExpTime.ToString());
        }

    }
}