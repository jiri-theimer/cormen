using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip15ClientOperBL
    {
        public BO.p15ClientOper Load(int pid);
        public IEnumerable<BO.p15ClientOper> GetList(BO.myQuery mq);
        public int Save(BO.p15ClientOper rec);
    }
    class p15ClientOperBL : BaseBL, Ip15ClientOperBL
    {
        public p15ClientOperBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p15") + " FROM p15ClientOper a";
        }
        public BO.p15ClientOper Load(int pid)
        {
            return _db.Load<BO.p15ClientOper>(string.Format("{0} WHERE a.p15ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p15ClientOper> GetList(BO.myQuery mq)
        {
            mq.explicit_orderby = "a.p15RowNum";
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p15ClientOper>(fq.FinalSql, fq.Parameters);

        }


        public int Save(BO.p15ClientOper rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p15ID);
            p.AddInt("p12ID", rec.p12ID, true);
            p.AddString("p15Name", rec.p15Name);
            p.AddString("p15MaterialCode", rec.p15MaterialCode);
            p.AddString("p15MaterialName", rec.p15MaterialName);
            p.AddInt("p15RowNum", rec.p15RowNum);
            p.AddString("p15OperCode", rec.p15OperCode);
            p.AddString("p15OperNum", rec.p15OperNum);
            p.AddInt("p15OperParam", rec.p15OperParam);
            p.AddDouble("p15UnitsCount", rec.p15UnitsCount);
            p.AddDouble("p15DurationPreOper", rec.p15DurationPreOper);
            p.AddDouble("p15DurationPostOper", rec.p15DurationPostOper);
            p.AddDouble("p15DurationOper", rec.p15DurationOper);


            return _db.SaveRecord("p15ClientOper", p.getDynamicDapperPars(), rec);
        }
    }
}
