﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip13MasterTpvBL
    {
        public BO.p13MasterTpv Load(int pid);
        public IEnumerable<BO.p13MasterTpv> GetList(BO.myQuery mq);
        public int Save(BO.p13MasterTpv rec);
    }
    class p13MasterTpvBL : BaseBL,Ip13MasterTpvBL
    {     
        public p13MasterTpvBL(BL.Factory mother):base(mother)
        {
           
        }
       
       
        private string GetSQL1()
        {
            return "SELECT a.*,p25.p25Name," + _db.GetSQL1_Ocas("p13") + " FROM p13MasterTpv a INNER JOIN p25MszType p25 ON a.p25ID=p25.p25ID";
        }
        public BO.p13MasterTpv Load(int pid)
        {

            return _db.Load<BO.p13MasterTpv>(string.Format("{0} WHERE a.p13ID=@pid", GetSQL1()),new { pid = pid });
        }
        public IEnumerable<BO.p13MasterTpv> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p13MasterTpv>(fq.FinalSql, fq.Parameters);

        }        

        public int Save(BO.p13MasterTpv rec)
        {

            if (rec.p25ID == 0 || string.IsNullOrEmpty(rec.p13Code) || string.IsNullOrEmpty(rec.p13Name))
            {
                _mother.CurrentUser.AddMessage("Chybí vyplnit typ zařízení, kód nebo název receptury.");
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p13ID);
            p.AddInt("p25ID", rec.p25ID);
            p.AddString("p13Name", rec.p13Name);
            p.AddString("p13Code", rec.p13Code);
            p.AddString("p13Memo", rec.p13Memo);
           
            int intPID = _db.SaveRecord("p13MasterTpv", p.getDynamicDapperPars(), rec);

            
            return intPID;

        }

        

        
    }
}
