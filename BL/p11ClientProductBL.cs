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
            return "SELECT a.*," + _db.GetSQL1_Ocas("p11") + ",b02.b02Name,p12.p12Name,p12.p12Code,p21.p21Name,p21.p21Code,p10.p10Name,p10.p10Code,p20.p20Code,p20.p20Name,p28.p28Name,p20pro.p20Name as p20NamePro,p20pro.p20Code as p20CodePro FROM p11ClientProduct a INNER JOIN p12ClientTpv p12 ON a.p12ID=p12.p12ID INNER JOIN p20Unit p20 ON a.p20ID=p20.p20ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID LEFT OUTER JOIN p21License p21 ON a.p21ID=p21.p21ID LEFT OUTER JOIN p28Company p28 ON p21.p28ID=p28.p28ID LEFT OUTER JOIN p10MasterProduct p10 ON a.p10ID_Master=p10.p10ID LEFT OUTER JOIN p20Unit p20pro ON a.p20ID_Pro=p20pro.p20ID";
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
            if (rec.p12ID > 0)
            {
                rec.p21ID = _mother.p12ClientTpvBL.Load(rec.p12ID).p21ID;   //licenci produktu zjistit z receptury
            }
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p11ID);
            p.AddInt("p12ID", rec.p12ID, true);
            p.AddInt("p10ID_Master", rec.p10ID_Master, true);
            p.AddInt("b02ID", rec.b02ID, true);
            p.AddInt("p20ID", rec.p20ID, true);
            p.AddInt("p20ID_Pro", rec.p20ID_Pro, true);
            p.AddInt("p21ID", rec.p21ID, true);
            p.AddString("p11Name", rec.p11Name);
            p.AddString("p11Code", rec.p11Code);
            p.AddString("p11Memo", rec.p11Memo);
            p.AddDouble("p11UnitPrice", rec.p11UnitPrice);
            p.AddDouble("p11RecalcUnit2Kg", rec.p11RecalcUnit2Kg);
            p.AddEnumInt("p11TypeFlag", rec.p11TypeFlag);
            p.AddString("p11PackagingCode", rec.p11PackagingCode);
            p.AddDouble("p11Davka", rec.p11Davka);
            p.AddDouble("p11DavkaMin", rec.p11DavkaMin);
            p.AddDouble("p11DavkaMax", rec.p11DavkaMax);
            p.AddDouble("p11SalesPerMonth", rec.p11SalesPerMonth);
            p.AddDouble("p11UnitsPerPalette", rec.p11UnitsPerPalette);

            int intPID= _db.SaveRecord("p11ClientProduct", p.getDynamicDapperPars(), rec);
            return intPID;
        }

        private bool ValidateBeforeSave(BO.p11ClientProduct rec)
        {
            if (rec.p21ID == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit [Licence]."); return false;
            }
            if (rec.p20ID == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit [Měrná jednotka]."); return false;
            }
            if (rec.p20ID_Pro == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit [Výrobní jednotka]."); return false;
            }
            if (rec.p12ID == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit [Receptura]."); return false;
            }
            if (rec.p11RecalcUnit2Kg == 0)
            {
                _db.CurrentUser.AddMessage("Přepočet MJ na KG nemůže být NULA."); return false;
            }

            //if (LoadByCode(rec.p11Code, rec.pid) != null)
            //{
            //    _db.CurrentUser.AddMessage(string.Format("Zadaný kód nemůže být duplicitní s jiným produktem [{0}].", LoadByCode(rec.p11Code, rec.pid).p11Name));
            //    return false;
            //}

            BO.p12ClientTpv cP12 = _mother.p12ClientTpvBL.Load(rec.p12ID);
            if (cP12.p21ID != rec.p21ID)
            {
                _db.CurrentUser.AddMessage("Licence produktu se liší od licence receptury."); return false;
            }

            if (rec.p10ID_Master > 0)
            {
                BO.p10MasterProduct cP10 = _mother.p10MasterProductBL.Load(rec.p10ID_Master);                
                if (cP10.p13ID != cP12.p13ID_Master)
                {
                    _db.CurrentUser.AddMessage("Receptura vzorového Master produktu se liší od vzoru klientské receptury."); return false;
                }
            }
            else
            {
                BO.p21License cP21 = _mother.p21LicenseBL.Load(rec.p21ID);
                if (cP21.p21PermissionFlag == BO.p21PermENUM.Standard)
                {
                    _db.CurrentUser.AddMessage(string.Format("Licence [{1} - {0}] je typu [Standard], kde nelze zakládat produkty s vlastní recepturou.", cP21.p21Name, cP21.p21Code)); return false;
                }
            }
            

            return true;
        }
    }
}
