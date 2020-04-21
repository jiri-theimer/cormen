using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij02PersonBL
    {
        public BO.j02Person Load(int pid);
        public BO.j02Person LoadByLogin(string strLogin);
        public IEnumerable<BO.j02Person> GetList(BO.myQuery mq);
        public int Save(BO.j02Person rec);
       
    }
    class j02PersonBL : Ij02PersonBL
    {
        private DL.DbHandler _db;
        public j02PersonBL(DL.DbHandler db)
        {
            _db = db;
        }
       
        private string GetSQL1()
        {
            return "SELECT a.*,"+_db.GetSQL1_Ocas("j02")+",j04.j04Name as _j04Name,p28.p28Name as _p28Name FROM j02Person a LEFT OUTER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID";
        }
        public BO.j02Person Load(int intPID)
        {
            return _db.Load<BO.j02Person>(GetSQL1()+" WHERE a.j02ID=@pid",new { pid = intPID });
        }
        public BO.j02Person LoadByLogin(string strLogin)
        {           
            return _db.Load<BO.j02Person>(GetSQL1()+" WHERE a.j02Login LIKE @login", new { login = strLogin });
        }
        public IEnumerable<BO.j02Person>GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return _db.GetList<BO.j02Person>(fq.FinalSql,fq.Parameters);
        }

        public int Save(BO.j02Person rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.j02ID);
            p.Add("j02IsUser", rec.j02IsUser, System.Data.DbType.Boolean);
            p.Add("j02IsMustChangePassword", rec.j02IsMustChangePassword, System.Data.DbType.Boolean);
            p.Add("j04ID", BO.BAS.TestIntAsDbKey(rec.j04ID));
            p.Add("p28ID", BO.BAS.TestIntAsDbKey(rec.p28ID));
            p.Add("j02FirstName", rec.j02FirstName);
            p.Add("j02LastName", rec.j02LastName);
            p.Add("j02TitleBeforeName", rec.j02TitleBeforeName);
            p.Add("j02TitleAfterName", rec.j02TitleAfterName);
            p.Add("j02Email", rec.j02Email);
            p.Add("j02Login", rec.j02Login);
            p.Add("j02Tel1", rec.j02Tel1);
            p.Add("j02Tel2", rec.j02Tel2);
            p.Add("j02AccessFailedCount", rec.j02AccessFailedCount);
            if (!String.IsNullOrEmpty(rec.j02PasswordHash))
            {
                p.Add("j02PasswordHash", rec.j02PasswordHash);
            }
            

            return _db.SaveRecord(_db.CurrentUser,"j02Person", p,rec);
        }

        

    }
}
