using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip21LicenseBL
    {
        public BO.p21License Load(int pid);
        public IEnumerable<BO.p21License> GetList(BO.myQuery mq);
        public int Save(BO.p21License rec);
    }
    class p21LicenseBL:Ip21LicenseBL
    {
        private string GetSQL1()
        {
            return "SELECT a.*," + DL.DbHandler.GetSQL1_Ocas("p21") + ",b02.b02Name as _b02name,p28.p28Name as _p28Name FROM p21License a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID";
        }
        public BO.p21License Load(int pid)
        {
            return DL.DbHandler.Load<BO.p21License>(string.Format("{0} WHERE a.p21ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p21License> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return DL.DbHandler.GetList<BO.p21License>(fq.FinalSql, fq.Parameters);
            
        }

        public int Save(BO.p21License rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.p21ID);
            p.Add("p28ID", BO.BAS.TestIntAsDbKey(rec.p28ID));
            p.Add("b02ID", BO.BAS.TestIntAsDbKey(rec.b02ID));
            p.Add("p21Name", rec.p21Name);
            p.Add("p21Code", rec.p21Code);
            p.Add("p21Memo", rec.p21Memo);


            return DL.DbHandler.SaveRecord("p21License", p, rec);
        }
    }
}

