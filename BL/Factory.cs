using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BL
{
    public class Factory
    {        
        //public BO.RunningUser CurrentUser { get { return _fd.CurrentUser; } }
        public BO.RunningUser CurrentUser { get; set; }
        public BL.RunningApp App { get; set; }

        private Ij02PersonBL _j02;
        private Ij03UserBL _j03;
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
        private Ip14MasterOperBL _p14;
        private Ip11ClientProductBL _p11;
        private Ip12ClientTpvBL _p12;
        private Ip15ClientOperBL _p15;
        private Ip41TaskBL _p41;

        public Factory(BO.RunningUser c,BL.RunningApp runningapp)
        {
                        
            this.CurrentUser = c;
            this.App = runningapp;

            if (c.pid == 0 && string.IsNullOrEmpty(c.j03Login)==false)
            {
                InhaleUserByLogin(c.j03Login);
                
            }
            
        }

        

        public void InhaleUserByLogin(string strLogin)
        {
            DL.DbHandler db = new DL.DbHandler(this.App.ConnectString, this.CurrentUser,this.App.LogFolder);
            this.CurrentUser= db.Load<BO.RunningUser>("SELECT a.j03ID as pid,a.j02ID,j02.p28ID,p28.p28Name,a.j03Login,j02.j02FirstName+' '+j02.j02LastName as FullName,a.j03FontStyleFlag,a.j03EnvironmentFlag,a.j03IsMustChangePassword,j04.j04PermissionValue,null as ErrorMessage,CASE WHEN GETDATE() BETWEEN a.ValidFrom AND a.ValidUntil THEN 0 ELSE 1 end as isclosed,a.j03LiveChatTimestamp,j02.j02Email FROM j03User a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID INNER JOIN p28Company p28 ON j02.p28ID=p28.p28ID WHERE a.j03Login LIKE @login", new { login = strLogin });
            if (this.CurrentUser.j03LiveChatTimestamp !=null)
            {
                if (this.CurrentUser.j03LiveChatTimestamp.Value.AddMinutes(20) < DateTime.Now)
                {
                    var c = this.j03UserBL.Load(this.CurrentUser.pid);
                    c.j03LiveChatTimestamp = null;   //vypnout smartsupp
                    this.j03UserBL.Save(c);
                }
            }
        }



        public IDataGridBL gridBL
        {
            get
            {
                if (_grid == null) _grid = new DataGridBL(this);
                return _grid;
            }
        }
        public ICBL CBL
        {
            get
            {
                if (_cbl == null) _cbl = new CBL(this);
                return _cbl;
            }
        }
        public IFBL FBL
        {
            get
            {
                if (_fbl == null) _fbl = new FBL(this);
                return _fbl;
            }
        }
        public Ij03UserBL j03UserBL
        {
            get
            {
                if (_j03 == null) _j03 = new j03UserBL(this);
                return _j03;
            }
        }
        public Ij02PersonBL j02PersonBL
        {
            get
            {
                if (_j02 == null) _j02 = new j02PersonBL(this);
                return _j02;
            }
        }
        public Ij04UserRoleBL j04UserRoleBL
        {
            get
            {
                if (_j04 == null) _j04= new j04UserRoleBL(this);
                return _j04;
            }
        }
        public Ip28CompanyBL p28CompanyBL
        {
            get
            {
                if (_p28 == null) _p28 = new p28CompanyBL(this);
                return _p28;
            }
        }
        public Ip26MszBL p26MszBL
        {
            get
            {
                if (_p26 == null) _p26 = new p26MszBL(this);
                return _p26;
            }
        }
        public Ip21LicenseBL p21LicenseBL
        {
            get
            {
                if (_p21 == null) _p21 = new p21LicenseBL(this);
                return _p21;
            }
        }
        public Ib02StatusBL b02StatusBL
        {
            get
            {
                if (_b02 == null) _b02 = new b02StatusBL(this);
                return _b02;
            }
        }
        public Io12CategoryBL o12CategoryBL
        {
            get
            {
                if (_o12 == null) _o12 = new o12CategoryBL(this);
                return _o12;
            }
        }
        public Io23DocBL o23DocBL
        {
            get
            {
                if (_o23 == null) _o23 = new o23DocBL(this);
                return _o23;
            }
        }
        public Ip13MasterTpvBL p13MasterTpvBL
        {
            get
            {
                if (_p13 == null) _p13 = new p13MasterTpvBL(this);
                return _p13;
            }
        }
        public Ip14MasterOperBL p14MasterOperBL
        {
            get
            {
                if (_p14 == null) _p14 = new p14MasterOperBL(this);
                return _p14;
            }
        }
        public Ip10MasterProductBL p10MasterProductBL
        {
            get
            {
                if (_p10 == null) _p10 = new p10MasterProductBL(this);
                return _p10;
            }
        }
        public Ip11ClientProductBL p11ClientProductBL
        {
            get
            {
                if (_p11 == null) _p11 = new p11ClientProductBL(this);
                return _p11;
            }
        }
        public Ip12ClientTpvBL p12ClientTpvBL
        {
            get
            {
                if (_p12 == null) _p12 = new p12ClientTpvBL(this);
                return _p12;
            }
        }
        public Ip15ClientOperBL p15ClientOperBL
        {
            get
            {
                if (_p15 == null) _p15 = new p15ClientOperBL(this);
                return _p15;
            }
        }
        public Ip41TaskBL p41TaskBL
        {
            get
            {
                if (_p41 == null) _p41 = new p41TaskBL(this);
                return _p41;
            }
        }
        public Ip85TempboxBL p85TempboxBL
        {
            get
            {
                if (_p85 == null) _p85 = new p85TempboxBL(this);
                return _p85;
            }
        }
    }
}
