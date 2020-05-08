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
            return "SELECT a.*,p19.p19Code,p19.p19Name,p19.p19Code+' - '+p19.p19Name as Material," + _db.GetSQL1_Ocas("p14") + " FROM p14MasterOper a LEFT OUTER JOIN p19Material p19 ON a.p19ID=p19.p19ID";
        }
        public BO.p14MasterOper Load(int pid)
        {
            return _db.Load<BO.p14MasterOper>(string.Format("{0} WHERE a.p14ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p14MasterOper> GetList(BO.myQuery mq)
        {
            mq.explicit_orderby = "a.p14RowNum";
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p14MasterOper>(fq.FinalSql, fq.Parameters);

        }
        

       
    }
}
