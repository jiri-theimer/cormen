using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip28CompanyBL
    {
        public BO.p28Company Load(int pid);
        public IEnumerable<BO.p28Company> GetList(BO.myQuery mq);
        public int Save(BO.p28Company rec);
    }
    class p28CompanyBL : Ip28CompanyBL
    {
        private string GetSQL1()
        {
            return "SELECT a.*," + DL.DbHandler.GetSQL1_Ocas("p28") + " FROM p28Company a";
        }
        public BO.p28Company Load(int pid)
        {
            return DL.DbHandler.Load<BO.p28Company>(string.Format("{0} WHERE a.p28ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p28Company> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return DL.DbHandler.GetList<BO.p28Company>(fq.FinalSql,fq.Parameters);
        }

        public int Save(BO.p28Company rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.p28ID);            
            p.Add("p28Name", rec.p28Name);
            p.Add("p28ShortName", rec.p28ShortName);
            p.Add("p28Code", rec.p28Code);
            p.Add("p28RegID", rec.p28RegID);
            p.Add("p28VatID", rec.p28VatID);
            p.Add("p28Street1", rec.p28Street1);
            p.Add("p28City1", rec.p28City1);
            p.Add("p28PostCode1", rec.p28PostCode1);
            p.Add("p28Country1", rec.p28Country1);
            p.Add("p28Street2", rec.p28Street2);
            p.Add("p28City2", rec.p28City2);
            p.Add("p28PostCode2", rec.p28PostCode2);
            p.Add("p28Country2", rec.p28Country2);

            return DL.DbHandler.SaveRecord("p28Company", p,rec);
        }

    }
}
