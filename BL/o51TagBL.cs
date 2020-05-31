using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Io51TagBL
    {
        public BO.o51Tag Load(int pid);
        public IEnumerable<BO.o51Tag> GetList(BO.myQuery mq);
        public int Save(BO.o51Tag rec);
    }
    class o51TagBL : BaseBL, Io51TagBL
    {
        public o51TagBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("o51") + " FROM o51Tag a";
        }
        public BO.o51Tag Load(int pid)
        {
            return _db.Load<BO.o51Tag>(string.Format("{0} WHERE a.o51ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.o51Tag> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.o51Tag>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.o51Tag rec)
        {
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.o51ID);
            p.AddString("o51Name", rec.o51Name);
            p.AddString("o51Code", rec.o51Code);
            p.AddString("o51Entity", rec.o51Entity);


            return _db.SaveRecord("o51Tag", p.getDynamicDapperPars(), rec);
        }
    }
}
