using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip19MaterialBL
    {
        public BO.p19Material Load(int pid);
        public IEnumerable<BO.p19Material> GetList(BO.myQuery mq);
        public int Save(BO.p19Material rec);
    }
    class p19MaterialBL : BaseBL, Ip19MaterialBL
    {

        public p19MaterialBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,p28.p28Name,p20.p20Code," + _db.GetSQL1_Ocas("p19") + " FROM p19Material a INNER JOIN p20Unit p20 ON a.p20ID=p20.p20ID LEFT OUTER JOIN p28Company p28 ON a.p28ID=p28.p28ID";
        }
        public BO.p19Material Load(int pid)
        {
            return _db.Load<BO.p19Material>(string.Format("{0} WHERE a.p19ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.p19Material> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p19Material>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.p19Material rec)
        {
            if (rec.p20ID == 0)
            {
                _db.CurrentUser.AddMessage("Chybí vyplnit měrná jednotka."); return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p19ID);            
            p.AddInt("p20ID", rec.p20ID, true);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("p28ID", rec.p28ID, true);
            if (_db.CurrentUser.j03EnvironmentFlag == 2 && rec.p28ID !=_db.CurrentUser.p28ID)
            {
                _db.CurrentUser.AddMessage("V klientském režimu se pořizuje materiál na míru klienta. Musíte záznam svázat s klientem s vazbou na váš profil.");
                return 0;
            }

            p.AddString("p19Name", rec.p19Name);
            p.AddString("p19Code", rec.p19Code);
            p.AddString("p19Memo", rec.p19Memo);

            p.AddString("p19Supplier", rec.p19Supplier);
            p.AddString("p19Intrastat", rec.p19Intrastat);
            p.AddString("p19NameAlias", rec.p19NameAlias);
            p.AddString("p19ITSINC", rec.p19ITSINC);
            p.AddString("p19ITSCAS", rec.p19ITSCAS);
            p.AddString("p19ITSEINECS", rec.p19ITSEINECS);

            p.AddString("p19Lang1", rec.p19Lang1);
            p.AddString("p19Lang2", rec.p19Lang2);
            p.AddString("p19Lang3", rec.p19Lang3);
            p.AddString("p19Lang4", rec.p19Lang4);


            return _db.SaveRecord("p19Material", p.getDynamicDapperPars(), rec);
        }
    }
}
