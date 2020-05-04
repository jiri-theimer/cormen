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
            return "SELECT a.*," + _db.GetSQL1_Ocas("p26") + ",b02.b02Name,p28.p28Name FROM p26Msz a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN b02Status b02 ON a.b02ID=b02.b02ID";
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
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p26ID);            
            p.AddInt("b02ID",rec.b02ID,true);
            p.AddInt("p28ID", rec.p28ID,true);
            p.AddString("p26Name", rec.p26Name);           
            p.AddString("p26Code", rec.p26Code);
            p.AddString("p26Memo", rec.p26Memo);
            

            return _db.SaveRecord("p26Msz", p.getDynamicDapperPars(), rec);
        }
    }
}
