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
    class b02StatusBL : BaseBL,Ib02StatusBL
    {        
        public b02StatusBL(BL.Factory mother) : base(mother)
        {
       
        }
        
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("b02") + " FROM " + BL.TheEntities.ByPrefix("b02").SqlFrom;
        }
        public BO.b02Status Load(int pid)
        {
            return _db.Load<BO.b02Status>(string.Format("{0} WHERE a.b02ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.b02Status> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.b02Status>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.b02Status rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.b02ID);         
            p.Add("b02Name", rec.b02Name);
            p.Add("b02Code", rec.b02Code);
            p.Add("b02Entity", rec.b02Entity);


            return _db.SaveRecord("b02Status", p, rec);
        }
    }
}
