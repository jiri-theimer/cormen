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
    class p10MasterProductBL : BaseBL,Ip10MasterProductBL
    {

        public p10MasterProductBL(BL.Factory mother):base(mother)
        {
           
        }
       
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p10") + ",b02.b02Name,p13.p13Name,p13.p13Code,o12.o12Name,p20.p20Code FROM "+ BL.TheEntities.ByPrefix("p10").SqlFrom;
        }
        public BO.p10MasterProduct Load(int pid)
        {
            return _db.Load<BO.p10MasterProduct>(string.Format("{0} WHERE a.p10ID=@pid", GetSQL1()),new { pid = pid });
        }
        public BO.p10MasterProduct LoadByCode(string strCode,int intExcludePID)
        {
            return _db.Load<BO.p10MasterProduct>(string.Format("{0} WHERE a.p10Code LIKE @code AND a.p10ID<>@exclude", GetSQL1()),new { code = strCode, exclude = intExcludePID });
        }
        public IEnumerable<BO.p10MasterProduct> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p10MasterProduct>(fq.FinalSql, fq.Parameters);
            
        }

        public int Save(BO.p10MasterProduct rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p10ID);
            p.AddInt("p13ID",rec.p13ID,true);
            p.AddInt("b02ID", rec.b02ID,true);
            p.AddInt("o12ID", rec.o12ID,true);
            p.AddInt("p20ID", rec.p20ID, true);
            p.AddString("p10Name", rec.p10Name);
            p.AddString("p10Code", rec.p10Code);
            p.AddString("p10Memo", rec.p10Memo);
            p.AddInt("p10SwLicenseFlag", rec.p10SwLicenseFlag);


            return _db.SaveRecord("p10MasterProduct", p.getDynamicDapperPars(), rec);
        }

        private bool ValidateBeforeSave(BO.p10MasterProduct rec)
        {
            if (rec.p20ID == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit měrná jednotka."); return false;
            }
            if (LoadByCode(rec.p10Code,rec.pid) != null)
            {
                _db.CurrentUser.AddMessage(string.Format("Zadaný kód nemůže být duplicitní s jiným záznamem [{0}].", LoadByCode(rec.p10Code, rec.pid).p10Name));
                return false;
            }

            return true;
        }
    }
}
