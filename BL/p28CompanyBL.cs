using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip28CompanyBL
    {
        public BO.p28Company Load(int pid);
        public IEnumerable<BO.p28Company> GetList(BO.myQuery mq);
        public int Save(BO.p28Company rec, BO.j02Person recFirstPerson);        
        public bool UpdateCloudID(int intP28ID, string strCloudID);
    }
    class p28CompanyBL : BaseBL,Ip28CompanyBL
    {        
        public p28CompanyBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner," + _db.GetSQL1_Ocas("p28") + " FROM p28Company a";
        }
        public BO.p28Company Load(int pid)
        {
            return _db.Load<BO.p28Company>(string.Format("{0} WHERE a.p28ID=@pid", GetSQL1()),new { pid = pid });
        }
        
        public IEnumerable<BO.p28Company> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p28Company>(fq.FinalSql,fq.Parameters);
        }

        public bool UpdateCloudID(int intP28ID,string strCloudID)
        {
            if (string.IsNullOrEmpty(strCloudID) == false)
            {
                BO.COM.GetInteger c = _db.Load<BO.COM.GetInteger>("select p28ID as Value FROM p28Company WHERE p28CloudID=@cloudid AND p28ID<>@pid", new { cloudid = strCloudID, pid = intP28ID });
                if (c != null)
                {
                    _mother.CurrentUser.AddMessage("Zadané cloud-id je již obsazenou jiným subjektem.");
                    return false;
                }
            }
            if (strCloudID == "-1") strCloudID = null;
            return _db.RunSql("UPDATE p28Company SET p28CloudID=@cloudid WHERE p28ID=@pid", new { cloudid = strCloudID,pid= intP28ID });
        }

        public int Save(BO.p28Company rec,BO.j02Person recFirstPerson)
        {
            if (recFirstPerson != null)
            {
                if (String.IsNullOrEmpty(recFirstPerson.j02FirstName) || String.IsNullOrEmpty(recFirstPerson.j02LastName))
                {
                    _mother.CurrentUser.AddMessage("U kontaktní osoby musíte vyplnit [Jméno] a [Příjmení] nebo odškrtněte, že se má založit první kontaktní osoba klienta.");
                    return 0;
                }
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p28ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);            
            p.AddString("p28Name", rec.p28Name);
            p.AddString("p28ShortName", rec.p28ShortName);
            p.AddString("p28Code", rec.p28Code);            
            p.AddString("p28CloudID", rec.p28CloudID);
            p.AddString("p28RegID", rec.p28RegID);            
            p.AddString("p28VatID", rec.p28VatID);
            p.AddString("p28Street1", rec.p28Street1);
            p.AddString("p28City1", rec.p28City1);
            p.AddString("p28PostCode1", rec.p28PostCode1);
            p.AddString("p28Country1", rec.p28Country1);
            p.AddString("p28Street2", rec.p28Street2);
            p.AddString("p28City2", rec.p28City2);
            p.AddString("p28PostCode2", rec.p28PostCode2);
            p.AddString("p28Country2", rec.p28Country2);

            int intPID= _db.SaveRecord("p28Company", p.getDynamicDapperPars(),rec);

            if (intPID>0 && recFirstPerson != null)
            {
                recFirstPerson.p28ID = intPID;
                _mother.j02PersonBL.Save(recFirstPerson);
            }
            
            return intPID;
        }

    }
}
