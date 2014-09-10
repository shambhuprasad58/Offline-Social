using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using Microsoft.Phone.BackgroundTransfer;
using System.Linq;
using System.IO.IsolatedStorage;
using System.IO;
using System.Collections.ObjectModel;
using DataAccess;
using System.Text;
using System.Windows.Threading;
using Microsoft.Phone.Info;

namespace Offline_Social
{
    public class UploadService
    {
        private const string ServiceUploadLocationURL = "http://10.109.61.28/PhotoUploaderService/UploaderService/File/upload.php"; 
        private const string TransfersFiles = @"\shared\transfers";
        private string filename = Guid.NewGuid().ToString();
        public statusUpdateItem item;
        public Dispatcher dispatcher;


        private bool _wifiOnly = false;
        private bool _externalPowerOnly = false;

        private bool _waitingForExternalPower;
        private bool _waitingForExternalPowerDueToBatterySaverMode;
        private bool _waitingForNonVoiceBlockingNetwork;
        private bool _waitingForWiFi;        

        public IEnumerable<BackgroundTransferRequest> TransferRequests { get; private set; }

        public UploadService()
        {
            EnsureTransfersFolder();
        }

        private void EnsureTransfersFolder()
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStore.DirectoryExists(TransfersFiles))
                {
                    isoStore.CreateDirectory(TransfersFiles);
                }
            }
        }

        public void RefreshTransfers()
        {
            _waitingForExternalPower = false;
            _waitingForExternalPowerDueToBatterySaverMode = false;
            _waitingForNonVoiceBlockingNetwork = false;
            _waitingForWiFi = false;

            InitialTansferStatusCheck();
            UpdateRequestsList();
        }
        
        private void InitialTansferStatusCheck()
        {
            UpdateRequestsList();

            foreach (var transfer in TransferRequests)
            {
                transfer.TransferStatusChanged += OnTransferStatusChanged;
                transfer.TransferProgressChanged += OnTransferProgressChanged;
                ProcessTransfer(transfer);
            }

            if (_waitingForExternalPower)
            {
                dispatcher.BeginInvoke(() =>  MessageBox.Show("Connect your device to external power to continue uploading files."));
            }
            if (_waitingForExternalPowerDueToBatterySaverMode)
            {
                dispatcher.BeginInvoke(() =>  MessageBox.Show("Connect your device to external power or disable Battery Saver Mode to continue uploading files."));
            }
            if (_waitingForNonVoiceBlockingNetwork)
            {
                dispatcher.BeginInvoke(() =>  MessageBox.Show("Connect your device to network with simultaneous voice and data to continue uploading files."));
            }
            if (_waitingForWiFi)
            {
                dispatcher.BeginInvoke(() =>  MessageBox.Show("Connect your device to a WiFi network to continue uploading files."));
            }
        }

        private void OnTransferStatusChanged(object sender, BackgroundTransferEventArgs e)
        {
            ProcessTransfer(e.Request);
            UpdateRequestsList();
        }

        private void ProcessTransfer(BackgroundTransferRequest transfer)
        {
            switch (transfer.TransferStatus)
            {
                case TransferStatus.Completed:

                    if (transfer.StatusCode == 200 || transfer.StatusCode == 206)
                    {
                        var requestId = transfer.RequestId;
                        RemoveTransferRequest(requestId);
                        
                        //PictureRepository.Instance.RemovePicture(requestId);

                        using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            if (iso.FileExists(transfer.UploadLocation.OriginalString))
                                iso.DeleteFile(transfer.UploadLocation.OriginalString);
                        }
                        dispatcher.BeginInvoke(() => MessageBox.Show("Sent request successful."));

                    }
                    else
                    {
                        RemoveTransferRequest(transfer.RequestId);
                        dispatcher.BeginInvoke(() => MessageBox.Show("Sent request failed."));
                        if (transfer.TransferError != null)
                        {
                        }
                    }
                    break;

                case TransferStatus.WaitingForExternalPower:
                    _waitingForExternalPower = true;
                    break;

                case TransferStatus.WaitingForExternalPowerDueToBatterySaverMode:
                    _waitingForExternalPowerDueToBatterySaverMode = true;
                    break;

                case TransferStatus.WaitingForNonVoiceBlockingNetwork:
                    _waitingForNonVoiceBlockingNetwork = true;
                    break;

                case TransferStatus.WaitingForWiFi:
                    _waitingForWiFi = true;
                    break;
            }
        }

        private void OnTransferProgressChanged(object sender, BackgroundTransferEventArgs e)
        {
            UpdateRequestsList();
        }

        private void UpdateRequestsList()
        {
            if (TransferRequests != null)
            {
                foreach (var request in TransferRequests)
                {
                    request.Dispose();
                }
            }
            TransferRequests = BackgroundTransferService.Requests;

            //foreach (var tr in TransferRequests) ;
                //PictureRepository.Instance.UpdatePicture(tr.RequestId, tr.TransferStatus, tr.BytesSent, tr.TotalBytesToSend);            
        }

        private void RemoveTransferRequest(string requestId)
        {
            var transferToRemove = BackgroundTransferService.Find(requestId);

            if (transferToRemove == null) return;

            try
            {
                BackgroundTransferService.Remove(transferToRemove);
            }
            catch (Exception)
            {

            }
        }
        /*
        public void CancelUpload(Picture picture)
        {
            //RemoveTransferRequest(picture.RequestId);

            //PictureRepository.Instance.SetPictureStatus(picture, FileStatus.New, picture.RequestId);

            //UpdateRequestsList();
        }*/





        public void StartUpload()
        {
            if (BackgroundTransferService.Requests.Count() < 10)
            {
                createFile();
                AddBackgroundTransfer();
            }
            else
            {
                dispatcher.BeginInvoke(() =>  MessageBox.Show("The maximum number of background file transfer requests for this application has been exceeded."));
            }          
        }

        public void createFile()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = file.CreateFile(Path.Combine(TransfersFiles, filename));
            UTF8Encoding encoder = new UTF8Encoding();
            stream.Write(encoder.GetBytes("folder " + Windows.Phone.System.Analytics.HostInformation.PublisherHostId + "\n"), 0, encoder.GetByteCount("folder " + Windows.Phone.System.Analytics.HostInformation.PublisherHostId + "\n"));
            stream.Write(encoder.GetBytes("guid " + filename+"\n"),0,encoder.GetByteCount("guid " + filename+"\n"));
            stream.Write(encoder.GetBytes("facebookaccesstoken " + item.fbAccessToken + "\n"), 0, encoder.GetByteCount("facebookaccesstoken " + item.fbAccessToken + "\n"));
            stream.Write(encoder.GetBytes("twitteraccesstoken " + item.twAccessToken + "\n"), 0, encoder.GetByteCount("twitteraccesstoken " + item.twAccessToken + "\n"));
            stream.Write(encoder.GetBytes("twittertokensecret " + item.twAccessTokenSecret + "\n"), 0, encoder.GetByteCount("twittertokensecret " + item.twAccessTokenSecret + "\n"));
            stream.Write(encoder.GetBytes("linkedinaccesstoken " + item.ldAccessToken + "\n"), 0, encoder.GetByteCount("linkedinaccesstoken " + item.ldAccessToken + "\n"));
            stream.Write(encoder.GetBytes("starttime " + item.startTime + "\n"), 0, encoder.GetByteCount("starttime " + item.startTime + "\n"));
            stream.Write(encoder.GetBytes("endtime " + item.endTime + "\n"), 0, encoder.GetByteCount("endtime " + item.endTime+ "\n"));
            stream.Write(encoder.GetBytes("selected " + item.selected + "\n"), 0, encoder.GetByteCount("selected " + item.selected + "\n"));
            stream.Write(encoder.GetBytes("status " + item.status + "\n"), 0, encoder.GetByteCount("status " + item.status + "\n"));
            
            stream.Close();
        }
        /*
         * Guid jkd
status mystatus
facebookaccesstoken
twitteraccesstoken
twittertokensecret
linkedinaccesstoken
starttime
endtime
selected
         */

        private void AddBackgroundTransfer()
        {
            //IsolatedStorageFileExtensions.SavePicture(Path.Combine(TransfersFiles, picture.FileName), picture.Data);

            var transferRequest = new BackgroundTransferRequest(new Uri(ServiceUploadLocationURL, UriKind.Absolute));

            //if (!_wifiOnly)
            //{
            //    transferRequest.TransferPreferences = TransferPreferences.AllowCellular;
            //}
            //if (!_externalPowerOnly)
            //{
            //    transferRequest.TransferPreferences = TransferPreferences.AllowBattery;
            //}
            //if (!_wifiOnly && !_externalPowerOnly)
            //{
            //    transferRequest.TransferPreferences = TransferPreferences.AllowCellularAndBattery;
            //}
            transferRequest.TransferPreferences = TransferPreferences.AllowCellularAndBattery;
            transferRequest.Method = "POST";
            transferRequest.UploadLocation = new Uri(TransfersFiles + @"\" + filename, UriKind.Relative);
            transferRequest.TransferStatusChanged += OnTransferStatusChanged;
            transferRequest.TransferProgressChanged += OnTransferProgressChanged;

            try
            {                
                BackgroundTransferService.Add(transferRequest);
                dispatcher.BeginInvoke(() =>  MessageBox.Show("Added Background Agent for request"));
                //PictureRepository.Instance.SetPictureStatus(picture, FileStatus.Active, transferRequest.RequestId);
                //UpdateRequestsList();
            }
            catch (InvalidOperationException ex)
            {
                dispatcher.BeginInvoke(() =>  MessageBox.Show("Unable to add background transfer request. " + ex.Message));
            }
            catch (Exception ex)
            {
                dispatcher.BeginInvoke(() =>  MessageBox.Show("Unable to add background transfer request." + ex.Message));
            }
        }

        public static readonly UploadService Instance = new UploadService();


        
    }
    public class ServerFolderHandler
    {
        private const string ServiceUploadLocationURL = "http://10.3.32.139/PhotoUploaderService/UploaderService/File/createFolder.php";
        private const string TransfersFiles = @"\shared\transfers";
        private string filename = "ServerRequest.txt";
        public Dispatcher dispatcher;


        private bool _wifiOnly = false;
        private bool _externalPowerOnly = false;

        private bool _waitingForExternalPower;
        private bool _waitingForExternalPowerDueToBatterySaverMode;
        private bool _waitingForNonVoiceBlockingNetwork;
        private bool _waitingForWiFi;
        public IEnumerable<BackgroundTransferRequest> TransferRequests { get; private set; }


        public void StartUpload()
        {
            if (BackgroundTransferService.Requests.Count() < 10)
            {
                createFile();
                createServerFolderRequest();
            }
            else
            {
                dispatcher.BeginInvoke(() => MessageBox.Show("The maximum number of background file transfer requests for this application has been exceeded."));
            }
        }

        public void createFile()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = file.CreateFile(Path.Combine(TransfersFiles, filename));
            UTF8Encoding encoder = new UTF8Encoding();
            object Id = Windows.Phone.System.Analytics.HostInformation.PublisherHostId;
            MessageBox.Show("Id is "+Id);
            stream.Write(encoder.GetBytes(Id.ToString()), 0, encoder.GetByteCount(Id.ToString()));
            stream.Close();
        }

        public void createServerFolderRequest()
        {
            var transferRequest = new BackgroundTransferRequest(new Uri(ServiceUploadLocationURL, UriKind.Absolute));

            //if (!_wifiOnly)
            //{
            //    transferRequest.TransferPreferences = TransferPreferences.AllowCellular;
            //}
            //if (!_externalPowerOnly)
            //{
            //    transferRequest.TransferPreferences = TransferPreferences.AllowBattery;
            //}
            //if (!_wifiOnly && !_externalPowerOnly)
            //{
            //    transferRequest.TransferPreferences = TransferPreferences.AllowCellularAndBattery;
            //}
            transferRequest.TransferPreferences = TransferPreferences.AllowCellularAndBattery;
            transferRequest.Method = "POST";
            transferRequest.UploadLocation = new Uri(TransfersFiles + @"\" + filename, UriKind.Relative);
            transferRequest.TransferStatusChanged += OnTransferStatusChanged;
            transferRequest.TransferProgressChanged += OnTransferProgressChanged;

            try
            {
                BackgroundTransferService.Add(transferRequest);
                dispatcher.BeginInvoke(() => MessageBox.Show("Added Background Agent for server request"));
                //PictureRepository.Instance.SetPictureStatus(picture, FileStatus.Active, transferRequest.RequestId);
                //UpdateRequestsList();
            }
            catch (InvalidOperationException ex)
            {
                dispatcher.BeginInvoke(() => MessageBox.Show("Unable to add background transfer request. " + ex.Message));
            }
            catch (Exception ex)
            {
                dispatcher.BeginInvoke(() => MessageBox.Show("Unable to add background transfer request." + ex.Message));
            }
        }


        public void RefreshTransfers()
        {
            _waitingForExternalPower = false;
            _waitingForExternalPowerDueToBatterySaverMode = false;
            _waitingForNonVoiceBlockingNetwork = false;
            _waitingForWiFi = false;

            InitialTansferStatusCheck();
            UpdateRequestsList();
        }

        private void InitialTansferStatusCheck()
        {
            UpdateRequestsList();

            foreach (var transfer in TransferRequests)
            {
                transfer.TransferStatusChanged += OnTransferStatusChanged;
                transfer.TransferProgressChanged += OnTransferProgressChanged;
                ProcessTransfer(transfer);
            }

            if (_waitingForExternalPower)
            {
                dispatcher.BeginInvoke(() => MessageBox.Show("Connect your device to external power to continue uploading files."));
            }
            if (_waitingForExternalPowerDueToBatterySaverMode)
            {
                dispatcher.BeginInvoke(() => MessageBox.Show("Connect your device to external power or disable Battery Saver Mode to continue uploading files."));
            }
            if (_waitingForNonVoiceBlockingNetwork)
            {
                dispatcher.BeginInvoke(() => MessageBox.Show("Connect your device to network with simultaneous voice and data to continue uploading files."));
            }
            if (_waitingForWiFi)
            {
                dispatcher.BeginInvoke(() => MessageBox.Show("Connect your device to a WiFi network to continue uploading files."));
            }
        }

        private void OnTransferStatusChanged(object sender, BackgroundTransferEventArgs e)
        {
            if(e.Request.UploadLocation.OriginalString.ToLower().Equals((TransfersFiles + filename).ToLower()))
                ProcessTransfer(e.Request);
            UpdateRequestsList();
        }

        private void ProcessTransfer(BackgroundTransferRequest transfer)
        {
            switch (transfer.TransferStatus)
            {
                case TransferStatus.Completed:

                    if (transfer.StatusCode == 200 || transfer.StatusCode == 206)
                    {
                        var requestId = transfer.RequestId;
                        RemoveTransferRequest(requestId);

                        //PictureRepository.Instance.RemovePicture(requestId);

                        using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            if (iso.FileExists(transfer.UploadLocation.OriginalString))
                                iso.DeleteFile(transfer.UploadLocation.OriginalString);
                        }
                        serverFolder folderInfo = new serverFolder();
                        folderInfo.serverFolderUpdateInfo(2);
                        dispatcher.BeginInvoke(() => MessageBox.Show("Server folder create issue success"));
                    }
                    else
                    {
                        RemoveTransferRequest(transfer.RequestId);

                        if (transfer.TransferError != null)
                        {

                        }
                        dispatcher.BeginInvoke(() => MessageBox.Show("Server folder create issue failed"));
                    }
                    break;

                case TransferStatus.WaitingForExternalPower:
                    _waitingForExternalPower = true;
                    break;

                case TransferStatus.WaitingForExternalPowerDueToBatterySaverMode:
                    _waitingForExternalPowerDueToBatterySaverMode = true;
                    break;

                case TransferStatus.WaitingForNonVoiceBlockingNetwork:
                    _waitingForNonVoiceBlockingNetwork = true;
                    break;

                case TransferStatus.WaitingForWiFi:
                    _waitingForWiFi = true;
                    break;
            }
        }

        private void OnTransferProgressChanged(object sender, BackgroundTransferEventArgs e)
        {
            UpdateRequestsList();
        }

        private void UpdateRequestsList()
        {
            if (TransferRequests != null)
            {
                foreach (var request in TransferRequests)
                {
                    request.Dispose();
                }
            }
            TransferRequests = BackgroundTransferService.Requests;

            //foreach (var tr in TransferRequests) ;
            //PictureRepository.Instance.UpdatePicture(tr.RequestId, tr.TransferStatus, tr.BytesSent, tr.TotalBytesToSend);            
        }

        private void RemoveTransferRequest(string requestId)
        {
            var transferToRemove = BackgroundTransferService.Find(requestId);

            if (transferToRemove == null) return;

            try
            {
                BackgroundTransferService.Remove(transferToRemove);
            }
            catch (Exception)
            {

            }
        }

    }

}
