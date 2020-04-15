using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip13MasterTpvBL
    {
        public BO.p13MasterTpv Load(int pid);
        public IEnumerable<BO.p13MasterTpv> GetList(BO.myQuery mq);
        public int Save(BO.p13MasterTpv rec);
    }
    class p13MasterTpvBL : Ip13MasterTpvBL
    {
        private BO.RunningUser _cUser;
        public p13MasterTpvBL(BO.RunningUser cUser)
        {
            _cUser = cUser;
        }
        private string GetSQL1()
        {
            return "SELECT a.*," + DL.DbHandler.GetSQL1_Ocas("p13") + " FROM p13MasterTpv a";
        }
        public BO.p13MasterTpv Load(int pid)
        {
            return DL.DbHandler.Load<BO.p13MasterTpv>(string.Format("{0} WHERE a.p13ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p13MasterTpv> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return DL.DbHandler.GetList<BO.p13MasterTpv>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p13MasterTpv rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.p13ID);
            p.Add("p13Name", rec.p13Name);
            p.Add("p13Code", rec.p13Code);
            p.Add("p13Memo", rec.p13Memo);


            return DL.DbHandler.SaveRecord(_cUser,"p13MasterTpv", p, rec);
        }
    }
}
