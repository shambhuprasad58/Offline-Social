using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using DataAccess;
using System.Collections.ObjectModel;

namespace Offline_Social
{
    public partial class QueuedUpdates : PhoneApplicationPage, INotifyPropertyChanged
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



        public QueuedUpdates()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            statusUpdateDb = new StatusUpdateDataContext(StatusUpdateDataContext.DBConnectionString);

            // Data context and observable collection are children of the main page.
            this.DataContext = this;
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


//        {
        //    new Course() { CourseName = "Mobile Computing", Description = "Good course ! :)  " },
        //    new Course() { CourseName = "Wireless Communication", Description = "Good course ! :)  " },
        //    new Course() { CourseName = "Artificial Intelligent", Description = "Good course ! :)  " },
        //    new Course() { CourseName = "Database Design", Description = "Good course ! :)  " },
        //    new Course() { CourseName = "Marketing", Description = "Good course ! :)  " },
        //    new Course() { CourseName = "Communication Skills", Description = "Good course ! :)  " },
        //    new Course() { CourseName = "Presentation Skills", Description = "Good course ! :)  " },
        //    new Course() { CourseName = "Windows Phone Development", Description = "Good course ! :)  " },
//        };


        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ListCourses.DataContext = typeof(statusUpdateItem);
            ListCourses.ItemsSource = StatusUpdateItems;
        }



        private void DeleteButton_Click_1(object sender, EventArgs e)
        {
            statusUpdateItem item = (statusUpdateItem)ListCourses.SelectedItem;
            if (item == null)
                return;
            StatusUpdateItems.Remove(item);
            statusUpdateDb.StatusUpdateItems.DeleteOnSubmit(item);
            statusUpdateDb.SubmitChanges();
        }
    }
}