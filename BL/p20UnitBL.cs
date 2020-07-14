using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip20UnitBL
    {
        public BO.p20Unit Load(int pid);
        public BO.p20Unit LoadByCode(string strCode, int intExcludePID);
        public IEnumerable<BO.p20Unit> GetList(BO.myQuery mq);
        public int Save(BO.p20Unit rec);
    }
    class p20UnitBL : BaseBL, Ip20UnitBL
    {

        public p20UnitBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,p28.p28Name," + _db.GetSQL1_Ocas("p20") + " FROM p20Unit a LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID";
        }
        public BO.p20Unit Load(int pid)
        {
            return _db.Load<BO.p20Unit>(string.Format("{0} WHERE a.p20ID=@pid", GetSQL1()), new { pid = pid });
        }
        public BO.p20Unit LoadByCode(string strCode, int intExcludePID)
        {
            return _db.Load<BO.p20Unit>(string.Format("{0} WHERE a.p20Code LIKE @code AND a.p20ID<>@excludepid", GetSQL1()), new { code = strCode, excludepid = intExcludePID });
        }
        public IEnumerable<BO.p20Unit> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p20Unit>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p20Unit rec)
        {           
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p20ID);
                     
            p.AddString("p20Name", rec.p20Name);
            p.AddString("p20Code", rec.p20Code);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("p28ID", rec.p28ID, true);
            if (_db.CurrentUser.j03EnvironmentFlag == 2 && rec.p28ID != _db.CurrentUser.p28ID)
            {
                _db.CurrentUser.AddMessage("V klientském režimu se se musí položka měrné jednotky povinně svázat s klientem z vašeho profilu.");
                return 0;
            }


            return _db.SaveRecord("p20Unit", p.getDynamicDapperPars(), rec);
        }
    }
}
