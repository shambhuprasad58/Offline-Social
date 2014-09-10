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
    public partial class Help : PhoneApplicationPage
    {
        public Help()
        {
            InitializeComponent();
            HelpBox.Text = "1. New Status \r\n To put a new status , click on new status,  \r\n Next , Choose social networks, you need to log in first if not already. \r\n Click next. \r\n In the next screen you need to write Status you want to be posted , \r\n Come to next screen, Choose the time frame you want status to be posted, Default value is present time. \r\n Press next. \r\n Now if Time frame of status permits and Network is available , It will post status.  \r\n Else it will provide two option as displayed  \r\n whether you want status to be scheduled via our server or phone itself. \r\n Choose the option you want and close the app. Status will be post accordingly. \r\n \r\n 2. Feeds \r\n To see live feeds in brower click on feeds tab and slide left or right to choose social network \r\n  \r\n 3. Queued updates \r\n To edit or delete already queued update , visit queued updates. click on specific status from list & choose option accordingly.";
        }
    }
}