using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip14MasterOperBL
    {
        public BO.p14MasterOper Load(int pid);
        public IEnumerable<BO.p14MasterOper> GetList(BO.myQuery mq);
       
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
        

       
    }
}
