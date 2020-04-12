using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip13TpvBL
    {
        public BO.p13Tpv Load(int pid);
        public IEnumerable<BO.p13Tpv> GetList(BO.myQuery mq);
        public int Save(BO.p13Tpv rec);
    }
    class p13TpvBL : Ip13TpvBL
    {
        private string GetSQL1()
        {
            return "SELECT a.*," + DL.DbHandler.GetSQL1_Ocas("p13") + " FROM p13Tpv a";
        }
        public BO.p13Tpv Load(int pid)
        {
            return DL.DbHandler.Load<BO.p13Tpv>(string.Format("{0} WHERE a.p13ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p13Tpv> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return DL.DbHandler.GetList<BO.p13Tpv>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p13Tpv rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.p13ID);
            p.Add("p13Name", rec.p13Name);
            p.Add("p13Code", rec.p13Code);
            p.Add("p13Memo", rec.p13Memo);


            return DL.DbHandler.SaveRecord("p13Tpv", p, rec);
        }
    }
}
