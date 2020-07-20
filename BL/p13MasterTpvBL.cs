using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip13MasterTpvBL
    {
        public BO.p13MasterTpv Load(int pid);
        public BO.p13MasterTpv LoadByCode(string strCode, int intExcludePID);
        public IEnumerable<BO.p13MasterTpv> GetList(BO.myQuery mq);
        public int Save(BO.p13MasterTpv rec,int intP13ID_CloneP14Recs);
        public void DeleteAllP14(int p13id);
    }
    class p13MasterTpvBL : BaseBL,Ip13MasterTpvBL
    {     
        public p13MasterTpvBL(BL.Factory mother):base(mother)
        {
           
        }
       
       
        private string GetSQL1()
        {
            return "SELECT a.*,p25.p25Name," + _db.GetSQL1_Ocas("p13") + " FROM p13MasterTpv a INNER JOIN p25MszType p25 ON a.p25ID=p25.p25ID";
        }
        public BO.p13MasterTpv Load(int pid)
        {

            return _db.Load<BO.p13MasterTpv>(string.Format("{0} WHERE a.p13ID=@pid", GetSQL1()),new { pid = pid });
        }
        public BO.p13MasterTpv LoadByCode(string strCode, int intExcludePID)
        {
            return _db.Load<BO.p13MasterTpv>(string.Format("{0} WHERE a.p13Code LIKE @code AND a.p13ID<>@excludepid", GetSQL1()), new { code = strCode, excludepid = intExcludePID });
        }
        public IEnumerable<BO.p13MasterTpv> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p13MasterTpv>(fq.FinalSql, fq.Parameters);

        }        

        public int Save(BO.p13MasterTpv rec, int intP13ID_CloneP14Recs)
        {

            if (rec.p25ID == 0 || string.IsNullOrEmpty(rec.p13Code) || string.IsNullOrEmpty(rec.p13Name))
            {
                _mother.CurrentUser.AddMessage("Chybí vyplnit typ zařízení, kód nebo název receptury.");
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p13ID);
            p.AddInt("p25ID", rec.p25ID);
            p.AddString("p13Name", rec.p13Name);
            p.AddString("p13Code", rec.p13Code);
            p.AddString("p13Memo", rec.p13Memo);
           
            int intPID = _db.SaveRecord("p13MasterTpv", p.getDynamicDapperPars(), rec);

            if (intP13ID_CloneP14Recs>0 && rec.pid == 0)
            {
                var mq = new BO.myQuery("p14MasterOper");
                mq.p13id = intP13ID_CloneP14Recs;
                var lis = _mother.p14MasterOperBL.GetList(mq);
                foreach(var c in lis)
                {
                    c.pid = 0;
                    c.p13ID = intPID;
                    _mother.p14MasterOperBL.Save(c);
                }
            }
            
            
            return intPID;

        }

        public void DeleteAllP14(int p13id)
        {
            _db.RunSql("DELETE FROM p14MasterOper WHERE p13ID=@pid", new { pid = p13id });
        }

        

        
    }
}
