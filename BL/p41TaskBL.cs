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
    }
    class p41TaskBL : BaseBL, Ip41TaskBL
    {
        public p41TaskBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p41") + ",b02.b02Name,p11.p11Name,dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner,p28.p28Name,p26.p26Name FROM " + BL.TheEntities.ByPrefix("p41").SqlFrom;
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
            p.AddInt("p11ID", rec.p11ID, true);
            p.AddInt("p28ID", rec.p28ID, true);
            p.AddInt("p26ID", rec.p26ID, true);
            p.AddInt("b02ID", rec.b02ID, true);
            p.AddString("p41Name", rec.p41Name);
            p.AddString("p41Code", rec.p41Code);
            p.AddString("p41Memo", rec.p41Memo);
            p.AddString("p41StockCode", rec.p41StockCode);

            p.AddDateTime("p41PlanStart", rec.p41PlanStart);
            p.AddDateTime("p41PlanEnd", rec.p41PlanEnd);
            p.AddDateTime("p41RealStart", rec.p41RealStart);
            p.AddDateTime("p41RealEnd", rec.p41RealEnd);

            p.AddDouble("p41PlanUnitsCount", rec.p41PlanUnitsCount);
            p.AddDouble("p41RealUnitsCount", rec.p41RealUnitsCount);

            p.AddInt("p41ActualRowNum", rec.p41ActualRowNum);



            return _db.SaveRecord("p41Task", p.getDynamicDapperPars(), rec);
        }

        private bool ValidateBeforeSave(BO.p41Task rec)
        {
            if (LoadByCode(rec.p41Code, rec.pid) != null)
            {
                _db.CurrentUser.AddMessage(string.Format("Zadaný kód nemůže být duplicitní s jiným produktem [{0}].", LoadByCode(rec.p41Code, rec.pid).p41Name));
                return false;
            }

            return true;
        }
    }
}
