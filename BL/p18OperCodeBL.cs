﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip18OperCodeBL
    {
        public BO.p18OperCode Load(int pid);
        public BO.p18OperCode LoadByCode(string strCode, int intP25ID, int intExcludePID);
        public IEnumerable<BO.p18OperCode> GetList(BO.myQuery mq);
        public int Save(BO.p18OperCode rec);
    }
    class p18OperCodeBL : BaseBL, Ip18OperCodeBL
    {

        public p18OperCodeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,p25.p25Name,p19.p19Name,p19.p19Code," + _db.GetSQL1_Ocas("p18") + " FROM "+ BL.TheEntities.ByPrefix("p18").SqlFrom;
        }
        public BO.p18OperCode Load(int pid)
        {
            return _db.Load<BO.p18OperCode>(string.Format("{0} WHERE a.p18ID=@pid", GetSQL1()), new { pid = pid });
        }
        public BO.p18OperCode LoadByCode(string strCode,int intP25ID, int intExcludePID)
        {
            return _db.Load<BO.p18OperCode>(string.Format("{0} WHERE a.p18Code LIKE @code AND a.p18ID<>@exclude AND a.p25ID=@p25id", GetSQL1()), new { code = strCode, exclude = intExcludePID,p25id= intP25ID });
        }
        public IEnumerable<BO.p18OperCode> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p18OperCode>(fq.FinalSql, fq.Parameters);

        }

        
        public int Save(BO.p18OperCode rec)
        {
            if (LoadByCode(rec.p18Code,rec.p25ID,rec.p18ID) != null)
            {
                _mother.CurrentUser.AddMessage("Kód operace nesmí být duplicitní v rámci jednoho typu zařízení.");
                return 0;
            }
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.p18ID);
            p.AddInt("p25ID", rec.p25ID, true);
            p.AddInt("p19ID", rec.p19ID, true);
            p.AddString("p18Name", rec.p18Name);
            p.AddString("p18Code", rec.p18Code);
            p.AddDouble("p18UnitsCount", rec.p18UnitsCount);
            p.AddDouble("p18DurationPreOper", rec.p18DurationPreOper);
            p.AddDouble("p18DurationOper", rec.p18DurationOper);
            p.AddDouble("p18DurationPostOper", rec.p18DurationPostOper);
            p.AddString("p18Lang1", rec.p18Lang1);
            p.AddString("p18Lang2", rec.p18Lang2);
            p.AddString("p18Lang3", rec.p18Lang3);
            p.AddString("p18Lang4", rec.p18Lang4);

            return _db.SaveRecord("p18OperCode", p.getDynamicDapperPars(), rec);
        }
    }
}
