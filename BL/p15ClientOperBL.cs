using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip15ClientOperBL
    {
        public BO.p15ClientOper Load(int pid);
        public IEnumerable<BO.p15ClientOper> GetList(BO.myQuery mq);
        
    }
    class p15ClientOperBL : BaseBL, Ip15ClientOperBL
    {
        public p15ClientOperBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,p19.p19Code,p19.p19Name,p19.p19Code+' - '+p19.p19Name as Material,p18.p18Code as OperCode," + _db.GetSQL1_Ocas("p15") + " FROM "+ BL.TheEntities.ByPrefix("p15").SqlFrom;
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


       
    }
}
