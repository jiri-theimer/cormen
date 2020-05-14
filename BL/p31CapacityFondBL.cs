using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip31CapacityFondBL
    {
        public BO.p31CapacityFond Load(int pid);
        public IEnumerable<BO.p31CapacityFond> GetList(BO.myQuery mq);
        public int Save(BO.p31CapacityFond rec);
    }
    class p31CapacityFondBL : BaseBL, Ip31CapacityFondBL
    {

        public p31CapacityFondBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner," + _db.GetSQL1_Ocas("p31") + " FROM " + BL.TheEntities.ByPrefix("p31").SqlFrom;
        }
        public BO.p31CapacityFond Load(int pid)
        {
            return _db.Load<BO.p31CapacityFond>(string.Format("{0} WHERE a.p31ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.p31CapacityFond> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p31CapacityFond>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p31CapacityFond rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p31ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.pid;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddString("p31Name", rec.p31Name);
         

            return _db.SaveRecord("p31CapacityFond", p.getDynamicDapperPars(), rec);
        }
    }
}
