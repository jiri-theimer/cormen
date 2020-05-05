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
       
      
        public BO.j04UserRole Load(int pid)
        {            
            return _db.Load<BO.j04UserRole>(string.Format("SELECT a.*,{0} FROM j04UserRole a WHERE a.j04ID={1}", _db.GetSQL1_Ocas("j04"), pid.ToString()));
        }
        public IEnumerable<BO.j04UserRole> GetList(BO.myQuery mq)
        {
            return _db.GetList<BO.j04UserRole>(string.Format("SELECT a.*,{0} FROM j04UserRole a", _db.GetSQL1_Ocas("j04")));
        }

        public int Save(BO.j04UserRole rec)
        {
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.j04ID);
            p.AddString("j04Name", rec.j04Name);
            p.AddInt("j04PermissionValue", rec.j04PermissionValue);
          

            return _db.SaveRecord("j04UserRole", p.getDynamicDapperPars(), rec);
        }
    }
}
