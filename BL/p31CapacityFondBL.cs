using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ip31CapacityFondBL
    {
        public BO.p31CapacityFond Load(int pid);
        public IEnumerable<BO.p31CapacityFond> GetList(BO.myQuery mq);
        public int Save(BO.p31CapacityFond rec);
        public int SaveCells(List<BO.p33CapacityTimeline> cells);
        public int ClearCells(List<BO.p33CapacityTimeline> cells);
        public IEnumerable<BO.p33CapacityTimeline> GetCells(int p31id, DateTime d1, DateTime d2);
    }
    class p31CapacityFondBL : BaseBL, Ip31CapacityFondBL
    {

        public p31CapacityFondBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1()
        {
            return "SELECT a.*,dbo.j02_show_as_owner(a.j02ID_Owner) as RecordOwner," + _db.GetSQL1_Ocas("p31") + " FROM " + BL.TheEntities.ByPrefix("p31").SqlFrom;
        }
        public BO.p31CapacityFond Load(int pid)
        {
            return _db.Load<BO.p31CapacityFond>(string.Format("{0} WHERE a.p31ID=@pid", GetSQL1()), new { pid = pid });
        }
        public IEnumerable<BO.p31CapacityFond> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p31CapacityFond>(fq.FinalSql, fq.Parameters);

        }
        public IEnumerable<BO.p33CapacityTimeline> GetCells(int p31id,DateTime d1,DateTime d2)
        {
            
            return _db.GetList<BO.p33CapacityTimeline>("select * from p33CapacityTimeline WHERE p31ID=@p31id AND p33Date BETWEEN @d1 AND @d2", new {p31id=p31id, d1 = d1, d2 = d2 });

        }

        public int Save(BO.p31CapacityFond rec)
        {
            if (rec.p31DayHour1>= rec.p31DayHour2)
            {
                _mother.CurrentUser.AddMessage("Poslední hodina musí být větší než první hodina.");
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.p31ID);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddString("p31Name", rec.p31Name);
            p.AddInt("p31DayHour1", rec.p31DayHour1);
            p.AddInt("p31DayHour2", rec.p31DayHour2);

            return _db.SaveRecord("p31CapacityFond", p.getDynamicDapperPars(), rec);
        }
        public int SaveCells(List<BO.p33CapacityTimeline> cells)
        {
            int sucs = 0;

            foreach(var rec in cells)
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.p33ID);               
                p.AddInt("p31ID", rec.p31ID, true);
                p.AddDateTime("p33Date", rec.p33Date);
                p.AddDateTime("p33DateTime", rec.p33DateTime);
                p.AddInt("p33Day", rec.p33Day);
                p.AddInt("p33Hour", rec.p33Hour);
                p.AddInt("p33Minute", rec.p33Minute);


                if ( _db.SaveRecord("p33CapacityTimeline", p.getDynamicDapperPars(), rec) > 0)
                {
                    sucs += 1;
                }
                
            }

            return sucs;
        }
        public int ClearCells(List<BO.p33CapacityTimeline> cells)
        {
            int sucs = 0;
            string strGUID = BO.BAS.GetGuid();

            foreach (var rec in cells)
            {
                
                if (_db.RunSql("UPDATE p33CapacityTimeline SET ValidUntil=GETDATE(),UserUpdate=@guid WHERE p31ID=@p31id AND p33DateTime=@d", new { p31id = rec.p31ID, d = rec.p33DateTime,guid=strGUID }))
                {
                    sucs += 1;
                }


            }

            _db.RunSql("DELETE FROM p33CapacityTimeline WHERE UserUpdate=@guid AND ValidUntil<=GETDATE()",new { guid = strGUID });

            return sucs;
        }
    }
}
