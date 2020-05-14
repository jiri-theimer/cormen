using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip13MasterTpvBL
    {
        public BO.p13MasterTpv Load(int pid);
        public IEnumerable<BO.p13MasterTpv> GetList(BO.myQuery mq);
        public int Save(BO.p13MasterTpv rec, List<BO.p14MasterOper> lisP14);
    }
    class p13MasterTpvBL : BaseBL,Ip13MasterTpvBL
    {     
        public p13MasterTpvBL(BL.Factory mother):base(mother)
        {
           
        }
       
       
        private string GetSQL1()
        {
            return "SELECT a.*,p25.p25Name," + _db.GetSQL1_Ocas("p13") + " FROM "+ BL.TheEntities.ByPrefix("p13").SqlFrom;
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

        public int Save(BO.p13MasterTpv rec,List<BO.p14MasterOper> lisP14)
        {
            if (rec.p25ID == 0)
            {
                _mother.CurrentUser.AddMessage("Chybí vyplnit typ zařízení.");
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p13ID);
            p.AddInt("p25ID", rec.p25ID);
            p.AddString("p13Name", rec.p13Name);
            p.AddString("p13Code", rec.p13Code);
            p.AddString("p13Memo", rec.p13Memo);
           
            int intPID = _db.SaveRecord("p13MasterTpv", p.getDynamicDapperPars(), rec);

            if (intPID > 0 && lisP14 !=null)
            {
                foreach (var c in lisP14)
                {
                    c.pid = c.p14ID;
                    c.p13ID = intPID;
                    if (c.TempRecDisplay == "none" && c.p14ID > 0)
                    {
                        _db.RunSql("DELETE FROM p14MasterOper WHERE p14ID=@p14id", new { p14id = c.p14ID });
                    }
                    else
                    {
                        SaveP14Rec(c);
                    }
                    
                }
            }

            return intPID;

        }

        private int SaveP14Rec(BO.p14MasterOper rec)
        {
            var p = new DL.Params4Dapper();            
            p.AddInt("pid", rec.p14ID);
            p.AddInt("p13ID", rec.p13ID, true);
            p.AddInt("p19ID", rec.p19ID, true);
            p.AddInt("p18ID", rec.p18ID, true);
            p.AddString("p14Name", rec.p14Name);
           
            p.AddInt("p14RowNum", rec.p14RowNum);            
            p.AddString("p14OperNum", rec.p14OperNum);
            p.AddInt("p14OperParam", rec.p14OperParam);
            p.AddDouble("p14UnitsCount", rec.p14UnitsCount);
            p.AddDouble("p14DurationPreOper", rec.p14DurationPreOper);
            p.AddDouble("p14DurationPostOper", rec.p14DurationPostOper);
            p.AddDouble("p14DurationOper", rec.p14DurationOper);

            return _db.SaveRecord("p14MasterOper", p.getDynamicDapperPars(), rec);
        }

        
    }
}
