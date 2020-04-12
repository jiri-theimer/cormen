using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij04UserRoleBL
    {
        public BO.j04UserRole Load(int pid);
        public IEnumerable<BO.j04UserRole> GetList(BO.myQuery mq);

    }
    class j04UserRoleBL: Ij04UserRoleBL
    {
        public BO.j04UserRole Load(int pid)
        {            
            return DL.DbHandler.Load<BO.j04UserRole>(string.Format("SELECT a.*,{0} FROM j04UserRole a WHERE a.j04ID={1}", DL.DbHandler.GetSQL1_Ocas("j04"), pid.ToString()));
        }
        public IEnumerable<BO.j04UserRole> GetList(BO.myQuery mq)
        {
            return DL.DbHandler.GetList<BO.j04UserRole>(string.Format("SELECT a.*,{0} FROM j04UserRole a", DL.DbHandler.GetSQL1_Ocas("j04")));
        }


    }
}
