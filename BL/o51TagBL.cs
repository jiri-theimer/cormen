using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Io51TagBL
    {
        public BO.o51Tag Load(int pid);
        public IEnumerable<BO.o51Tag> GetList(BO.myQuery mq);
        public int Save(BO.o51Tag rec);
    }
    class o51TagBL : BaseBL, Io51TagBL
    {
        public o51TagBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,o51_o53.o53Name," + _db.GetSQL1_Ocas("o51") + " FROM o51Tag a LEFT OUTER JOIN o53TagGroup o51_o53 ON a.o53ID=o51_o53.o53ID";
        }
        public BO.o51Tag Load(int pid)
        {
            return _db.Load<BO.o51Tag>(string.Format("{0} WHERE a.o51ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.o51Tag> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.o51Tag>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.o51Tag rec)
        {
            
            if (GetList(new BO.myQuery("o51Tag")).Where(p=>p.pid !=rec.pid && p.o51Name.ToLower() == rec.o51Name.Trim().ToLower()).Count()>0)
            {
                _mother.CurrentUser.AddMessage("Štítek s tímto názvem již existuje.");
                return 0;
            }
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.o51ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("o53ID", rec.o53ID, true);
            p.AddString("o51Name", rec.o51Name);
            p.AddString("o51Code", rec.o51Code);
            p.AddString("o51Entities", rec.o51Entities);


            return _db.SaveRecord("o51Tag", p.getDynamicDapperPars(), rec);
        }
    }
}
