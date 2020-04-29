using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij02PersonBL
    {
        public BO.j02Person Load(int pid);
        
        public IEnumerable<BO.j02Person> GetList(BO.myQuery mq);
        public int Save(BO.j02Person rec);
       
    }
    class j02PersonBL : BaseBL,Ij02PersonBL
    {
        public j02PersonBL(BL.Factory mother):base(mother)
        {
           
        }
        
       
        private string GetSQL1()
        {
            return "SELECT a.*,"+_db.GetSQL1_Ocas("j02")+",j04.j04Name,p28.p28Name,j03.j03Login,j03.j03ID FROM j02Person a LEFT OUTER JOIN j03User j03 ON a.j02ID=j03.j02ID LEFT OUTER JOIN j04UserRole j04 ON j03.j04ID=j04.j04ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID";
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
           

          
            p.Add("p28ID", BO.BAS.TestIntAsDbKey(rec.p28ID));
            p.Add("j02FirstName", rec.j02FirstName);
            p.Add("j02LastName", rec.j02LastName);
            p.Add("j02TitleBeforeName", rec.j02TitleBeforeName);
            p.Add("j02TitleAfterName", rec.j02TitleAfterName);
            p.Add("j02Email", rec.j02Email);
      
            p.Add("j02Tel1", rec.j02Tel1);
            p.Add("j02Tel2", rec.j02Tel2);
            
            
            return _db.SaveRecord("j02Person", p,rec);
        }

        

    }
}
