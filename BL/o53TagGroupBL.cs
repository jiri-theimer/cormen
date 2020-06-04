using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Io53TagGroupBL
    {
        public BO.o53TagGroup Load(int pid);
        public IEnumerable<BO.o53TagGroup> GetList(BO.myQuery mq);
        public int Save(BO.o53TagGroup rec);
    }
    class o53TagGroupBL :BaseBL, Io53TagGroupBL
    {
     
        public o53TagGroupBL(BL.Factory mother):base(mother)
        {
            
        }
      
       
        private string GetSQL1()
        {
            return "SELECT a.*," + _db.GetSQL1_Ocas("o53") + " FROM o53TagGroup a";
        }
        public BO.o53TagGroup Load(int pid)
        {
            return _db.Load<BO.o53TagGroup>(string.Format("{0} WHERE a.o53ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.o53TagGroup> GetList(BO.myQuery mq)
        {
            mq.explicit_orderby = "a.o53Ordinary,a.o53Name";
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser) ;
            return _db.GetList<BO.o53TagGroup>(fq.FinalSql, fq.Parameters);

        }

        public int Save(BO.o53TagGroup rec)
        {
            if (String.IsNullOrEmpty(rec.o53Entities)==true)
            {
                _mother.CurrentUser.AddMessage("Chybí vazba na entity.");
                return 0;
            }
            var p = new DL.Params4Dapper();
         
            p.AddInt("pid", rec.o53ID);
            p.AddString("o53Name", rec.o53Name);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddString("o53Entities", rec.o53Entities);
            p.AddInt("o53Ordinary", rec.o53Ordinary);
            p.AddBool("o53IsMultiSelect", rec.o53IsMultiSelect);

            return _db.SaveRecord("o53TagGroup", p.getDynamicDapperPars(), rec);
        }
    }
}
