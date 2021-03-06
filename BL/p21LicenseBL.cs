﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ip21LicenseBL
    {
        public BO.p21License Load(int pid);
        public IEnumerable<BO.p21License> GetList(BO.myQuery mq);
        public int Save(BO.p21License rec);
        BO.Result CreateClientProducts(int intP21ID);
        public bool HasClientValidLicense(int p28id);
        public bool AppendP10IDs(int p21id, List<int> p10ids);
        public bool RemoveP10IDs(int p21id, List<int> p10ids);
    }
    class p21LicenseBL:BaseBL,Ip21LicenseBL
    {
        public p21LicenseBL(BL.Factory mother):base(mother)
        {
           
        }
      
       
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p21") + ",b02.b02Name,p28.p28Name FROM p21License a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID";
        }
        public BO.p21License Load(int pid)
        {
            return _db.Load<BO.p21License>(string.Format("{0} WHERE a.p21ID=@pid", GetSQL1()), new { pid = pid });
        }
        public BO.p21License LoadByCode(string strCode, int intExcludePID)
        {
            return _db.Load<BO.p21License>(string.Format("{0} WHERE a.p21Code LIKE @code AND a.p21ID<>@exclude", GetSQL1()), new { code = strCode, exclude = intExcludePID });
        }
        public IEnumerable<BO.p21License> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p21License>(fq.FinalSql, fq.Parameters);
            
        }

        public bool HasClientValidLicense(int p28id)
        {
            int intP21ID=_db.Load<BO.COM.GetInteger>("SELECT top 1 a.p21ID as Value FROM p21License a WHERE a.p28ID=@p28id AND GETDATE() BETWEEN a.ValidFrom AND a.ValidUntil", new { p28id = p28id }).Value;
            if (intP21ID > 0)
            {
                return true;
            }else
            {
                return false;
            }
        }
        
        public bool AppendP10IDs(int p21id, List<int> p10ids)
        {
            if (p21id == 0)
            {
                this.AddMessage("Na vstupu chybí licence."); return false;
            }
            if (p10ids.Count == 0)
            {
                this.AddMessage("Na vstupu nejsou produkty.");return false;             
            }
            var mq = new BO.myQuery("p10");
            mq.pids = p10ids;
            if (_mother.p10MasterProductBL.GetList(mq).Where(p => p.p13ID == 0).Count() > 0)
            {
                this.AddMessage("Ve výběru produktů je minimálně jeden, u kterého chybí vazba na recepturu."); return false;
            }

            _db.RunSql("INSERT INTO p22LicenseBinding(p21ID,p10ID) SELECT @pid,p10ID FROM p10MasterProduct WHERE p10ID IN (" + string.Join(",", p10ids) + ")", new { pid = p21id });

            return true;
        }
        public bool RemoveP10IDs(int p21id, List<int> p10ids)
        {
            if (p21id == 0)
            {
                this.AddMessage("Na vstupu chybí licence."); return false;
            }
            if (p10ids.Count == 0)
            {
                this.AddMessage("Na vstupu nejsou produkty."); return false;
            }
            _db.RunSql("DELETE FROM p22LicenseBinding WHERE p21ID=@pid AND p10ID IN (" + string.Join(",", p10ids) + ")", new { pid = p21id });

            return true;
        }
        public int Save(BO.p21License rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p21ID);
            if (rec.pid == 0)
            {
                rec.b02ID = _mother.b02StatusBL.LoadStartStatusPID("p21", rec.b02ID);  //startovací workflow stav
            }
            p.AddInt("b02ID", rec.b02ID, true);
            p.AddInt("p28ID", rec.p28ID, true);

            p.AddEnumInt("p21PermissionFlag", rec.p21PermissionFlag);
            p.AddString("p21Name", rec.p21Name);
            p.AddString("p21Code", rec.p21Code);
            p.AddString("p21Memo", rec.p21Memo);
            p.AddDouble("p21Price", rec.p21Price);
            p.AddDateTime("ValidFrom", rec.ValidFrom);
            p.AddDateTime("ValidUntil", rec.ValidUntil);

            int intPID= _db.SaveRecord("p21License", p.getDynamicDapperPars(), rec);
            
            return intPID;

            

        }

        public BO.Result CreateClientProducts(int intP21ID)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("userid", _db.CurrentUser.pid);            
            p.Add("p21id", intP21ID, System.Data.DbType.Int32);
            p.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            string s1 = _db.RunSp("p21_create_clientproducts", ref p);
            if (s1 == "1")
            {
                return new BO.Result(false,"Operace proběhla");
            }
            else
            {
                return new BO.Result(true, s1);
               
            }

        }



    }
}

