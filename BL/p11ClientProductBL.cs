﻿using System;
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
            return "SELECT a.*," + _db.GetSQL1_Ocas("p11") + ",b02.b02Name,p12.p12Name,p12.p12Code,p21.p21Name,p21.p21Code,p10.p10Name,p10.p10Code,p20.p20Code FROM "+ BL.TheEntities.ByPrefix("p11").SqlFrom;
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
            p.AddInt("p10ID_Master", rec.p10ID_Master, true);
            p.AddInt("b02ID", rec.b02ID, true);
            p.AddInt("p20ID", rec.p20ID, true);
            p.AddString("p11Name", rec.p11Name);
            p.AddString("p11Code", rec.p11Code);
            p.AddString("p11Memo", rec.p11Memo);
            p.AddDouble("p11UnitPrice", rec.p11UnitPrice);
            p.AddDouble("p11RecalcUnit2Kg", rec.p11RecalcUnit2Kg);


            return _db.SaveRecord("p11ClientProduct", p.getDynamicDapperPars(), rec);
        }

        private bool ValidateBeforeSave(BO.p11ClientProduct rec)
        {
            if (rec.p20ID == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit měrná jednotka."); return false;
            }
            if (rec.p12ID == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit vazbu na recepturu."); return false;
            }
            if (rec.p10ID_Master == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit vazbu na Master produkt."); return false;
            }
            //if (LoadByCode(rec.p11Code, rec.pid) != null)
            //{
            //    _db.CurrentUser.AddMessage(string.Format("Zadaný kód nemůže být duplicitní s jiným produktem [{0}].", LoadByCode(rec.p11Code, rec.pid).p11Name));
            //    return false;
            //}

            BO.p10MasterProduct cP10 = _mother.p10MasterProductBL.Load(rec.p10ID_Master);
            BO.p12ClientTpv cP12 = _mother.p12ClientTpvBL.Load(rec.p12ID);
            if (cP10.p13ID != cP12.p13ID_Master)
            {
                _db.CurrentUser.AddMessage("Receptura vzorového Master produktu se liší od vzoru klientské receptury."); return false;
            }

            return true;
        }
    }
}
