using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ib02StatusBL
    {
        public BO.b02Status Load(int pid);
        public IEnumerable<BO.b02Status> GetList(BO.myQuery mq);
        public int Save(BO.b02Status rec);
    }
    class b02StatusBL : Ib02StatusBL
    {
        private DL.DbHandler _db;
        public b02StatusBL(DL.DbHandler db)
        {
            _db = db;
        }
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("b02") + " FROM b02Status a";
        }
        public BO.b02Status Load(int pid)
        {
            return _db.Load<BO.b02Status>(string.Format("{0} WHERE a.b02ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.b02Status> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return _db.GetList<BO.b02Status>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.b02Status rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.b02ID);         
            p.Add("b02Name", rec.b02Name);
            p.Add("b02Code", rec.b02Code);
            p.Add("b02Entity", rec.b02Entity);


            return _db.SaveRecord(_db.CurrentUser,"b02Status", p, rec);
        }
    }
}
