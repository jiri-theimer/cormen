using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip44TaskOperPlanBL
    {
        public BO.p44TaskOperPlan Load(int pid);
        public IEnumerable<BO.p44TaskOperPlan> GetList(BO.myQuery mq);
        public int Save(BO.p44TaskOperPlan rec);
        

    }
    class p44TaskOperPlanBL: BaseBL,Ip44TaskOperPlanBL
    {
        public p44TaskOperPlanBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,p19.p19Code,p19.p19Name,p19.p19Code+' - '+p19.p19Name as Material,p18.p18Code as OperCode,p18.p18Code+isnull(' - '+p18.p18Name,'') as OperCodePlusName," + _db.GetSQL1_Ocas("p44") + " FROM p44TaskOperPlan a LEFT OUTER JOIN p19Material p19 ON a.p19ID=p19.p19ID LEFT OUTER JOIN p18OperCode p18 ON a.p18ID=p18.p18ID";
        }
        public BO.p44TaskOperPlan Load(int pid)
        {
            return _db.Load<BO.p44TaskOperPlan>(string.Format("{0} WHERE a.p44ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.p44TaskOperPlan> GetList(BO.myQuery mq)
        {
            mq.explicit_orderby = "a.p44RowNum";
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p44TaskOperPlan>(fq.FinalSql, fq.Parameters);

        }


        
        public int Save(BO.p44TaskOperPlan rec)
        {
            if (rec.p18ID == 0)
            {
                _mother.CurrentUser.AddMessage("Chybí vazba na číselník [Kód operace].");
                return 0;
            }
            if (rec.p41ID == 0)
            {
                _mother.CurrentUser.AddMessage("Chybí vazba na zakázku.");
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p44ID);
            p.AddInt("p41ID", rec.p41ID, true);
            p.AddInt("p19ID", rec.p19ID, true);
            p.AddInt("p18ID", rec.p18ID, true);

            p.AddInt("p44RowNum", -1 + rec.p44RowNum * 100);
            p.AddInt("p44OperNum", rec.p44OperNum);
            p.AddInt("p44OperParam", rec.p44OperParam);
            p.AddDouble("p44MaterialUnitsCount", rec.p44MaterialUnitsCount);
            //p.AddDateTime("p44Start", rec.p44Start);
            p.AddDouble("p44TotalDurationOperMin", rec.p44TotalDurationOperMin);
            //p.AddDateTime("p44End", rec.p44Start.AddMinutes(rec.p44TotalDurationOperMin));

            var intPID = _db.SaveRecord("p44TaskOperPlan", p.getDynamicDapperPars(), rec);
            if (intPID > 0)
            {
                _db.RunSql("UPDATE p44TaskOperPlan SET p44RowNum=p44RowNum*100 WHERE p41ID=@p41id AND p44ID<>@pid", new { p41id = rec.p41ID, pid = intPID });
                _db.RunSql("update a set p44RowNum=RowID from (SELECT ROW_NUMBER() OVER(ORDER BY p44RowNum ASC) AS RowID,* FROM p44TaskOperPlan WHERE p41ID=@p41id) a", new { p41id = rec.p41ID });

                var pars = new Dapper.DynamicParameters();
                pars.Add("userid", _db.CurrentUser.pid);
                pars.Add("pid", rec.p41ID, System.Data.DbType.Int32);
                pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);
                _db.RunSp("p41_after_save", ref pars);
            }

            return intPID;
        }
    }
}
