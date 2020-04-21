using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BL
{
    public class RunningApp
    {
        private static RunningApp _instance = null;
        
        private static string _ConString;
        private static string _UploadFolder;
        private static string _TempFolder;
        private static string _LogFolder;
        public static void SetConnectString(string strConString)
        {
            _ConString = strConString;
        }
        public static void SetFolders(string strUpload,string strTemp,string strLog)
        {
            _UploadFolder = strUpload;
            _TempFolder = strTemp;
            _LogFolder = strLog;
            
        }

        public DateTime InstanceTimestamp = DateTime.Now;

        

        public string ConnectString { get
            {
                return _ConString;
            }
        }
        public string UploadFolder { get { return _UploadFolder; } }
        public string TempFolder { get { return _TempFolder; } }
        public string LogFolder { get { return _LogFolder; } }

        private RunningApp()
        {
            //záměrně privat konstruktor
           
        }
        public static void RefreshIntance()
        {            
            _instance = new RunningApp();
        }
        public static RunningApp Instance()
        {
            if (_instance == null)
            {
                _instance = new RunningApp();                
            }

            return _instance;
        }


    }
}
