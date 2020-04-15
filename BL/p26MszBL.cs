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
    class p26MszBL:Ip26MszBL
    {
        private BO.RunningUser _cUser;
        public p26MszBL(BO.RunningUser cUser)
        {
            _cUser = cUser;
        }
        private string GetSQL1()
        {
            return "SELECT a.*," + DL.DbHandler.GetSQL1_Ocas("p26") + ",b02.b02Name as _b02name FROM p26Msz a LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID";
        }
        public BO.p26Msz Load(int pid)
        {
            return DL.DbHandler.Load<BO.p26Msz>(string.Format("{0} WHERE a.p26ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p26Msz> GetList(BO.myQuery mq)
        {
            return DL.DbHandler.GetList<BO.p26Msz>(GetSQL1());
        }

        public int Save(BO.p26Msz rec)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("pid", rec.p26ID);            
            p.Add("b02ID",BO.BAS.TestIntAsDbKey(rec.b02ID));
            p.Add("p26Name", rec.p26Name);           
            p.Add("p26Code", rec.p26Code);
            p.Add("p26Memo", rec.p26Memo);
            

            return DL.DbHandler.SaveRecord(_cUser,"p26Msz", p, rec);
        }
    }
}
