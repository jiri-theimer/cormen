using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ip15ClientOperBL
    {
        public BO.p15ClientOper Load(int pid);
        public IEnumerable<BO.p15ClientOper> GetList(BO.myQuery mq);
        public int Save(BO.p15ClientOper rec);
        public void PrecislujOperNum(int p15id_start);

    }
    class p15ClientOperBL : BaseBL, Ip15ClientOperBL
    {
        public p15ClientOperBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,p19.p19Code,p19.p19Name,p19.p19Code+' - '+p19.p19Name as Material,p18.p18Code as OperCode,p18.p18Code+isnull(' - '+p18.p18Name,'') as OperCodePlusName," + _db.GetSQL1_Ocas("p15") + " FROM p15ClientOper a LEFT OUTER JOIN p19Material p19 ON a.p19ID=p19.p19ID LEFT OUTER JOIN p18OperCode p18 ON a.p18ID=p18.p18ID";
        }
        public BO.p15ClientOper Load(int pid)
        {
            return _db.Load<BO.p15ClientOper>(string.Format("{0} WHERE a.p15ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.p15ClientOper> GetList(BO.myQuery mq)
        {
            mq.explicit_orderby = "a.p15RowNum";
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p15ClientOper>(fq.FinalSql, fq.Parameters);

        }


        public int Save(BO.p15ClientOper rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p15ID);
            p.AddInt("p12ID", rec.p12ID, true);
            p.AddInt("p19ID", rec.p19ID, true);
            p.AddInt("p18ID", rec.p18ID, true);
            
            p.AddInt("p15RowNum", -1 + rec.p15RowNum * 100);
            p.AddInt("p15OperNum", rec.p15OperNum);
            p.AddDouble("p15OperParam", rec.p15OperParam);
            p.AddDouble("p15UnitsCount", rec.p15UnitsCount);
            p.AddDouble("p15DurationPreOper", rec.p15DurationPreOper);
            p.AddDouble("p15DurationPostOper", rec.p15DurationPostOper);
            p.AddDouble("p15DurationOper", rec.p15DurationOper);

            int intPID= _db.SaveRecord("p15ClientOper", p.getDynamicDapperPars(), rec);

            _db.RunSql("UPDATE p15ClientOper SET p15RowNum=p15RowNum*100 WHERE p12ID=@p12id AND p15ID<>@pid", new { p12ID = rec.p12ID, pid = intPID });
            _db.RunSql("update a set p15RowNum=RowID from (SELECT ROW_NUMBER() OVER(ORDER BY p15RowNum ASC) AS RowID,* FROM p15ClientOper WHERE p12ID=@p12id) a", new { p12ID = rec.p12ID });

            _db.RunSql("UPDATE p12ClientTpv SET p12TotalDuration=(SELECT sum(isnull(p15DurationPreOper,0)+isnull(p15DurationOper,0)+isnull(p15DurationPostOper,0)) FROM p15ClientOper WHERE p12ID=@pid) WHERE p12ID=@pid", new { pid = rec.p12ID });

            return intPID;
        }


        public void PrecislujOperNum(int p15id_start)
        {
            var rec = Load(p15id_start);
            var rn = rec.p15OperNum;
            var mq = new BO.myQuery("p15ClientOper");
            mq.p12id = rec.p12ID;
            BO.p15ClientOper cLast = rec;
            foreach (var c in GetList(mq).OrderBy(p => p.p15RowNum).Where(p => p.p15RowNum > rec.p15RowNum))
            {
                if (c.OperCode != cLast.OperCode)
                {
                    rn = cLast.p15OperNum + 10;
                    _db.RunSql("UPDATE p15ClientOper set p15OperNum=@opernum WHERE p15ID=@pid", new { pid = c.pid, opernum = rn });
                    c.p15OperNum = rn;
                }
                else
                {
                    _db.RunSql("UPDATE p15ClientOper set p15OperNum=@opernum WHERE p15ID=@pid", new { pid = c.pid, opernum = cLast.p15OperNum });
                    c.p15OperNum = cLast.p15OperNum;
                }
                cLast = c;
            }

        }
    }
}
