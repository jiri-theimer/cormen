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
            return "SELECT a.*," + _db.GetSQL1_Ocas("b02") + " FROM b02Status a";
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
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.b02ID);         
            p.AddString("b02Name", rec.b02Name);
            p.AddString("b02Code", rec.b02Code);
            p.AddString("b02Entity", rec.b02Entity);
            p.AddInt("b02Ordinary", rec.b02Ordinary);
            p.AddString("b02Memo", rec.b02Memo);

            return _db.SaveRecord("b02Status", p.getDynamicDapperPars(), rec);
        }
    }
}
