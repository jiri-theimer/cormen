using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip31CapTemplateBL
    {
        public BO.p31CapTemplate Load(int pid);
        public IEnumerable<BO.p31CapTemplate> GetList(BO.myQuery mq);
        public int Save(BO.p31CapTemplate rec);
    }
    class p31CapTemplateBL : BaseBL, Ip31CapTemplateBL
    {

        public p31CapTemplateBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p31") + " FROM p31CapTemplate a";
        }
        public BO.p31CapTemplate Load(int pid)
        {
            return _db.Load<BO.p31CapTemplate>(string.Format("{0} WHERE a.p31ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p31CapTemplate> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p31CapTemplate>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p31CapTemplate rec)
        {
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.p31ID);
            p.AddString("p31Name", rec.p31Name);
         

            return _db.SaveRecord("p31CapTemplate", p.getDynamicDapperPars(), rec);
        }
    }
}
