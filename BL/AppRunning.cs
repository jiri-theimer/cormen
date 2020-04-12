using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public class AppRunning
    {
        private static AppRunning _instance = null;
        private BO.j02Person _user = null;
        private static string _Login;
        public static void SetCurrentUser(string strLogin)
        {
            _Login = strLogin;
        }

        public DateTime InstanceTimestamp = DateTime.Now;

        public BO.j02Person User
        {            
            get
            {
                if (_Login == null) return null;
                
                return _user;
            }
        }

        public int UserID { get
            {
                if (_user == null) return 0;
                return _user.pid;
            }
        }

        private AppRunning()
        {
            //záměrně privat konstruktor
            if (_user == null)
            {
                _user = DL.DbHandler.Load<BO.j02Person>("SELECT a.*," + DL.DbHandler.GetSQL1_Ocas("j02") + " FROM j02Person a WHERE a.j02Login LIKE @login", new { login = _Login });                
            }
        }
        public static void RefreshIntance()
        {            
            _instance = new AppRunning();
        }
        public static AppRunning Get()
        {
            if (_instance == null)
            {
                _instance = new AppRunning();                
            }

            return _instance;
        }


    }
}
