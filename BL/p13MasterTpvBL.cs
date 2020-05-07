using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip13MasterTpvBL
    {
        public BO.p13MasterTpv Load(int pid);
        public IEnumerable<BO.p13MasterTpv> GetList(BO.myQuery mq);
        public int Save(BO.p13MasterTpv rec, List<BO.p14MasterOper> lisP14);
    }
    class p13MasterTpvBL : BaseBL,Ip13MasterTpvBL
    {     
        public p13MasterTpvBL(BL.Factory mother):base(mother)
        {
           
        }
       
       
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("p13") + " FROM p13MasterTpv a";
        }
        public BO.p13MasterTpv Load(int pid)
        {

            return _db.Load<BO.p13MasterTpv>(string.Format("{0} WHERE a.p13ID={1}", GetSQL1(), pid));
        }
        public IEnumerable<BO.p13MasterTpv> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p13MasterTpv>(fq.FinalSql, fq.Parameters);

        }        

        public int Save(BO.p13MasterTpv rec,List<BO.p14MasterOper> lisP14)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p13ID);           
            p.AddString("p13Name", rec.p13Name);
            p.AddString("p13Code", rec.p13Code);
            p.AddString("p13Memo", rec.p13Memo);
           
            int intPID = _db.SaveRecord("p13MasterTpv", p.getDynamicDapperPars(), rec);

            if (intPID > 0 && lisP14 !=null)
            {
                foreach (var c in lisP14)
                {
                    c.pid = c.p14ID;
                    c.p13ID = intPID;
                    if (c.TempRecDisplay == "none" && c.p14ID > 0)
                    {
                        _db.RunSql("DELETE FROM p14MasterOper WHERE p14ID=@p14id", new { p14id = c.p14ID });
                    }
                    else
                    {
                        SaveP14Rec(c);
                    }
                    
                }
            }

            return intPID;

        }

        private int SaveP14Rec(BO.p14MasterOper rec)
        {
            var p = new DL.Params4Dapper();            
            p.AddInt("pid", rec.p14ID);
            p.AddInt("p13ID", rec.p13ID, true);
            p.AddString("p14Name", rec.p14Name);
            p.AddString("p14MaterialCode", rec.p14MaterialCode);
            p.AddString("p14MaterialName", rec.p14MaterialName);
            p.AddInt("p14RowNum", rec.p14RowNum);
            p.AddString("p14OperCode", rec.p14OperCode);
            p.AddString("p14OperNum", rec.p14OperNum);
            p.AddInt("p14OperParam", rec.p14OperParam);
            p.AddDouble("p14UnitsCount", rec.p14UnitsCount);
            p.AddDouble("p14DurationPreOper", rec.p14DurationPreOper);
            p.AddDouble("p14DurationPostOper", rec.p14DurationPostOper);
            p.AddDouble("p14DurationOper", rec.p14DurationOper);

            return _db.SaveRecord("p14MasterOper", p.getDynamicDapperPars(), rec);
        }

        //public int Save(BO.p13MasterTpv rec,string strGUID)
        //{

        //    BO.p85Tempbox cP85;

        //    cP85 = new BO.p85Tempbox() { p85RecordPid = rec.pid, p85GUID = strGUID, p85Prefix = "p13", p85FreeText01 = rec.p13Name, p85FreeText02 = rec.p13Code, p85Message = rec.p13Memo, p85FreeDate01 = rec.ValidFrom, p85FreeDate02 = rec.ValidUntil };
        //    _mother.p85TempboxBL.Save(cP85);           

        //    var p = new Dapper.DynamicParameters();
        //    p.Add("userid", _db.CurrentUser.pid);
        //    p.Add("guid", strGUID);
        //    p.Add("pid_ret", rec.pid, System.Data.DbType.Int32, System.Data.ParameterDirection.Output);
        //    p.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);

        //    string s1 = _db.RunSp("p13_save", ref p);
        //    if (s1 == "1")
        //    {
        //        return p.Get<int>("pid_ret");
        //    }
        //    else
        //    {

        //        return 0;
        //    }

        //}
    }
}
