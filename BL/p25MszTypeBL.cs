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
        public bool Copy_p18OperCode(int p25id_dest, int p25id_source);
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
            return _db.Load<BO.p25MszType>(string.Format("{0} WHERE a.p25ID=@pid", GetSQL1()), new { pid = pid });
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

        public bool Copy_p18OperCode(int p25id_dest,int p25id_source)
        {
            string s = "INSERT INTO p18OperCode(p25ID, p18Code, p18Name, p19ID, p18UnitsCount, p18DurationPreOper, p18DurationOper, p18DurationPostOper, p18Lang1, p18Lang2, p18Lang3, p18Lang4)";
            s += " SELECT @p25id_dest,p18Code, p18Name, p19ID, p18UnitsCount, p18DurationPreOper, p18DurationOper, p18DurationPostOper, p18Lang1, p18Lang2, p18Lang3, p18Lang4";
            s += " FROM p18OperCode WHERE p25ID=@p25id_source";
            s += " AND p18Code NOT IN (select p18Code FROM p18OperCode WHERE p25ID=@p25id_dest)";
            return _db.RunSql(s, new { p25id_dest = p25id_dest, p25id_source = p25id_source });
        }
    }
}
