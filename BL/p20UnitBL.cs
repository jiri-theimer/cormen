using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip20UnitBL
    {
        public BO.p20Unit Load(int pid);
        public IEnumerable<BO.p20Unit> GetList(BO.myQuery mq);
        public int Save(BO.p20Unit rec);
    }
    class p20UnitBL : BaseBL, Ip20UnitBL
    {

        public p20UnitBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p20") + " FROM " + BL.TheEntities.ByPrefix("p20").SqlFrom;
        }
        public BO.p20Unit Load(int pid)
        {
            return _db.Load<BO.p20Unit>(string.Format("{0} WHERE a.p20ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.p20Unit> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p20Unit>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p20Unit rec)
        {           
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p20ID);
                     
            p.AddString("p20Name", rec.p20Name);
            p.AddString("p20Code", rec.p20Code);
            

            return _db.SaveRecord("p20Unit", p.getDynamicDapperPars(), rec);
        }
    }
}
