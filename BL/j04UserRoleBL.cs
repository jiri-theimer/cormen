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
            return "SELECT a.*," + _db.GetSQL1_Ocas("j04") + " FROM j04UserRole a";
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

        private bool TestOnePerm(BO.UserPermFlag oneperm, BO.j04UserRole rec)
        {
            int x = (int)oneperm;
            int y = x & rec.j04PermissionValue;
            if (y == x)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int Save(BO.j04UserRole rec)
        {           
            if (!_mother.CurrentUser.IsGURU())
            {
                if (rec.j04IsClientRole && (TestOnePerm(BO.UserPermFlag.MasterAdmin, rec) || TestOnePerm(BO.UserPermFlag.MasterReader, rec)))
                {
                    _db.CurrentUser.AddMessage("U [CLIENT] role musíte lze zaškrtnout pouze client oprávnění.");
                    return 0;
                }
                if (rec.j04IsClientRole == false && (TestOnePerm(BO.UserPermFlag.ClientAdmin, rec) || TestOnePerm(BO.UserPermFlag.ClientReader, rec)))
                {
                    _db.CurrentUser.AddMessage("U [MASTER] role musíte lze zaškrtnout pouze master oprávnění.");
                    return 0;
                }
            }
            
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.j04ID);
            p.AddString("j04Name", rec.j04Name);
            p.AddInt("j04PermissionValue", rec.j04PermissionValue);
            p.AddBool("j04IsClientRole", rec.j04IsClientRole);

            return _db.SaveRecord("j04UserRole", p.getDynamicDapperPars(), rec);
        }
    }
}
