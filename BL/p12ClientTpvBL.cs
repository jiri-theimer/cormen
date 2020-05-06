using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip12ClientTpvBL
    {
        public BO.p12ClientTpv Load(int pid);
        public IEnumerable<BO.p12ClientTpv> GetList(BO.myQuery mq);
        public int Save(BO.p12ClientTpv rec, string strGUID);
    }
    class p12ClientTpvBL : BaseBL, Ip12ClientTpvBL
    {
        public p12ClientTpvBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,p21.p21Name," + _db.GetSQL1_Ocas("p12") + " FROM p12ClientTpv a LEFT OUTER JOIN p21License p21 ON a.p21ID=p21.p21ID";
        }
        public BO.p12ClientTpv Load(int pid)
        {

            return _db.Load<BO.p12ClientTpv>(string.Format("{0} WHERE a.p12ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p12ClientTpv> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p12ClientTpv>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p12ClientTpv rec, string strGUID)
        {

            BO.p85Tempbox cP85;

            cP85 = new BO.p85Tempbox() { p85RecordPid = rec.pid, p85GUID = strGUID, p85Prefix = "p12", p85FreeText01 = rec.p12Name, p85FreeText02 = rec.p12Code, p85Message = rec.p12Memo, p85FreeDate01 = rec.ValidFrom, p85FreeDate02 = rec.ValidUntil };
            _mother.p85TempboxBL.Save(cP85);

            var p = new Dapper.DynamicParameters();
            p.Add("userid", _db.CurrentUser.pid);
            p.Add("guid", strGUID);
            p.Add("pid_ret", rec.pid, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            p.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            string s1 = _db.RunSp("p12_save", ref p);
            if (s1 == "1")
            {
                return p.Get<int>("pid_ret");
            }
            else
            {

                return 0;
            }

        }
    }
}
