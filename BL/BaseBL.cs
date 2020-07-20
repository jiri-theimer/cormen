using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    class BaseBL
    {
        protected BL.Factory _mother;
        protected DL.DbHandler _db;

        public BaseBL(BL.Factory mother)
        {
            _mother = mother;
            _db = new DL.DbHandler(_mother.App.ConnectString, _mother.CurrentUser,_mother.App.LogFolder);
        }

        public void AddMessage(string strMessage, string template = "error")
        {

            _mother.CurrentUser.AddMessage(strMessage, template);
        }

    }
}
