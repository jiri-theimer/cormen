﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip26MszBL
    {
        public BO.p26Msz Load(int pid);
        public IEnumerable<BO.p26Msz> GetList(BO.myQuery mq);
        public int Save(BO.p26Msz rec);
    }
    class p26MszBL:BaseBL,Ip26MszBL
    {      
        public p26MszBL(BL.Factory mother):base(mother)
        {
           
        }
        
        
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p26") + ",b02.b02Name,p28.p28Name,o12.o12Name,p25.p25Name FROM "+ BL.TheEntities.ByPrefix("p26").SqlFrom;
        }
        public BO.p26Msz Load(int pid)
        {
            return _db.Load<BO.p26Msz>(string.Format("{0} WHERE a.p26ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.p26Msz> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p26Msz>(fq.FinalSql, fq.Parameters);
        }

        public int Save(BO.p26Msz rec)
        {
            if (rec.p25ID == 0)
            {
                _mother.CurrentUser.AddMessage("Chybí vyplnit typ zařízení.");
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p26ID);
            p.AddInt("p25ID", rec.p25ID, true);
            p.AddInt("b02ID",rec.b02ID,true);
            p.AddInt("p28ID", rec.p28ID,true);
            p.AddInt("o12ID", rec.o12ID, true);
            p.AddInt("p31ID", rec.p31ID, true);
            p.AddString("p26Name", rec.p26Name);           
            p.AddString("p26Code", rec.p26Code);
            p.AddString("p26Memo", rec.p26Memo);
            

            return _db.SaveRecord("p26Msz", p.getDynamicDapperPars(), rec);
        }
    }
}
