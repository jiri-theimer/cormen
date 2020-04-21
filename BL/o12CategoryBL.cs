using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Io12CategoryBL
    {
        public BO.o12Category Load(int pid);
        public IEnumerable<BO.o12Category> GetList(BO.myQuery mq);
        public int Save(BO.o12Category rec);
    }
    class o12CategoryBL : Io12CategoryBL
    {
        private DL.DbHandler _db;
        public o12CategoryBL(DL.DbHandler db)
        {
            _db = db;
        }
       
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("o12") + " FROM o12Category a";
        }
        public BO.o12Category Load(int pid)
        {
            return _db.Load<BO.o12Category>(string.Format("{0} WHERE a.o12ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.o12Category> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return _db.GetList<BO.o12Category>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.o12Category rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.o12ID);
            p.Add("o12Name", rec.o12Name);
            p.Add("o12Code", rec.o12Code);
            p.Add("o12Entity", rec.o12Entity);


            return _db.SaveRecord(_db.CurrentUser,"o12Category", p, rec);
        }
    }
}
