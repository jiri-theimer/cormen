using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Io23DocBL
    {
        public BO.o23Doc Load(int pid);
        public IEnumerable<BO.o23Doc> GetList(BO.myQuery mq);
        public int Save(BO.o23Doc rec);
    }
    class o23DocBL:Io23DocBL
    {
        private BO.RunningUser _cUser;
        public o23DocBL(BO.RunningUser cUser)
        {
            _cUser = cUser;
        }
        private string GetSQL1()
        {
            return "SELECT a.*," + DL.DbHandler.GetSQL1_Ocas("o23") + ",o12.o12Name as _o12Name,b02.b02Name as _b02Name FROM o23Doc a LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID";
        }
        public BO.o23Doc Load(int pid)
        {
            return DL.DbHandler.Load<BO.o23Doc>(string.Format("{0} WHERE a.o23ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.o23Doc> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return DL.DbHandler.GetList<BO.o23Doc>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.o23Doc rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.o23ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _cUser.pid;
            p.Add("j02ID_Owner", BO.BAS.TestIntAsDbKey(rec.j02ID_Owner));
            p.Add("o23RecordPid", BO.BAS.TestIntAsDbKey(rec.o23RecordPid));
            p.Add("o23Entity", rec.o23Entity);
            p.Add("o12ID", BO.BAS.TestIntAsDbKey(rec.o12ID));
            p.Add("b02ID", BO.BAS.TestIntAsDbKey(rec.b02ID));
            p.Add("o23Date", rec.o23Date);
            p.Add("o23Name", rec.o23Name);
            p.Add("o23Code", rec.o23Code);
            p.Add("o23Entity", rec.o23Entity);
            p.Add("o23Memo", rec.o23Memo);


            return DL.DbHandler.SaveRecord(_cUser, "o23Doc", p, rec);
        }
    }
}
