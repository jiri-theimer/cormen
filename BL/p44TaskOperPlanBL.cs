using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ip44TaskOperPlanBL
    {
        public BO.p44TaskOperPlan Load(int pid);
        public IEnumerable<BO.p44TaskOperPlan> GetList(BO.myQuery mq);
        



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


        
        
    }
}
