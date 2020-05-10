using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip25MszTypeBL
    {
        public BO.p25MszType Load(int pid);
        public IEnumerable<BO.p25MszType> GetList(BO.myQuery mq);
        public int Save(BO.p25MszType rec);
    }
    class p25MszTypeBL : BaseBL, Ip25MszTypeBL
    {

        public p25MszTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p25") + " FROM p25MszType a";
        }
        public BO.p25MszType Load(int pid)
        {
            return _db.Load<BO.p25MszType>(string.Format("{0} WHERE a.p25ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p25MszType> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p25MszType>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p25MszType rec)
        {
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.p25ID);
            p.AddString("p25Name", rec.p25Name);
            p.AddString("p25Code", rec.p25Code);
            p.AddString("p25Memo", rec.p25Memo);


            return _db.SaveRecord("p25MszType", p.getDynamicDapperPars(), rec);
        }
    }
}
