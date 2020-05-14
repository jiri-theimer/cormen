using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij04UserRoleBL
    {
        public BO.j04UserRole Load(int pid);
        public IEnumerable<BO.j04UserRole> GetList(BO.myQuery mq);
        public int Save(BO.j04UserRole rec);
    }
    class j04UserRoleBL: BaseBL,Ij04UserRoleBL
    {

        public j04UserRoleBL(BL.Factory mother):base(mother)
        {
            
        }

        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("j04") + " FROM " + BL.TheEntities.ByPrefix("j04").SqlFrom;
        }

        public BO.j04UserRole Load(int pid)
        {
            return _db.Load<BO.j04UserRole>(string.Format("{0} WHERE a.j04ID=@pid", GetSQL1()), new { pid = pid });            
        }
        public IEnumerable<BO.j04UserRole> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j04UserRole>(fq.FinalSql, fq.Parameters);
        }

        public int Save(BO.j04UserRole rec)
        {
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.j04ID);
            p.AddString("j04Name", rec.j04Name);
            p.AddInt("j04PermissionValue", rec.j04PermissionValue);
            p.AddBool("j04IsClientRole", rec.j04IsClientRole);

            return _db.SaveRecord("j04UserRole", p.getDynamicDapperPars(), rec);
        }
    }
}
