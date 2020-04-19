using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip14MasterOperBL
    {
        public BO.p14MasterOper Load(int pid);
        public IEnumerable<BO.p14MasterOper> GetList(BO.myQuery mq);
        public int Save(BO.p14MasterOper rec);
    }
    class p14MasterOperBL : Ip14MasterOperBL
    {
        private BO.RunningUser _cUser;
        public p14MasterOperBL(BO.RunningUser cUser)
        {
            _cUser = cUser;
        }
        private string GetSQL1()
        {
            return "SELECT a.*," + DL.DbHandler.GetSQL1_Ocas("p14") + " FROM p14MasterOper a";
        }
        public BO.p14MasterOper Load(int pid)
        {
            return DL.DbHandler.Load<BO.p14MasterOper>(string.Format("{0} WHERE a.p14ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p14MasterOper> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return DL.DbHandler.GetList<BO.p14MasterOper>(fq.FinalSql, fq.Parameters);

        }
        

        public int Save(BO.p14MasterOper rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.p14ID);
            p.Add("p13ID", BO.BAS.TestIntAsDbKey(rec.p13ID));
            p.Add("p14Name", rec.p14Name);
            p.Add("p14MaterialCode", rec.p14MaterialCode);
            p.Add("p14MaterialName", rec.p14MaterialName);
            p.Add("p14RowNum", rec.p14RowNum);
            p.Add("p14OperCode", rec.p14OperCode);
            p.Add("p14OperNum", rec.p14OperNum);
            p.Add("p14OperParam", rec.p14OperParam);
            p.Add("p14UnitsCount", rec.p14UnitsCount);
            p.Add("p14DurationPreOper", rec.p14DurationPreOper);
            p.Add("p14DurationPostOper", rec.p14DurationPostOper);
            p.Add("p14DurationOper", rec.p14DurationOper);


            return DL.DbHandler.SaveRecord(_cUser, "p14MasterOper", p, rec);
        }
    }
}
