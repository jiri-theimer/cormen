using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BL
{
    public class RunningApp2
    {
        
        private  string _ConString;
        private  string _UploadFolder;
        private  string _TempFolder;
        private  string _LogFolder;
        public RunningApp2(string strConString)
        {
            _ConString = strConString;
        }
        public  void SetConnectString(string strConString)
        {
            _ConString = strConString;
        }
        public  void SetFolders(string strUpload, string strTemp, string strLog)
        {
            _UploadFolder = strUpload;
            _TempFolder = strTemp;
            _LogFolder = strLog;

        }

        public DateTime InstanceTimestamp = DateTime.Now;

        public string Pokus { get; set; }

        public string ConnectString
        {
            get
            {
                return _ConString;
            }
        }
        public string UploadFolder { get { return _UploadFolder; } }
        public string TempFolder { get { return _TempFolder; } }
        public string LogFolder { get { return _LogFolder; } }

        
       


    }
}
