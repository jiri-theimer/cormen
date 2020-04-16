using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public class RunningApp
    {
        private static RunningApp _instance = null;
        
        private static string _ConString;
        private static string _UploadFolder;
        private static string _TempFolder;
        public static void SetConnectString(string strConString)
        {
            _ConString = strConString;
        }
        public static void SetFolders(string strUpload,string strTemp)
        {
            _UploadFolder = strUpload;
            _TempFolder = strTemp;
        }

        public DateTime InstanceTimestamp = DateTime.Now;

        

        public string ConnectString { get
            {
                return _ConString;
            }
        }
        public string UploadFolder { get { return _UploadFolder; } }
        public string TempFolder { get { return _TempFolder; } }

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
