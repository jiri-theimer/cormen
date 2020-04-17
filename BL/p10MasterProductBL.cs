using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip10MasterProductBL
    {
        public BO.p10MasterProduct Load(int pid);
        public IEnumerable<BO.p10MasterProduct> GetList(BO.myQuery mq);
        public int Save(BO.p10MasterProduct rec);
    }
    class p10MasterProductBL : Ip10MasterProductBL
    {
        private BO.RunningUser _cUser;
        
        public p10MasterProductBL(BO.RunningUser cUser)
        {
            //_matka = matka;
            _cUser = cUser;



        }
        private string GetSQL1()
        {
            return "SELECT a.*," + DL.DbHandler.GetSQL1_Ocas("p10") + ",b02.b02Name as _b02name,p13.p13Name as _p13Name,o12.o12Name as _o12Name FROM p10MasterProduct a LEFT OUTER JOIN p13MasterTpv p13 ON a.p13ID=p13.p13ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN o12Category o12 ON a.o12ID=o12.o12ID";
        }
        public BO.p10MasterProduct Load(int pid)
        {
            return DL.DbHandler.Load<BO.p10MasterProduct>(string.Format("{0} WHERE a.p10ID={1}", GetSQL1(), pid));
        }
        public BO.p10MasterProduct LoadByCode(string strCode,int intExcludePID)
        {
            return DL.DbHandler.Load<BO.p10MasterProduct>(string.Format("{0} WHERE a.p10Code LIKE @code AND a.p10ID<>@exclude", GetSQL1()),new { code = strCode, exclude = intExcludePID });
        }
        public IEnumerable<BO.p10MasterProduct> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq);
            return DL.DbHandler.GetList<BO.p10MasterProduct>(fq.FinalSql, fq.Parameters);
            
        }

        public int Save(BO.p10MasterProduct rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.p10ID);
            p.Add("p13ID", BO.BAS.TestIntAsDbKey(rec.p13ID));
            p.Add("b02ID", BO.BAS.TestIntAsDbKey(rec.b02ID));
            p.Add("o12ID", BO.BAS.TestIntAsDbKey(rec.o12ID));
            p.Add("p10Name", rec.p10Name);
            p.Add("p10Code", rec.p10Code);
            p.Add("p10Memo", rec.p10Memo);


            return DL.DbHandler.SaveRecord(_cUser,"p10MasterProduct", p, rec);
        }

        private bool ValidateBeforeSave(BO.p10MasterProduct rec)
        {
            if (LoadByCode(rec.p10Code,rec.pid) != null)
            {
                _cUser.ErrorMessage = string.Format("Zadaný kód nemůže být duplicitní s jiným záznamem [{0}].", LoadByCode(rec.p10Code, rec.pid).p10Name);
                return false;
            }

            return true;
        }
    }
}
