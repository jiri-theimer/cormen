using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip41TaskBL
    {
        public BO.p41Task Load(int pid);
        public IEnumerable<BO.p41Task> GetList(BO.myQuery mq);
        public int Save(BO.p41Task rec);
        public bool ValidateBeforeSave(BO.p41Task rec);
    }
    class p41TaskBL : BaseBL, Ip41TaskBL
    {
        public p41TaskBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p41") + ",b02.b02Name,p11.p11Name,dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner,p28.p28Name,p26.p26Name,p52.p52Code,p27.p27Name,p51.p51Code FROM p41Task a INNER JOIN p52OrderItem p52 ON a.p52ID=p52.p52ID INNER JOIN p27MszUnit p27 ON a.p27ID=p27.p27ID INNER JOIN p11ClientProduct p11 ON p52.p11ID=p11.p11ID INNER JOIN p51Order p51 ON p52.p51ID=p51.p51ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p28Company p28 ON p51.p28ID=p28.p28ID LEFT OUTER JOIN p26Msz p26 ON p27.p26ID=p26.p26ID";
        }
        public BO.p41Task Load(int pid)
        {
            return _db.Load<BO.p41Task>(string.Format("{0} WHERE a.p41ID=@pid", GetSQL1()), new { pid = pid });
        }
        public BO.p41Task LoadByCode(string strCode, int intExcludePID)
        {
            return _db.Load<BO.p41Task>(string.Format("{0} WHERE a.p41Code LIKE @code AND a.p41ID<>@exclude", GetSQL1()), new { code = strCode, exclude = intExcludePID });
        }
        public IEnumerable<BO.p41Task> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p41Task>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p41Task rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p41ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("p27ID", rec.p27ID, true);
            p.AddInt("p52ID", rec.p52ID, true);            
            p.AddInt("b02ID", rec.b02ID, true);
            p.AddBool("p41IsDraft", rec.p41IsDraft);
            p.AddString("p41Name", rec.p41Name);
            p.AddString("p41Code", rec.p41Code);
            p.AddString("p41Memo", rec.p41Memo);
            p.AddString("p41StockCode", rec.p41StockCode);

            p.AddDateTime("p41PlanStart", rec.p41PlanStart);
            p.AddDateTime("p41PlanEnd", rec.p41PlanEnd);
            //p.AddDateTime("p41RealStart", rec.p41RealStart);
            //p.AddDateTime("p41RealEnd", rec.p41RealEnd);

            p.AddDouble("p41PlanUnitsCount", rec.p41PlanUnitsCount);
            //p.AddDouble("p41RealUnitsCount", rec.p41RealUnitsCount);

            


            return _db.SaveRecord("p41Task", p.getDynamicDapperPars(), rec);
        }

        public bool ValidateBeforeSave(BO.p41Task rec)
        {
            if (String.IsNullOrEmpty(rec.p41Name) || string.IsNullOrEmpty(rec.p41Code))
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit název nebo kód zakázky.");
                return false;
            }
            if (rec.p27ID==0 || rec.p52ID == 0)
            {
                _db.CurrentUser.AddMessage("Na vstupu chybí středisko nebo objednávka.");
                return false;
            }
            if (rec.p41PlanStart==null || rec.p41PlanEnd==null)
            {
                _db.CurrentUser.AddMessage("Čas plánovaného zahájení a dokončení je povinné vyplnit.");
                return false;
            }
            if (rec.p41PlanStart >= rec.p41PlanEnd)
            {
                _db.CurrentUser.AddMessage("Čas plánovaného zahájení musí být menší než čas dokončení.");
                return false;
            }
            if (LoadByCode(rec.p41Code, rec.pid) != null)
            {
                _db.CurrentUser.AddMessage(string.Format("Zadaný kód nemůže být duplicitní s jinou zákazkou [{0}].", LoadByCode(rec.p41Code, rec.pid).p41Name));
                return false;
            }

            return true;
        }
    }
}
