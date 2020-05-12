using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip11ClientProductBL
    {
        public BO.p11ClientProduct Load(int pid);
        public IEnumerable<BO.p11ClientProduct> GetList(BO.myQuery mq);
        public int Save(BO.p11ClientProduct rec);
    }
    class p11ClientProductBL : BaseBL, Ip11ClientProductBL
    {
        public p11ClientProductBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p11") + ",b02.b02Name,p12.p12Name,p12.p12Code,p21.p21Name,p21.p21Code,p10.p10Name,p10.p10Code,p20.p20Code FROM p11ClientProduct a LEFT OUTER JOIN p12ClientTpv p12 ON a.p12ID=p12.p12ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p21License p21 ON a.p21ID=p21.p21ID LEFT OUTER JOIN p10MasterProduct p10 ON a.p10ID_Master=p10.p10ID LEFT OUTER JOIN p20Unit p20 ON a.p20ID=p20.p20ID";
        }
        public BO.p11ClientProduct Load(int pid)
        {
            return _db.Load<BO.p11ClientProduct>(string.Format("{0} WHERE a.p11ID=@pid", GetSQL1()), new { pid = pid });
        }
        public BO.p11ClientProduct LoadByCode(string strCode, int intExcludePID)
        {
            return _db.Load<BO.p11ClientProduct>(string.Format("{0} WHERE a.p11Code LIKE @code AND a.p11ID<>@exclude", GetSQL1()), new { code = strCode, exclude = intExcludePID });
        }
        public IEnumerable<BO.p11ClientProduct> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p11ClientProduct>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p11ClientProduct rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p11ID);
            p.AddInt("p12ID", rec.p12ID, true);
            p.AddInt("b02ID", rec.b02ID, true);
            p.AddInt("p20ID", rec.p20ID, true);
            p.AddString("p11Name", rec.p11Name);
            p.AddString("p11Code", rec.p11Code);
            p.AddString("p11Memo", rec.p11Memo);
            p.AddDouble("p11UnitPrice", rec.p11UnitPrice);


            return _db.SaveRecord("p11ClientProduct", p.getDynamicDapperPars(), rec);
        }

        private bool ValidateBeforeSave(BO.p11ClientProduct rec)
        {
            if (LoadByCode(rec.p11Code, rec.pid) != null)
            {
                _db.CurrentUser.AddMessage(string.Format("Zadaný kód nemůže být duplicitní s jiným produktem [{0}].", LoadByCode(rec.p11Code, rec.pid).p11Name));
                return false;
            }

            return true;
        }
    }
}
