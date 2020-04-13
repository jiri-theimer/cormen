using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public class BasRunningApp
    {
        private static BasRunningApp _instance = null;
        
        private static string _ConString;
        public static void SetConnectString(string strConString)
        {
            _ConString = strConString;
        }

        public DateTime InstanceTimestamp = DateTime.Now;

        

        public string ConnectString { get
            {
                return _ConString;
            }
        }

        private BasRunningApp()
        {
            //záměrně privat konstruktor
           
        }
        public static void RefreshIntance()
        {            
            _instance = new BasRunningApp();
        }
        public static BasRunningApp Instance()
        {
            if (_instance == null)
            {
                _instance = new BasRunningApp();                
            }

            return _instance;
        }


    }
}
