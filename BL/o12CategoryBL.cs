using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Io12CategoryBL
    {
        public BO.o12Category Load(int pid);
        public IEnumerable<BO.o12Category> GetList(BO.myQuery mq);
        public int Save(BO.o12Category rec);
    }
    class o12CategoryBL : Io12CategoryBL
    {
        private BO.RunningUser _cUser;
        public o12CategoryBL(BO.RunningUser cUser)
        {
            _cUser = cUser;
        }
        private string GetSQL1()
        {
            return "SELECT a.*," + DL.DbHandler.GetSQL1_Ocas("o12") + " FROM o12Category a";
        }
        public BO.o12Category Load(int pid)
        {
            return DL.DbHandler.Load<BO.o12Category>(string.Format("{0} WHERE a.o12ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.o12Category> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return DL.DbHandler.GetList<BO.o12Category>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.o12Category rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.o12ID);
            p.Add("o12Name", rec.o12Name);
            p.Add("o12Code", rec.o12Code);
            p.Add("o12Entity", rec.o12Entity);


            return DL.DbHandler.SaveRecord(_cUser,"o12Category", p, rec);
        }
    }
}
