﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ip26MszBL
    {
        public BO.p26Msz Load(int pid);
        public IEnumerable<BO.p26Msz> GetList(BO.myQuery mq);
        public int Save(BO.p26Msz rec,List<int>p27IDs);
        
    }
    class p26MszBL:BaseBL,Ip26MszBL
    {      
        public p26MszBL(BL.Factory mother):base(mother)
        {
           
        }
        
        
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p26") + ",b02.b02Name,p28.p28Name,p25.p25Name FROM p26Msz a INNER JOIN p25MszType p25 ON a.p25ID=p25.p25ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID";
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
        

        public int Save(BO.p26Msz rec, List<int> p27IDs)
        {
            if (rec.p25ID == 0)
            {
                _mother.CurrentUser.AddMessage("Chybí vyplnit typ zařízení.");
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p26ID);            
            if (rec.pid == 0)
            {
                rec.b02ID = _mother.b02StatusBL.LoadStartStatusPID("p26", rec.b02ID);  //startovací workflow stav
            }
            p.AddInt("p25ID", rec.p25ID, true);
            p.AddInt("b02ID",rec.b02ID,true);
            p.AddInt("p28ID", rec.p28ID,true);            
            p.AddString("p26Name", rec.p26Name);           
            p.AddString("p26Code", rec.p26Code);
            p.AddString("p26Memo", rec.p26Memo);
            

            int intPID= _db.SaveRecord("p26Msz", p.getDynamicDapperPars(), rec);
            if (p27IDs != null)
            {
                _db.RunSql("DELETE FROM p29MszUnitBinding WHERE p26ID=@pid", new { pid = intPID });
                if (p27IDs.Count > 0)
                {
                    _db.RunSql("INSERT INTO p29MszUnitBinding(p26ID,p27ID) SELECT @pid,p27ID FROM p27MszUnit WHERE p27ID IN ("+string.Join(",",p27IDs)+")", new { pid = intPID });
                }
            }
            
            
            

            return intPID;
        }
    }
}
