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
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
            InfoBox.Text = " Offline Social is a windows phone app made by students of IIT Kharagpur to make social life comfortable. \r\n \r\n This app has got two major functionality: \r\n First one is putting status update , when user is in unstable network zone. Suppose you wish to update a status, but stuck in situation where network is continuosly fluctuating. Instead of keep looking and wait for network, what you can do is , save status in offline social and this app will put the status update in social network from background as soon as internet comes. \r\n \r\n Another major function it has got is Scheduled status, If a user wants some update to be at an exact time, he can save it in the app, the app will upload it on the background server and will put the status update at the expected time. This way user need not have internet at the time he wants to put that status. Scheduling status without server but via phone is also available but in that case phone should have working internet at scheduled time. \r\n \r\n We have also come up with integrated browser , so that you can browse over feeds when network allows.";
        }
    }
}