using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij02PersonBL
    {
        public BO.j02Person Load(int pid);
        BO.COM.GetInteger LoadPersonalP28ID(int intPID);

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
            return "SELECT a.*,"+_db.GetSQL1_Ocas("j02")+ ",j04.j04Name,p28.p28Name,j03.j03Login,j03.j03ID,dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner FROM "+ BL.TheEntities.ByPrefix("j02").SqlFrom;
        }
        public BO.j02Person Load(int intPID)
        {
            return _db.Load<BO.j02Person>(GetSQL1()+" WHERE a.j02ID=@pid",new { pid = intPID });
        }
        public BO.COM.GetInteger LoadPersonalP28ID(int intPID)
        {
            return _db.Load<BO.COM.GetInteger>("SELECT p28ID as Value FROM j02Person WHERE j02ID=@pid", new { pid = intPID });
        }

        public IEnumerable<BO.j02Person>GetList(BO.myQuery mq)
        {            
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq,_mother.CurrentUser);
            return _db.GetList<BO.j02Person>(fq.FinalSql,fq.Parameters);
        }

        private bool ValidateBeforeSave(BO.j02Person rec)
        {
            if (string.IsNullOrEmpty(rec.j02FirstName))
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit [Jméno]."); return false;
            }
            if (string.IsNullOrEmpty(rec.j02LastName))
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit [Příjmení]."); return false;
            }

            return true;
        }

        public int Save(BO.j02Person rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.j02ID);
           
            p.AddInt("p28ID",rec.p28ID,true);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddString("j02FirstName", rec.j02FirstName);
            p.AddString("j02LastName", rec.j02LastName);
            p.AddString("j02TitleBeforeName", rec.j02TitleBeforeName);
            p.AddString("j02TitleAfterName", rec.j02TitleAfterName);
            p.AddString("j02Email", rec.j02Email);
      
            p.AddString("j02Tel1", rec.j02Tel1);
            p.AddString("j02Tel2", rec.j02Tel2);
            p.AddString("j02JobTitle", rec.j02JobTitle);
            
            return _db.SaveRecord("j02Person", p.getDynamicDapperPars(),rec);
        }

        

    }
}
