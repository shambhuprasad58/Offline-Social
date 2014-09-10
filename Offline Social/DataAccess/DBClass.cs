using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    [Table]
    public class statusUpdateItem : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property and database column.
        private int _toDoItemId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int statusUpdateItemId
        {
            get
            {
                return _toDoItemId;
            }
            set
            {
                if (_toDoItemId != value)
                {
                    NotifyPropertyChanging("statusUpdateItemId");
                    _toDoItemId = value;
                    NotifyPropertyChanged("statusUpdateItemId");
                }
            }
        }

        // Define item name: private field, public property and database column.
        private string _status;

        [Column]
        public string status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    NotifyPropertyChanging("status");
                    _status = value;
                    NotifyPropertyChanged("status");
                }
            }
        }

        // Define completion value: private field, public property and database column.
        private int _selected;

        [Column]
        public int selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected != value)
                {
                    NotifyPropertyChanging("_selected");
                    _selected = value;
                    NotifyPropertyChanged("_selected");
                }
            }
        }


        private DateTime _startTime;

        [Column]
        public DateTime startTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                if (_startTime != value)
                {
                    NotifyPropertyChanging("_startTime");
                    _startTime = value;
                    NotifyPropertyChanged("_startTime");
                }
            }
        }


        private DateTime _endTime;

        [Column]
        public DateTime endTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                if (_endTime != value)
                {
                    NotifyPropertyChanging("_endTime");
                    _endTime = value;
                    NotifyPropertyChanged("_endTime");
                }
            }
        }


        private string _picFolder;

        [Column]
        public string picFolder
        {
            get
            {
                return _picFolder;
            }
            set
            {
                if (_picFolder != value)
                {
                    NotifyPropertyChanging("_picFolder");
                    _picFolder = value;
                    NotifyPropertyChanged("_picFolder");
                }
            }
        }

        private int _uploadStatus;

        [Column]
        public int uploadStatus
        {
            get
            {
                return _uploadStatus;
            }
            set
            {
                if (_uploadStatus != value)
                {
                    NotifyPropertyChanging("_uploadStatus");
                    _uploadStatus = value;
                    NotifyPropertyChanged("_uploadStatus");
                }
            }
        }

        private string _fbAccessToken;

        [Column]
        public string fbAccessToken
        {
            get
            {
                return _fbAccessToken;
            }
            set
            {
                if (_fbAccessToken != value)
                {
                    NotifyPropertyChanging("_fbAccessToken");
                    _fbAccessToken = value;
                    NotifyPropertyChanged("_fbAccessToken");
                }
            }
        }

        private string _twAccessToken;

        [Column]
        public string twAccessToken
        {
            get
            {
                return _twAccessToken;
            }
            set
            {
                if (_twAccessToken != value)
                {
                    NotifyPropertyChanging("_twAccessToken");
                    _twAccessToken = value;
                    NotifyPropertyChanged("_twAccessToken");
                }
            }
        }

        private string _twAccessTokenSecret;

        [Column]
        public string twAccessTokenSecret
        {
            get
            {
                return _twAccessTokenSecret;
            }
            set
            {
                if (_twAccessTokenSecret != value)
                {
                    NotifyPropertyChanging("_twAccessTokenSecret");
                    _twAccessTokenSecret = value;
                    NotifyPropertyChanged("_twAccessTokenSecret");
                }
            }
        }

        private string _ldAccessToken;

        [Column]
        public string ldAccessToken
        {
            get
            {
                return _ldAccessToken;
            }
            set
            {
                if (_ldAccessToken != value)
                {
                    NotifyPropertyChanging("_ldAccessToken");
                    _ldAccessToken = value;
                    NotifyPropertyChanged("_ldAccessToken");
                }
            }
        }



        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify the page that a data context property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify the data context that a data context property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }


    public class StatusUpdateDataContext : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Data Source=isostore:/statusUpdate.sdf";

        // Pass the connection string to the base class.
        public StatusUpdateDataContext(string connectionString)
            : base(connectionString)
        { }

        // Specify a single table for the to-do items.
        public Table<statusUpdateItem> StatusUpdateItems;
    }
}
