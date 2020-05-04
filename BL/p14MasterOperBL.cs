using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip14MasterOperBL
    {
        public BO.p14MasterOper Load(int pid);
        public IEnumerable<BO.p14MasterOper> GetList(BO.myQuery mq);
        public int Save(BO.p14MasterOper rec);
    }
    class p14MasterOperBL : BaseBL,Ip14MasterOperBL
    {       
        public p14MasterOperBL(BL.Factory mother):base(mother)
        {
            
        }
        
        
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p14") + " FROM p14MasterOper a";
        }
        public BO.p14MasterOper Load(int pid)
        {
            return _db.Load<BO.p14MasterOper>(string.Format("{0} WHERE a.p14ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p14MasterOper> GetList(BO.myQuery mq)
        {
            mq.explicit_orderby = "a.p14RowNum";
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return _db.GetList<BO.p14MasterOper>(fq.FinalSql, fq.Parameters);

        }
        

        public int Save(BO.p14MasterOper rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p14ID);
            p.AddInt("p13ID", rec.p13ID,true);
            p.AddString("p14Name", rec.p14Name);
            p.AddString("p14MaterialCode", rec.p14MaterialCode);
            p.AddString("p14MaterialName", rec.p14MaterialName);
            p.AddInt("p14RowNum", rec.p14RowNum);
            p.AddString("p14OperCode", rec.p14OperCode);
            p.AddString("p14OperNum", rec.p14OperNum);
            p.AddInt("p14OperParam", rec.p14OperParam);
            p.AddDouble("p14UnitsCount", rec.p14UnitsCount);
            p.AddDouble("p14DurationPreOper", rec.p14DurationPreOper);
            p.AddDouble("p14DurationPostOper", rec.p14DurationPostOper);
            p.AddDouble("p14DurationOper", rec.p14DurationOper);


            return _db.SaveRecord( "p14MasterOper", p.getDynamicDapperPars(), rec);
        }
    }
}
