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

        public int Save(BO.p13MasterTpv rec,string strGUID)
        {
            //var p = new Dapper.DynamicParameters();
            //p.Add("pid", rec.p13ID);
            //p.Add("p13Name", rec.p13Name);
            //p.Add("p13Code", rec.p13Code);
            //p.Add("p13Memo", rec.p13Memo);


            //return DL.DbHandler.SaveRecord(_cUser,"p13MasterTpv", p, rec);

            BO.p85Tempbox cP85;
            var tempBL = new BL.p85TempboxBL(_cUser);
            cP85 = new BO.p85Tempbox() { p85RecordPid = rec.pid, p85GUID = strGUID, p85Prefix = "p13", p85FreeText01 = rec.p13Name, p85FreeText02 = rec.p13Code, p85Message = rec.p13Memo, p85FreeDate01 = rec.ValidFrom, p85FreeDate02 = rec.ValidUntil };
            tempBL.Save(cP85);           

            var p = new Dapper.DynamicParameters();
            p.Add("userid", _cUser.pid);
            p.Add("guid", strGUID);
            p.Add("pid_ret", rec.pid, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
            p.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

            string s1 = DL.DbHandler.RunSp("p13_save", ref p);
            if (s1 == "1")
            {
                return p.Get<int>("pid_ret");
            }
            else
            {
                _cUser.ErrorMessage = s1;
                return 0;
            }

        }
    }
}
