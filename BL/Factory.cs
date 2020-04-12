using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public class Factory
    {
        private Ij02PersonBL _j02;
        private Ip28CompanyBL _p28;
        private Ip26MszBL _p26;
        private Ij04UserRoleBL _j04;
        private IDataGridBL _grid;
        private ICBL _cbl;
        private IFBL _fbl;
        
        public Factory(string login)
        {
            AppRunning.SetCurrentUser(login);
            if (login != "")
            {
                var s = AppRunning.Get().UserID;
            }
            

        }
        public IDataGridBL gridBL
        {
            get
            {
                if (_grid == null) _grid = new DataGridBL();
                return _grid;
            }
        }
        public ICBL CBL
        {
            get
            {
                if (_cbl == null) _cbl = new CBL();
                return _cbl;
            }
        }
        public IFBL FBL
        {
            get
            {
                if (_fbl == null) _fbl = new FBL();
                return _fbl;
            }
        }

        public Ij02PersonBL j02PersonBL
        {
            get
            {
                if (_j02 == null) _j02 = new j02PersonBL();
                return _j02;
            }
        }
        public Ij04UserRoleBL j04UserRoleBL
        {
            get
            {
                if (_j04 == null) _j04= new j04UserRoleBL();
                return _j04;
            }
        }
        public Ip28CompanyBL p28CompanyBL
        {
            get
            {
                if (_p28 == null) _p28 = new p28CompanyBL();
                return _p28;
            }
        }
        public Ip26MszBL p26MszBL
        {
            get
            {
                if (_p26 == null) _p26 = new p26MszBL();
                return _p26;
            }
        }
    }
}
