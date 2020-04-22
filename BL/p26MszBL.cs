using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip26MszBL
    {
        public BO.p26Msz Load(int pid);
        public IEnumerable<BO.p26Msz> GetList(BO.myQuery mq);
        public int Save(BO.p26Msz rec);
    }
    class p26MszBL:BaseBL,Ip26MszBL
    {      
        public p26MszBL(BL.Factory mother):base(mother)
        {
           
        }
        
        
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p26") + ",b02.b02Name as _b02name,p28.p28Name as _p28Name FROM p26Msz a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID";
        }
        public BO.p26Msz Load(int pid)
        {
            return _db.Load<BO.p26Msz>(string.Format("{0} WHERE a.p26ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p26Msz> GetList(BO.myQuery mq)
        {
            return _db.GetList<BO.p26Msz>(GetSQL1());
        }

        public int Save(BO.p26Msz rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.p26ID);            
            p.Add("b02ID",BO.BAS.TestIntAsDbKey(rec.b02ID));
            p.Add("p28ID", BO.BAS.TestIntAsDbKey(rec.p28ID));
            p.Add("p26Name", rec.p26Name);           
            p.Add("p26Code", rec.p26Code);
            p.Add("p26Memo", rec.p26Memo);
            

            return _db.SaveRecord("p26Msz", p, rec);
        }
    }
}
