using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace BL
{
    public interface Ip14MasterOperBL
    {
        public BO.p14MasterOper Load(int pid);
        public IEnumerable<BO.p14MasterOper> GetList(BO.myQuery mq);
        public int Save(BO.p14MasterOper rec);
        public void PrecislujOperNum(int p14id_start);

    }
    class p14MasterOperBL : BaseBL,Ip14MasterOperBL
    {       
        public p14MasterOperBL(BL.Factory mother):base(mother)
        {
            
        }
        
        
        private string GetSQL1()
        {
            return "SELECT a.*,p19.p19Code,p19.p19Name,p19.p19Code+' - '+p19.p19Name as Material,p18.p18Code as OperCode," + _db.GetSQL1_Ocas("p14") + " FROM p14MasterOper a LEFT OUTER JOIN p19Material p19 ON a.p19ID=p19.p19ID LEFT OUTER JOIN p18OperCode p18 ON a.p18ID=p18.p18ID";
        }
        public BO.p14MasterOper Load(int pid)
        {
            return _db.Load<BO.p14MasterOper>(string.Format("{0} WHERE a.p14ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.p14MasterOper> GetList(BO.myQuery mq)
        {
            mq.explicit_orderby = "a.p14RowNum";
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p14MasterOper>(fq.FinalSql, fq.Parameters);

        }


        public void PrecislujOperNum(int p14id_start)
        {
            var rec = Load(p14id_start);
            var rn = rec.p14OperNum + 10;
            var mq = new BO.myQuery("p14MasterOper");
            mq.p13id = rec.p13ID;
            foreach (var c in GetList(mq).OrderBy(p => p.p14RowNum).Where(p => p.p14RowNum > rec.p14RowNum))
            {
                if (c.p18ID != rec.p18ID)
                {
                    _db.RunSql("UPDATE p14MasterOper set p14OperNum=@rn WHERE p14ID=@pid", new { pid = c.pid, rn = rn });
                    rn += 10;
                }
                
            }
            //foreach (var c in GetList(mq).OrderBy(p=>p.p14RowNum).Where(p => p.p14RowNum > rec.p14RowNum))
            //{
            //    _db.RunSql("UPDATE p14MasterOper set p14OperNum=@rn WHERE p14ID=@pid", new { pid = c.pid, rn = rn });
            //    rn += 10;
            //}






        }

        public int Save(BO.p14MasterOper rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p14ID);
            p.AddInt("p13ID", rec.p13ID, true);
            p.AddInt("p19ID", rec.p19ID, true);
            p.AddInt("p18ID", rec.p18ID, true);
            p.AddString("p14Name", rec.p14Name);

            p.AddInt("p14RowNum", -1+rec.p14RowNum * 100);
            p.AddInt("p14OperNum", rec.p14OperNum);
            p.AddInt("p14OperParam", rec.p14OperParam);
            p.AddDouble("p14UnitsCount", rec.p14UnitsCount);
            p.AddDouble("p14DurationPreOper", rec.p14DurationPreOper);
            p.AddDouble("p14DurationPostOper", rec.p14DurationPostOper);
            p.AddDouble("p14DurationOper", rec.p14DurationOper);

            var intPID= _db.SaveRecord("p14MasterOper", p.getDynamicDapperPars(), rec);

            _db.RunSql("UPDATE p14MasterOper SET p14RowNum=p14RowNum*100 WHERE p13ID=@p13id AND p14ID<>@pid", new { p13id = rec.p13ID,pid=intPID });            
            _db.RunSql("update a set p14RowNum=RowID from (SELECT ROW_NUMBER() OVER(ORDER BY p14RowNum ASC) AS RowID,* FROM p14MasterOper WHERE p13ID=@p13id) a", new { p13id = rec.p13ID });

            _db.RunSql("UPDATE p13MasterTpv SET p13TotalDuration=(SELECT sum(isnull(p14DurationPreOper,0)+isnull(p14DurationOper,0)+isnull(p14DurationPostOper,0)) FROM p14MasterOper WHERE p13ID=@pid) WHERE p13ID=@pid", new { pid = rec.p13ID });

            return intPID;
        }

    }
}
