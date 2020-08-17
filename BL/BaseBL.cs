﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    class BaseBL
    {
        protected BL.Factory _mother;
        protected DL.DbHandler _db;
        private readonly System.Text.StringBuilder _sb;

        public BaseBL(BL.Factory mother)
        {
            _mother = mother;
            _db = new DL.DbHandler(_mother.App.ConnectString, _mother.CurrentUser,_mother.App.LogFolder);
            _sb = new System.Text.StringBuilder();
        }

        public void AddMessage(string strMessage, string template = "error")
        {

            _mother.CurrentUser.AddMessage(strMessage, template);
        }


        public void sb(string s = null)
        {
          
            if (s != null)
            {
                _sb.Append(s);
            }

        }
        public void sbinit()
        {
            _sb.Clear();
        }
        public string sbret()
        {
            string s = _sb.ToString();
            sbinit();
            return s;
        }

    }
}
