using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij03UserBL
    {
        public BO.j03User Load(int pid);
        public BO.j03User LoadByLogin(string strLogin);
        
        public IEnumerable<BO.j03User> GetList(BO.myQuery mq);
        public int Save(BO.j03User rec);

    }
    class j03UserBL : BaseBL, Ij03UserBL
    {
        public j03UserBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("j03") + ",j04.j04Name,p28.p28Name,j02.j02LastName+' '+j02.j02FirstName+ISNULL(' '+j02.j02TitleBeforeName,' ') as fullname_desc,j02.j02Email FROM j03User a INNER JOIN j02Person j02 ON a.j02ID=a.j02ID INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON j02.p28ID=p28.p28ID";
        }
        public BO.j03User Load(int intPID)
        {
            return _db.Load<BO.j03User>(GetSQL1() + " WHERE a.j03ID=@pid", new { pid = intPID });
        }
       
        public BO.j03User LoadByLogin(string strLogin)
        {
            return _db.Load<BO.j03User>(GetSQL1() + " WHERE a.j03Login LIKE @login", new { login = strLogin });
        }
        public IEnumerable<BO.j03User> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return _db.GetList<BO.j03User>(fq.FinalSql, fq.Parameters);
        }

        public int Save(BO.j03User rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.j03ID);
            p.Add("j02ID", BO.BAS.TestIntAsDbKey(rec.j02ID));
            p.Add("j04ID", BO.BAS.TestIntAsDbKey(rec.j04ID));
            p.Add("j03IsMustChangePassword", rec.j03IsMustChangePassword, System.Data.DbType.Boolean);            
            p.Add("j03Login", rec.j03Login);         
            p.Add("j03AccessFailedCount", rec.j03AccessFailedCount);
            p.Add("j03ModalDialogFlag", rec.j03ModalDialogFlag);
            if (!String.IsNullOrEmpty(rec.j03PasswordHash))
            {
                p.Add("j03PasswordHash", rec.j03PasswordHash);
            }


            return _db.SaveRecord("j03User", p, rec);
        }
    }
}
