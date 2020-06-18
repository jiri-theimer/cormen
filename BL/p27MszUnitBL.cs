using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip27MszUnitBL
    {
        public BO.p27MszUnit Load(int pid);
        public BO.p27MszUnit LoadByCode(string strCode, int intExcludePID);
        public IEnumerable<BO.p27MszUnit> GetList(BO.myQuery mq);
        public int Save(BO.p27MszUnit rec);
        
    }
    class p27MszUnitBL : BaseBL, Ip27MszUnitBL
    {

        public p27MszUnitBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,p25.p25Name,p31.p31Name,p27Name+' ('+p27Code+')' as NamePlusCode," + _db.GetSQL1_Ocas("p27") + " FROM p27MszUnit a INNER JOIN p25MszType p25 ON a.p25ID=p25.p25ID LEFT OUTER JOIN p31CapacityFond p31 ON a.p31ID=p31.p31ID";
        }
        public BO.p27MszUnit Load(int pid)
        {
            return _db.Load<BO.p27MszUnit>(string.Format("{0} WHERE a.p27ID=@pid", GetSQL1()), new { pid = pid });
        }
        public BO.p27MszUnit LoadByCode(string strCode, int intExcludePID)
        {
            return _db.Load<BO.p27MszUnit>(string.Format("{0} WHERE a.p27Code LIKE @code AND a.p27ID<>@exclude", GetSQL1()), new { code = strCode, exclude = intExcludePID });
        }
        public IEnumerable<BO.p27MszUnit> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p27MszUnit>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p27MszUnit rec)
        {
            if (rec.p25ID == 0)
            {
                _mother.CurrentUser.AddMessage("Chybí vyplnit typ zařízení.");
                return 0;
            }
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.p27ID);
            p.AddInt("p25ID", rec.p25ID,true);
            p.AddInt("p31ID", rec.p31ID, true);
            p.AddString("p27Name", rec.p27Name);
            p.AddString("p27Code", rec.p27Code);
            p.AddDouble("p27Capacity", rec.p27Capacity);


            return _db.SaveRecord("p27MszUnit", p.getDynamicDapperPars(), rec);
        }

       
    }
}
