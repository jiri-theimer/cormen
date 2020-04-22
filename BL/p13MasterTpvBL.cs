using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip13MasterTpvBL
    {
        public BO.p13MasterTpv Load(int pid);
        public IEnumerable<BO.p13MasterTpv> GetList(BO.myQuery mq);
        public int Save(BO.p13MasterTpv rec, string strGUID);
    }
    class p13MasterTpvBL : BaseBL,Ip13MasterTpvBL
    {     
        public p13MasterTpvBL(BL.Factory mother):base(mother)
        {
           
        }
       
       
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p13") + " FROM p13MasterTpv a";
        }
        public BO.p13MasterTpv Load(int pid)
        {

            return _db.Load<BO.p13MasterTpv>(string.Format("{0} WHERE a.p13ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p13MasterTpv> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return _db.GetList<BO.p13MasterTpv>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p13MasterTpv rec,string strGUID)
        {
            
            BO.p85Tempbox cP85;
            
            cP85 = new BO.p85Tempbox() { p85RecordPid = rec.pid, p85GUID = strGUID, p85Prefix = "p13", p85FreeText01 = rec.p13Name, p85FreeText02 = rec.p13Code, p85Message = rec.p13Memo, p85FreeDate01 = rec.ValidFrom, p85FreeDate02 = rec.ValidUntil };
            _mother.p85TempboxBL.Save(cP85);           

            var p = new Dapper.DynamicParameters();
            p.Add("userid", _db.CurrentUser.pid);
            p.Add("guid", strGUID);
            p.Add("pid_ret", rec.pid, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            p.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            string s1 = _db.RunSp("p13_save", ref p);
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
