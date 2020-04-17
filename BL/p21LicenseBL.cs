﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip21LicenseBL
    {
        public BO.p21License Load(int pid);
        public IEnumerable<BO.p21License> GetList(BO.myQuery mq);
        public int Save(BO.p21License rec, List<int> p10ids = null);
    }
    class p21LicenseBL:Ip21LicenseBL
    {
        private BO.RunningUser _cUser;
        public p21LicenseBL(BO.RunningUser cUser)
        {
            _cUser = cUser;
        }
        private string GetSQL1()
        {
            return "SELECT a.*," + DL.DbHandler.GetSQL1_Ocas("p21") + ",b02.b02Name as _b02name,p28.p28Name as _p28Name FROM p21License a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID";
        }
        public BO.p21License Load(int pid)
        {
            return DL.DbHandler.Load<BO.p21License>(string.Format("{0} WHERE a.p21ID={1}", GetSQL1(), pid));
        }
        public BO.p21License LoadByCode(string strCode, int intExcludePID)
        {
            return DL.DbHandler.Load<BO.p21License>(string.Format("{0} WHERE a.p21Code LIKE @code AND a.p21ID<>@exclude", GetSQL1()), new { code = strCode, exclude = intExcludePID });
        }
        public IEnumerable<BO.p21License> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return DL.DbHandler.GetList<BO.p21License>(fq.FinalSql, fq.Parameters);
            
        }
        

        public int Save(BO.p21License rec,List<int> p10ids)
        {           
            var strGUID = BO.BAS.GetGuid();
            BO.p85Tempbox cP85;
            var tempBL = new BL.p85TempboxBL(_cUser);
            cP85 = new BO.p85Tempbox() {p85RecordPid=rec.pid, p85GUID = strGUID, p85Prefix = "p21", p85FreeText01 = rec.p21Name, p85FreeText02 = rec.p21Code, p85Message = rec.p21Memo, p85OtherKey1 = rec.p28ID, p85OtherKey2 = rec.b02ID, p85FreeDate01 = rec.ValidFrom, p85FreeDate02 = rec.ValidUntil };
            tempBL.Save(cP85);

            foreach (var p10id in p10ids)
            {
                cP85= new BO.p85Tempbox() { p85GUID = strGUID, p85Prefix = "p22", p85OtherKey1 = p10id };                
                tempBL.Save(cP85);
            }
           
            var p = new Dapper.DynamicParameters();
            p.Add("userid", _cUser.pid);                 
            p.Add("guid", strGUID);
            p.Add("pid_ret", rec.pid, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            p.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            string s1 = DL.DbHandler.RunSp("p21_save",ref p);
            if (s1 == "1")
            {
                return p.Get<int>("pid_ret");
            }
            else
            {                
                _cUser.ErrorMessage = s1;
                return 0;
            }           
            
        }

       
    }
}

