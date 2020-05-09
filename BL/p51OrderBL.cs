using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip51OrderBL
    {
        public BO.p51Order Load(int pid);
        public IEnumerable<BO.p51Order> GetList(BO.myQuery mq);
        public int Save(BO.p51Order rec);
    }
    class p51OrderBL : BaseBL, Ip51OrderBL
    {
        public p51OrderBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p51") + ",b02.b02Name,dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner,p28.p28Name,p26.p26Name FROM p51Order a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN p26Msz p26 ON a.p26ID=p26.p26ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID";
        }
        public BO.p51Order Load(int pid)
        {
            return _db.Load<BO.p51Order>(string.Format("{0} WHERE a.p51ID=@pid", GetSQL1(), new { pid = pid }));
        }
        public BO.p51Order LoadByCode(string strCode, int intExcludePID)
        {
            return _db.Load<BO.p51Order>(string.Format("{0} WHERE a.p51Code LIKE @code AND a.p51ID<>@exclude", GetSQL1()), new { code = strCode, exclude = intExcludePID });
        }
        public IEnumerable<BO.p51Order> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p51Order>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p51Order rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p51ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.pid;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);            
            p.AddInt("p28ID", rec.p28ID, true);
            p.AddInt("p26ID", rec.p26ID, true);
            p.AddInt("b02ID", rec.b02ID, true);
            p.AddBool("p51IsDraft", rec.p51IsDraft);
            p.AddString("p51Name", rec.p51Name);
            p.AddString("p51Code", rec.p51Code);
            p.AddString("p51Memo", rec.p51Memo);
            p.AddString("p51CodeByClient", rec.p51CodeByClient);
           
            p.AddDateTime("p51Date", rec.p51Date);
            p.AddDateTime("p51DateDelivery", rec.p51DateDelivery);

          



            return _db.SaveRecord("p51Order", p.getDynamicDapperPars(), rec);
        }

        private bool ValidateBeforeSave(BO.p51Order rec)
        {
            if (LoadByCode(rec.p51Code, rec.pid) != null)
            {
                _db.CurrentUser.AddMessage(string.Format("Zadaný kód nemůže být duplicitní s jinou objednávkou [{0}].", LoadByCode(rec.p51Code, rec.pid).p51Name));
                return false;
            }

            return true;
        }
    }
}

