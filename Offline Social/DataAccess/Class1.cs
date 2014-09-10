using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DataAccess
{

    public class ACCESSTOKENS
    {
        public string fbtoken = "";
        public string twtoken = "";
        public string twsecret = "";
        public string ldtoken = "";

        public ACCESSTOKENS()
        {
            readFromFile();
        }

        public void readFromFile()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream;
            if (!file.FileExists("userInfo.txt"))
            {
                stream = file.CreateFile("userInfo.txt");
                fbtoken = "";
                twsecret = "";
                twtoken = "";
                ldtoken = "";
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine("facebook" + fbtoken);
                writer.WriteLine("twitter" + twtoken);
                writer.WriteLine("twittersecret" + twsecret);
                writer.WriteLine("linkedin" + ldtoken);
                writer.Close();
            }
            else
            {
                stream = file.OpenFile("userInfo.txt", FileMode.Open);
                StreamReader reader = new StreamReader(stream);
                fbtoken = reader.ReadLine();
                fbtoken = fbtoken.Replace("facebook", "");
                twtoken = reader.ReadLine();
                twtoken = twtoken.Replace("twitter", "");
                twsecret = reader.ReadLine();
                twsecret = twsecret.Replace("twittersecret", "");
                ldtoken = reader.ReadLine();
                ldtoken = ldtoken.Replace("linkedin", "");
                reader.Close();
            }
            stream.Close();
        }

        public void writeToFile()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream;
            stream = file.OpenFile("userInfo.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine("facebook"+fbtoken);
            writer.WriteLine("twitter"+twtoken);
            writer.WriteLine("twittersecret" + twsecret);
            writer.WriteLine("linkedin"+ldtoken);
            writer.Close();
            stream.Close();
        }
    }

    public class UserName
    {
        public string fbname = "";
        public string twname = "";
        public string ldname = "";
        public const string filename = "username.txt";
        public UserName()
        {
            readFromFile();
        }

        public void readFromFile()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream;
            if (!file.FileExists(filename))
            {
                stream = file.CreateFile(filename);
                fbname = "";
                twname = "";
                ldname = "";
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine("facebookname" + fbname);
                writer.WriteLine("twittername" + twname);
                writer.WriteLine("linkedinname" + ldname);
                writer.Close();
            }
            else
            {
                stream = file.OpenFile(filename, FileMode.Open);
                StreamReader reader = new StreamReader(stream);
                fbname = reader.ReadLine();
                fbname = fbname.Replace("facebookname", "");
                twname = reader.ReadLine();
                twname = twname.Replace("twittername", "");
                ldname = reader.ReadLine();
                ldname = ldname.Replace("linkediname", "");
                reader.Close();
            }
            stream.Close();
        }

        public void writeToFile()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream;
            stream = file.OpenFile(filename, FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine("facebookname"+fbname);
            writer.WriteLine("twittername"+twname);
            writer.WriteLine("linkedinname"+ldname);
            writer.Close();
            stream.Close();
        }
    }




    public class serverFolder
    {
        public serverFolder()
        {
            updateInfo();
        }
        public const string serverInfoFile = "serverInfo.txt";
        public int status = 0;

        private void updateInfo()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream;
            if (!file.FileExists(serverInfoFile))
            {
                stream = file.CreateFile(serverInfoFile);
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine("0");
                status = 0;
                writer.Close();
                stream.Close();
            }
            else
            {
                stream = file.OpenFile(serverInfoFile, FileMode.Open);
                StreamReader reader = new StreamReader(stream);
                status = int.Parse(reader.ReadLine());
                reader.Close();
                stream.Close();
            }
        }

        public void checkAndUpdate()
        {
            if(status == 0)
                SendPost();
        }

        public void serverFolderUpdateInfo(int newstatus)
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream;
            stream = file.CreateFile(serverInfoFile);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(newstatus);
            status = newstatus;
            writer.Close();
            stream.Close();
        }


        void SendPost()
        {
            var url = "http://10.3.32.139//PhotoUploaderService/UploaderService/File/createFolder.php";

            // Create the web request object
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            // Start the request
            webRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), webRequest);
        }

        void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            // End the stream request operation
            Stream postStream = webRequest.EndGetRequestStream(asynchronousResult);

            // Create the post data
            // Demo POST data 
            string postData = "folder=" + Windows.Phone.System.Analytics.HostInformation.PublisherHostId;

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Add the post data to the web request
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();

            // Start the web request
            webRequest.BeginGetResponse(new AsyncCallback(GetResponseCallback), webRequest);
        }

        void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Started Response");
                });
                HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse response;

                // End the get response operation
                response = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamResponse);
                var Response = streamReader.ReadToEnd();
                streamResponse.Close();
                streamReader.Close();
                response.Close();
                status = 1;
                serverFolderUpdateInfo(1);
            }
            catch (WebException e)
            {
                status = 0;
            }
        }
    }

    public class FbFriends
    {

        public List<FbFriend> friends = new List<FbFriend>();
        public FbFriends()
        {
            readFromFile();
        }

        public void readFromFile()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream;
            if (!file.FileExists("FbFriendList.txt"))
            {
                stream = file.CreateFile("FbFriendList.txt");
                StreamWriter writer = new StreamWriter(stream);
                writer.Close();
            }
            else
            {
                stream = file.OpenFile("FbFriendList.txt", FileMode.Open);
                StreamReader reader = new StreamReader(stream);
                friends = new List<FbFriend>();
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] delimiter = { ":_," };
                    string id = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)[0];
                    string name = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries)[1];
                    friends.Add(new FbFriend(id, name));
                }
                reader.Close();
            }
            stream.Close();
        }

        public void writeToFile()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream;
            stream = file.OpenFile("FbFriendList.txt", FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);
            foreach(FbFriend friend in friends)
            {
                writer.WriteLine(friend.id + ":_," + friend.name);
            }
            writer.Close();
            stream.Close();
        }
    }

    public class FbFriend
    {
        public string id
        {
            get;
            set;
        }
        public string name
        {
            get;
            set;
        }
        public FbFriend(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
