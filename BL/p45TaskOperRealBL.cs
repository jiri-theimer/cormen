using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip45TaskOperRealBL
    {
        public BO.p45TaskOperReal Load(int pid);
        public IEnumerable<BO.p45TaskOperReal> GetList(BO.myQuery mq);
        public int Save(BO.p45TaskOperReal rec);



    }
    class p45TaskOperRealBL : BaseBL, Ip45TaskOperRealBL
    {
        public p45TaskOperRealBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,p19.p19Code,p19.p19Name,p19.p19Code+' - '+p19.p19Name as Material,p18.p18Code as OperCode,p18.p18Code+isnull(' - '+p18.p18Name,'') as OperCodePlusName," + _db.GetSQL1_Ocas("p45") + " FROM p45TaskOperReal a LEFT OUTER JOIN p19Material p19 ON a.p19ID=p19.p19ID LEFT OUTER JOIN p18OperCode p18 ON a.p18ID=p18.p18ID";
        }
        public BO.p45TaskOperReal Load(int pid)
        {
            return _db.Load<BO.p45TaskOperReal>(string.Format("{0} WHERE a.p45ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.p45TaskOperReal> GetList(BO.myQuery mq)
        {
            mq.explicit_orderby = "a.p45RowNum";
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p45TaskOperReal>(fq.FinalSql, fq.Parameters);

        }


        public int Save(BO.p45TaskOperReal rec)
        {
            if (rec.p41ID == 0)
            {
                _mother.CurrentUser.AddMessage("Chybí p41ID.");
                return 0;
            }
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.p45ID);

            p.AddInt("p41ID", rec.p41ID, true);
            p.AddInt("p18ID", rec.p18ID, true);
            p.AddInt("p19ID", rec.p19ID, true);
            
            p.AddString("p45Name", rec.p45Name);
            p.AddString("p45MaterialCode", rec.p45MaterialCode);
            p.AddString("p45MaterialBattch", rec.p45MaterialBattch);
            p.AddString("p45MaterialName", rec.p45MaterialName);

           
            p.AddInt("p45RowNum", rec.p45RowNum);
            p.AddString("p45OperCode", rec.p45OperCode);
            p.AddInt("p45OperNum", rec.p45OperNum);
            p.AddDouble("p45OperParam", rec.p45OperParam);
            p.AddString("p45OperStatus", rec.p45OperStatus);
            p.AddDateTime("p45Start", rec.p45Start);
            p.AddDateTime("p45End", rec.p45End);
            p.AddInt("p45EndFlag", rec.p45EndFlag);
            p.AddDouble("p45MaterialUnitsCount", rec.p45MaterialUnitsCount);
            p.AddDouble("p45TotalDurationOperMin", rec.p45TotalDurationOperMin);
            p.AddString("p45Operator", rec.p45Operator);


            return _db.SaveRecord("p45TaskOperReal", p.getDynamicDapperPars(), rec);
        }


    }
}
