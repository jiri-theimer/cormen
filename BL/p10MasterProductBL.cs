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
            return "SELECT a.*," + _db.GetSQL1_Ocas("p10") + ",b02.b02Name,p13.p13Name,p13.p13Code,p20.p20Code,p20.p20Name,p20pro.p20Name as p20NamePro,p20pro.p20Code as p20CodePro,p13.p25ID FROM p10MasterProduct a INNER JOIN p20Unit p20 ON a.p20ID=p20.p20ID LEFT OUTER JOIN p13MasterTpv p13 ON a.p13ID=p13.p13ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p20Unit p20pro ON a.p20ID_Pro=p20pro.p20ID";
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
            if (rec.pid == 0)
            {
                rec.b02ID = _mother.b02StatusBL.LoadStartStatusPID("p10", rec.b02ID);  //startovací workflow stav
            }
            p.AddInt("b02ID", rec.b02ID,true);
            
            p.AddInt("p20ID", rec.p20ID, true);
            p.AddInt("p20ID_Pro", rec.p20ID_Pro, true);
            p.AddString("p10Name", rec.p10Name);
            p.AddString("p10Code", rec.p10Code);
            p.AddString("p10Memo", rec.p10Memo);
            
            p.AddDouble("p10RecalcUnit2Kg", rec.p10RecalcUnit2Kg);
            p.AddEnumInt("p10TypeFlag", rec.p10TypeFlag);

            int intPID= _db.SaveRecord("p10MasterProduct", p.getDynamicDapperPars(), rec);
            var recP19 = _mother.p19MaterialBL.LoadByMasterP10ID(intPID);
            if (rec.p10TypeFlag == BO.ProductTypeEnum.Polotovar)    //zkopírovat polotovar do surovin p19
            {                
                if (recP19 == null)
                {
                    recP19 = new BO.p19Material();
                }
                recP19.p10ID_Master = intPID;
                recP19.p19Name = rec.p10Name;
                recP19.p19Code = rec.p10Code;
                recP19.p20ID = rec.p20ID;
                recP19.p19Memo = rec.p10Memo;
                _mother.p19MaterialBL.Save(recP19);
            }
            else
            {
                if (recP19 != null)
                {
                    _mother.CBL.DeleteRecord("p19", recP19.pid);
                }
            }
            return intPID;
        }

        private bool ValidateBeforeSave(BO.p10MasterProduct rec)
        {
            if (rec.p20ID == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit měrná jednotka."); return false;
            }
            if (rec.p20ID_Pro == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit výrobní měrná jednotka."); return false;
            }
            if (rec.p10RecalcUnit2Kg == 0)
            {
                _db.CurrentUser.AddMessage("Přepočet MJ na KG nemůže být NULA."); return false;
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
