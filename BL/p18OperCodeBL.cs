using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip18OperCodeBL
    {
        public BO.p18OperCode Load(int pid);
        public IEnumerable<BO.p18OperCode> GetList(BO.myQuery mq);
        public int Save(BO.p18OperCode rec);
    }
    class p18OperCodeBL : BaseBL, Ip18OperCodeBL
    {

        public p18OperCodeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,p25.p25Name," + _db.GetSQL1_Ocas("p18") + " FROM p18OperCode a INNER JOIN p25MszType p25 ON a.p25ID=p25.p25ID";
        }
        public BO.p18OperCode Load(int pid)
        {
            return _db.Load<BO.p18OperCode>(string.Format("{0} WHERE a.p18ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p18OperCode> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p18OperCode>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p18OperCode rec)
        {
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.p18ID);
            p.AddString("p18Name", rec.p18Name);
            p.AddString("p18Code", rec.p18Code);
            p.AddString("p18Memo", rec.p18Memo);


            return _db.SaveRecord("p18OperCode", p.getDynamicDapperPars(), rec);
        }
    }
}
