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

        public int Save(BO.p28Company rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p28ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.pid;            
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("p28TypeFlag", rec.p28TypeFlag);
            p.AddString("p28Name", rec.p28Name);
            p.AddString("p28ShortName", rec.p28ShortName);
            p.AddString("p28Code", rec.p28Code);
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

            return _db.SaveRecord("p28Company", p.getDynamicDapperPars(),rec);
        }

    }
}
