using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public class Factory
    {
        private Ij02PersonBL _j02;
        private Ip10MasterProductBL _p10;
        private Ip13TpvBL _p13;
        private Ip28CompanyBL _p28;
        private Ip21LicenseBL _p21;
        private Ip26MszBL _p26;
        private Ij04UserRoleBL _j04;
        private Ib02StatusBL _b02;
        private Io12CategoryBL _o12;
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
        public Ip21LicenseBL p21LicenseBL
        {
            get
            {
                if (_p21 == null) _p21 = new p21LicenseBL();
                return _p21;
            }
        }
        public Ib02StatusBL b02StatusBL
        {
            get
            {
                if (_b02 == null) _b02 = new b02StatusBL();
                return _b02;
            }
        }
        public Io12CategoryBL o12CategoryBL
        {
            get
            {
                if (_o12 == null) _o12 = new o12CategoryBL();
                return _o12;
            }
        }
        public Ip13TpvBL p13TpvBL
        {
            get
            {
                if (_p13 == null) _p13 = new p13TpvBL();
                return _p13;
            }
        }
        public Ip10MasterProductBL p10MasterProductBL
        {
            get
            {
                if (_p10 == null) _p10 = new p10MasterProductBL();
                return _p10;
            }
        }
    }
}
