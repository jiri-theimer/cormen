using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI
{
    public class TaskSimulation
    {
        private readonly BL.Factory _f;
        private List<BO.p41Task> _tasks;
        private List<BO.p27MszUnit> _lisP27;
        private DateTime _dat0;

        public DateTime Date0
        {
            get
            {
                return _dat0;
            }
            set
            {
                _dat0 = value;
            }
        }
        public TaskSimulation(BL.Factory f)
        {
            _dat0 = DateTime.Now;
            _f = f;
            _tasks = new List<BO.p41Task>();

            var mq = new BO.myQuery("p27MszUnit");
            mq.IsRecordValid = true;
            _lisP27 = _f.p27MszUnitBL.GetList(mq).ToList();
        }

        
        public List<BO.p41Task> getTasksByP51(int p51id)
        {
            foreach (var kotel in _lisP27)
            {
                kotel.DateInsert = _dat0;
            }
            var lisP52 = _f.p52OrderItemBL.GetList(p51id);
            foreach(var rec in lisP52)
            {
                handle_create_by_p52(rec.pid);
                _dat0 = _tasks.Last().p41PlanEnd.AddSeconds(1);
            }
            
            return _tasks;
        }
        public List<BO.p41Task> getTasksByP52(int p52id)
        {
            foreach (var kotel in _lisP27)
            {
                kotel.DateInsert = _dat0;
            }
            handle_create_by_p52(p52id);
            return _tasks;
        }

        private void handle_create_by_p52(int p52id)
        {
            if (_lisP27.Count == 0 || p52id == 0)
            {
                return;
            }
            var cP52 = _f.p52OrderItemBL.Load(p52id);
            var cP11 = _f.p11ClientProductBL.Load(cP52.p11ID);

            double dblTotalKG = cP52.Recalc2Kg;
            double dblUsedKG = 0;
            DateTime dat0 = _dat0;
            int x = 1;
            


            while (dblUsedKG < dblTotalKG)
            {
                foreach (var kotel in _lisP27)
                {
                    var rec = new BO.p41Task();
                    rec.p52ID = p52id;
                    rec.p52Code = cP52.p52Code;
                    rec.p27ID = kotel.pid;
                    rec.p27Name = kotel.p27Name;
                    rec.p41Name = cP11.p11Name + " [" + cP11.p11Code + "]";
                    rec.p41PlanUnitsCount = kotel.p27Capacity;
                    if (rec.p41PlanUnitsCount + dblUsedKG > dblTotalKG)
                    {
                        rec.p41PlanUnitsCount = dblTotalKG - dblUsedKG;
                    }
                    double dur = _f.p12ClientTpvBL.Simulate_Total_Duration(cP11.p12ID, rec.p41PlanUnitsCount, kotel.pid);
                    dat0 = kotel.DateInsert;
                    rec.p41PlanStart = dat0;
                    rec.p41PlanEnd = rec.p41PlanStart.AddSeconds(dur*60);
                    rec.p41Code = cP52.p52Code.Replace("R", "T") + "." + BO.BAS.RightString("000" + x.ToString(), 3);
                    kotel.DateInsert = rec.p41PlanEnd.AddSeconds(1);

                    _tasks.Add(rec);

                    dblUsedKG += rec.p41PlanUnitsCount;
                    if (dblUsedKG >= dblTotalKG)
                    {
                        return;
                    }

                    x += 1;

                }
            }



        }
    }
}
