﻿#pragma checksum "d:\visual studio 2012\Projects\Offline Social\Offline Social\Page1.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2A03D2A3C1F8B347A7749E93C5DD6904"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18046
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Offline_Social {
    
    
    public partial class Page1 : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal Microsoft.Phone.Controls.DatePicker startdate;
        
        internal Microsoft.Phone.Controls.TimePicker starttime;
        
        internal Microsoft.Phone.Controls.DatePicker expdate;
        
        internal Microsoft.Phone.Controls.TimePicker exptime;
        
        internal System.Windows.Controls.Image fbsmall;
        
        internal System.Windows.Controls.Image twsmall;
        
        internal System.Windows.Controls.Image ldsmall;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Offline%20Social;component/Page1.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.startdate = ((Microsoft.Phone.Controls.DatePicker)(this.FindName("startdate")));
            this.starttime = ((Microsoft.Phone.Controls.TimePicker)(this.FindName("starttime")));
            this.expdate = ((Microsoft.Phone.Controls.DatePicker)(this.FindName("expdate")));
            this.exptime = ((Microsoft.Phone.Controls.TimePicker)(this.FindName("exptime")));
            this.fbsmall = ((System.Windows.Controls.Image)(this.FindName("fbsmall")));
            this.twsmall = ((System.Windows.Controls.Image)(this.FindName("twsmall")));
            this.ldsmall = ((System.Windows.Controls.Image)(this.FindName("ldsmall")));
        }
    }
}
