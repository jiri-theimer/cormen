using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public class Factory
    {        
        public BO.RunningUser CurrentUser;
        private Ij02PersonBL _j02;
        private Ip10MasterProductBL _p10;
        private Ip13MasterTpvBL _p13;
        private Ip28CompanyBL _p28;
        private Ip21LicenseBL _p21;
        private Ip26MszBL _p26;
        private Ij04UserRoleBL _j04;
        private Ib02StatusBL _b02;
        private Io12CategoryBL _o12;
        private Io23DocBL _o23;
        private IDataGridBL _grid;
        private ICBL _cbl;
        private IFBL _fbl;
        private Ip85TempboxBL _p85;

        
        public Factory(string strLogin)
        {
            CurrentUser = DL.DbHandler.Load<BO.RunningUser>("SELECT a.j02ID as pid,a.j02Login,a.j02FirstName+' '+a.j02LastName as FullName,a.j02IsMustChangePassword,b.j04PermissionValue,null as ErrorMessage FROM j02Person a INNER JOIN j04UserRole b ON a.j04ID=b.j04ID WHERE a.j02Login LIKE @login", new { login = strLogin });


        }
        public IDataGridBL gridBL
        {
            get
            {
                if (_grid == null) _grid = new DataGridBL(CurrentUser);
                return _grid;
            }
        }
        public ICBL CBL
        {
            get
            {
                if (_cbl == null) _cbl = new CBL(CurrentUser);
                return _cbl;
            }
        }
        public IFBL FBL
        {
            get
            {
                if (_fbl == null) _fbl = new FBL(CurrentUser);
                return _fbl;
            }
        }

        public Ij02PersonBL j02PersonBL
        {
            get
            {
                if (_j02 == null) _j02 = new j02PersonBL(CurrentUser);
                return _j02;
            }
        }
        public Ij04UserRoleBL j04UserRoleBL
        {
            get
            {
                if (_j04 == null) _j04= new j04UserRoleBL(CurrentUser);
                return _j04;
            }
        }
        public Ip28CompanyBL p28CompanyBL
        {
            get
            {
                if (_p28 == null) _p28 = new p28CompanyBL(CurrentUser);
                return _p28;
            }
        }
        public Ip26MszBL p26MszBL
        {
            get
            {
                if (_p26 == null) _p26 = new p26MszBL(CurrentUser);
                return _p26;
            }
        }
        public Ip21LicenseBL p21LicenseBL
        {
            get
            {
                if (_p21 == null) _p21 = new p21LicenseBL(CurrentUser);
                return _p21;
            }
        }
        public Ib02StatusBL b02StatusBL
        {
            get
            {
                if (_b02 == null) _b02 = new b02StatusBL(CurrentUser);
                return _b02;
            }
        }
        public Io12CategoryBL o12CategoryBL
        {
            get
            {
                if (_o12 == null) _o12 = new o12CategoryBL(CurrentUser);
                return _o12;
            }
        }
        public Io23DocBL o23DocBL
        {
            get
            {
                if (_o23 == null) _o23 = new o23DocBL(CurrentUser);
                return _o23;
            }
        }
        public Ip13MasterTpvBL p13MasterTpvBL
        {
            get
            {
                if (_p13 == null) _p13 = new p13MasterTpvBL(CurrentUser);
                return _p13;
            }
        }
        public Ip10MasterProductBL p10MasterProductBL
        {
            get
            {
                if (_p10 == null) _p10 = new p10MasterProductBL(CurrentUser);
                return _p10;
            }
        }
        public Ip85TempboxBL ip85TempboxBL
        {
            get
            {
                if (_p85 == null) _p85 = new p85TempboxBL(CurrentUser);
                return _p85;
            }
        }
    }
}
