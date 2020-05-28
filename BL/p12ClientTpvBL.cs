using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip12ClientTpvBL
    {
        public BO.p12ClientTpv Load(int pid);
        public IEnumerable<BO.p12ClientTpv> GetList(BO.myQuery mq);
        public int Save(BO.p12ClientTpv rec, List<BO.p15ClientOper> lisP15);
        public double Simulate_Total_Duration(int p12id, double unitscount, int p27id);
    }
    class p12ClientTpvBL : BaseBL, Ip12ClientTpvBL
    {
        public p12ClientTpvBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,p21.p21Name,p21.p21Code,p28.p28Name,p13.p13Name,p13.p13Code,p25.p25Name,p13.p25ID," + _db.GetSQL1_Ocas("p12") + " FROM p12ClientTpv a INNER JOIN p21License p21 ON a.p21ID=p21.p21ID LEFT OUTER JOIN p28Company p28 ON p21.p28ID=p28.p28ID LEFT OUTER JOIN p13MasterTpv p13 ON a.p13ID_Master=p13.p13ID LEFT OUTER JOIN p25MszType p25 ON a.p25ID=p25.p25ID";
        }
        public BO.p12ClientTpv Load(int pid)
        {
            return _db.Load<BO.p12ClientTpv>(string.Format("{0} WHERE a.p12ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.p12ClientTpv> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p12ClientTpv>(fq.FinalSql, fq.Parameters);

        }
        
        public double Simulate_Total_Duration(int p12id,double unitscount,int p27id)
        {
            return _db.Load<BO.COM.GetDouble>("select dbo.p12_calc_duration(@p12id,@unitscount,@p27id) as Value", new { p12id = p12id, unitscount = unitscount,p27id=p27id }).Value;
        }

        private bool ValidateBeforeSave(BO.p12ClientTpv rec)
        {
            if (rec.p21ID == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit [Licence]."); return false;
            }
            if (rec.p25ID == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit [Typ zařízení]."); return false;
            }
            BO.p21License cP21 = _mother.p21LicenseBL.Load(rec.p21ID);
            if (cP21.p21PermissionFlag != BO.p21PermENUM.Independent2Master)
            {
                _db.CurrentUser.AddMessage(string.Format("Ve zvolené licenci [{1} - {0}] nelze zakládat/upravovat vlastní receptury.",cP21.p21Name,cP21.p21Code)); return false;
            }

            return true;
        }

        public int Save(BO.p12ClientTpv rec, List<BO.p15ClientOper> lisP15)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p12ID);
            p.AddInt("p21ID", rec.p21ID, true);
            p.AddInt("p25ID", rec.p25ID, true);
            p.AddString("p12Name", rec.p12Name);
            p.AddString("p12Code", rec.p12Code);
            p.AddString("p12Memo", rec.p12Memo);

            int intPID = _db.SaveRecord("p12ClientTpv", p.getDynamicDapperPars(), rec);

            if (intPID > 0 && lisP15 != null)
            {
                foreach (var c in lisP15)
                {
                    c.pid = c.p15ID;
                    c.p12ID = intPID;
                    if (c.TempRecDisplay == "none" && c.p15ID > 0)
                    {
                        _db.RunSql("DELETE FROM p15ClientOper WHERE p15ID=@p15id", new { p15id = c.p15ID });
                    }
                    else
                    {
                        SaveP15Rec(c);
                    }

                }
            }
            var mq = new BO.myQuery("p15ClientOper");
            mq.p12id = intPID;
            var lis = _mother.p15ClientOperBL.GetList(mq);
            _db.RunSql("UPDATE p12ClientTpv SET p12TotalDuration=(SELECT sum(isnull(p15DurationPreOper,0)+isnull(p15DurationOper,0)+isnull(p15DurationPostOper,0)) FROM p15ClientOper WHERE p12ID=@pid) WHERE p12ID=@pid", new { pid = intPID });


            return intPID;

        }

        private int SaveP15Rec(BO.p15ClientOper rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p15ID);
            p.AddInt("p12ID", rec.p12ID, true);
            p.AddInt("p19ID", rec.p19ID, true);
            p.AddInt("p18ID", rec.p18ID, true);
            p.AddString("p15Name", rec.p15Name);
            p.AddInt("p15RowNum", rec.p15RowNum);
            p.AddString("p15OperNum", rec.p15OperNum);
            p.AddInt("p15OperParam", rec.p15OperParam);
            p.AddDouble("p15UnitsCount", rec.p15UnitsCount);
            p.AddDouble("p15DurationPreOper", rec.p15DurationPreOper);
            p.AddDouble("p15DurationPostOper", rec.p15DurationPostOper);
            p.AddDouble("p15DurationOper", rec.p15DurationOper);

            return _db.SaveRecord("p15ClientOper", p.getDynamicDapperPars(), rec);
        }
    }
}
