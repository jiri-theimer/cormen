using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ib03StatusGroupBL
    {
        public BO.b03StatusGroup Load(int pid);
        
        public IEnumerable<BO.b03StatusGroup> GetList(BO.myQuery mq);
        public int Save(BO.b03StatusGroup rec, List<int> b02ids);
    }
    class b03StatusGroupBL : BaseBL, Ib03StatusGroupBL
    {

        public b03StatusGroupBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("b03") + " FROM b03StatusGroup a";
        }
        public BO.b03StatusGroup Load(int pid)
        {
            return _db.Load<BO.b03StatusGroup>(string.Format("{0} WHERE a.b03ID=@pid", GetSQL1()), new { pid = pid });
        }
      
        public IEnumerable<BO.b03StatusGroup> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.b03StatusGroup>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.b03StatusGroup rec,List<int>b02ids)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.b03ID);

            p.AddString("b03Name", rec.b03Name);                                  

            int intPID= _db.SaveRecord("b03StatusGroup", p.getDynamicDapperPars(), rec);
            if (b02ids != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM b04StatusGroupBinding WHERE b03ID=@pid", new { pid = intPID });
                }
                if (b02ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO b04StatusGroupBinding(b03ID,b02ID) SELECT @pid,b02ID FROM b02Status WHERE b02ID IN (" + string.Join(",", b02ids)+")", new { pid = intPID });
                }
                
            }
            return intPID;
        }
    }
}
