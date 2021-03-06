﻿using System;
using System.Collections.Generic;
using System.Net.Cache;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace BL
{
    public class Factory
    {                
        public BO.RunningUser CurrentUser { get; set; }
        public BL.RunningApp App { get; set; }
        

        private Ij02PersonBL _j02;
        private Ij03UserBL _j03;
        private Ij72TheGridTemplateBL _j72;
        private Ip10MasterProductBL _p10;        
        private Ip13MasterTpvBL _p13;
        private Ip28CompanyBL _p28;
        private Ip21LicenseBL _p21;
        private Ip26MszBL _p26;
        private Ip25MszTypeBL _p25;
        private Ip27MszUnitBL _p27;
        private Ij04UserRoleBL _j04;
        private Ib02StatusBL _b02;
        private Ib03StatusGroupBL _b03;
        private Io53TagGroupBL _o53;
        private Io51TagBL _o51;
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
        private Ip44TaskOperPlanBL _p44;
        private Ip45TaskOperRealBL _p45;
        private Ip51OrderBL _p51;
        private Ip52OrderItemBL _p52;
        private Ip19MaterialBL _p19;
        private Ip20UnitBL _p20;
        private Ip18OperCodeBL _p18;
        private Ip31CapacityFondBL _p31;
        private Ix31ReportBL _x31;
        private IMailBL _mail;

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
            this.CurrentUser= db.Load<BO.RunningUser>("SELECT a.j03ID as pid,a.j02ID,a.j04ID,j02.p28ID,p28.p28Name,a.j03Login,j02.j02FirstName+' '+j02.j02LastName as FullName,a.j03FontStyleFlag,a.j03EnvironmentFlag,a.j03GridSelectionModeFlag,a.j03IsMustChangePassword,j04.j04PermissionValue,null as ErrorMessage,CASE WHEN GETDATE() BETWEEN a.ValidFrom AND a.ValidUntil THEN 0 ELSE 1 end as isclosed,a.j03LiveChatTimestamp,j02.j02Email,a.j03PingTimestamp,j04.j04IsClientRole FROM j03User a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID INNER JOIN p28Company p28 ON j02.p28ID=p28.p28ID WHERE a.j03Login LIKE @login", new { login = strLogin });
            if (this.CurrentUser.j03Login.Contains("marktime")==false)
            {
                if (this.CurrentUser.j04IsClientRole)
                {
                    this.CurrentUser.j03EnvironmentFlag = 2;    //natvrdo CLIENT prostředí
                }
                else
                {
                    this.CurrentUser.j03EnvironmentFlag = 1;    //natvrdo MASTER prostředí
                }
            }
                        


        }
        //logování přihlášení musí být zde, protože se logují i neńsspěšné pokusy o přihlášení a nešlo by to řešit v j03UserBL
        public void Write2AccessLog(BO.j90LoginAccessLog c) //zápis úspěšných přihlášení i neúspěšných pokusů o přihlášení
        {                                  
            DL.DbHandler db = new DL.DbHandler(this.App.ConnectString, this.CurrentUser, this.App.LogFolder);
            string s = "INSERT INTO j90LoginAccessLog(j03ID,j90Date,j90BrowserUserAgent,j90BrowserFamily,j90BrowserOS,j90BrowserDeviceType,j90BrowserDeviceFamily,j90BrowserAvailWidth,j90BrowserAvailHeight,j90BrowserInnerWidth,j90BrowserInnerHeight,j90LoginMessage,j90LoginName,j90CookieExpiresInHours,j90LocationHost)";
            s += " VALUES(@j03id,GETDATE(),@useragent,@browser,@os,@devicetype,@devicefamily,@aw,@ah,@iw,@ih,@mes,@loginname,@cookieexpire,@host)";
            db.RunSql(s,new {j03id=BO.BAS.TestIntAsDbKey(c.j03ID), useragent = c.j90BrowserUserAgent,browser= c.j90BrowserFamily,os=c.j90BrowserOS, devicetype=c.j90BrowserDeviceType, devicefamily=c.j90BrowserDeviceFamily,aw=c.j90BrowserAvailWidth,ah=c.j90BrowserAvailHeight,iw=c.j90BrowserInnerWidth,ih=c.j90BrowserInnerHeight,mes=c.j90LoginMessage, loginname=c.j90LoginName, cookieexpire=c.j90CookieExpiresInHours, host=c.j90LocationHost });
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
        public Ij72TheGridTemplateBL j72TheGridTemplateBL
        {
            get
            {
                if (_j72 == null) _j72 = new j72TheGridTemplateBL(this);
                return _j72;
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
        public Ip25MszTypeBL p25MszTypeBL
        {
            get
            {
                if (_p25 == null) _p25 = new p25MszTypeBL(this);
                return _p25;
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
        public Ip27MszUnitBL p27MszUnitBL
        {
            get
            {
                if (_p27 == null) _p27 = new p27MszUnitBL(this);
                return _p27;
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
        public Ib03StatusGroupBL b03StatusGroupBL
        {
            get
            {
                if (_b03 == null) _b03 = new b03StatusGroupBL(this);
                return _b03;
            }
        }
        public Io53TagGroupBL o53TagGroupBL
        {
            get
            {
                if (_o53 == null) _o53 = new o53TagGroupBL(this);
                return _o53;
            }
        }
        public Io51TagBL o51TagBL
        {
            get
            {
                if (_o51 == null) _o51 = new o51TagBL(this);
                return _o51;
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
        public Ip44TaskOperPlanBL p44TaskOperPlanBL
        {
            get
            {
                if (_p44 == null) _p44 = new p44TaskOperPlanBL(this);
                return _p44;
            }
        }
        public Ip45TaskOperRealBL p45TaskOperRealBL
        {
            get
            {
                if (_p45 == null) _p45 = new p45TaskOperRealBL(this);
                return _p45;
            }
        }
        public Ip51OrderBL p51OrderBL
        {
            get
            {
                if (_p51 == null) _p51 = new p51OrderBL(this);
                return _p51;
            }
        }
        public Ip52OrderItemBL p52OrderItemBL
        {
            get
            {
                if (_p52 == null) _p52 = new p52OrderItemBL(this);
                return _p52;
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
        public Ip19MaterialBL p19MaterialBL
        {
            get
            {
                if (_p19 == null) _p19 = new p19MaterialBL(this);
                return _p19;
            }
        }
        public Ip20UnitBL p20UnitBL
        {
            get
            {
                if (_p20 == null) _p20 = new p20UnitBL(this);
                return _p20;
            }
        }
        public Ip18OperCodeBL p18OperCodeBL
        {
            get
            {
                if (_p18 == null) _p18 = new p18OperCodeBL(this);
                return _p18;
            }
        }

        public Ip31CapacityFondBL p31CapacityFondBL
        {
            get
            {
                if (_p31 == null) _p31 = new p31CapacityFondBL(this);
                return _p31;
            }
        }

        public Ix31ReportBL x31ReportBL
        {
            get
            {
                if (_x31 == null) _x31 = new x31ReportBL(this);
                return _x31;
            }
        }
        public IMailBL MailBL
        {
            get
            {
                if (_mail == null) _mail = new MailBL(this);
                return _mail;
            }
        }
    }
}
