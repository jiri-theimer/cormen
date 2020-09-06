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
            //_dat0 = DateTime.Now;
            _dat0 = new DateTime(2000, 1, 1);
            _f = f;
            _tasks = new List<BO.p41Task>();


        }

        private void InhaleListP27(int p52id, int p27id)
        {
            var mq = new BO.myQuery("p27MszUnit");
            mq.IsRecordValid = true;

            BO.p52OrderItem cP52 = _f.p52OrderItemBL.Load(p52id);
            BO.p11ClientProduct cP11 = _f.p11ClientProductBL.Load(cP52.p11ID);
            if (cP11.p10ID_Master > 0)
            {
                mq.p25id = _f.p10MasterProductBL.Load(cP11.p10ID_Master).p25ID; //z RecP10 se bere typ zařízení pro combo nabídku zařízení
            }
            else
            {
                if (cP11.p12ID > 0)
                {
                    mq.p25id = _f.p12ClientTpvBL.Load(cP11.p12ID).p25ID;    //vlastní klientská receptura
                }
            }

            _lisP27 = _f.p27MszUnitBL.GetList(mq).ToList();

            if (p27id > 0)
            {
                _lisP27 = _lisP27.Where(p => p.pid == p27id).ToList();
            }
        }

        private DateTime getStartPlanDatePerP27(DateTime dat0, BO.p27MszUnit kotel)
        {
            var d = dat0;
            if (d.Year == 2000 && kotel.p31ID > 0)   //default datum - je třeba ho nastavit buď podle kapacitního plánu zařízení nebo aktuální čas
            {
                d = DateTime.Today;
            }

            if (d.Hour==0 && d.Minute==0 && kotel.p31ID > 0)
            {
                var lisP33 = _f.p31CapacityFondBL.GetCells(kotel.p31ID, d, d.AddDays(10)).OrderBy(p=>p.p33DateTime);
                if (lisP33.Count() > 0)
                {
                    d = lisP33.First().p33DateTime; //první datum+čas podle kapacitního plánu stroje
                }
            }
            if (d.Year <= 2000)
            {
                d = new DateTime(); //aktuální čas, protože stroj nemá pro dnešní den kapacitní fond
            }
            return d;
        }
        public List<BO.p41Task> getTasksByP51(int p51id)
        {
            foreach (var kotel in _lisP27)
            {
                kotel.DateInsert = getStartPlanDatePerP27(_dat0, kotel);

            }
            var lisP52 = _f.p52OrderItemBL.GetList(p51id);
            foreach (var rec in lisP52)
            {
                handle_create_by_p52(rec.pid);
                if (_tasks.Count > 0)
                {
                    _dat0 = _tasks.Last().p41PlanEnd;
                }

            }


            return _tasks;
        }
        public List<BO.p41Task> getTasksByP52IDs(List<int> p52ids)
        {
            if (_lisP27 == null)
            {
                InhaleListP27(p52ids[0], 0);
            }
            foreach (var kotel in _lisP27)
            {
                kotel.DateInsert = getStartPlanDatePerP27(_dat0, kotel);
            }
            foreach(int intP52ID in p52ids)
            {
                var recP52 = _f.p52OrderItemBL.Load(intP52ID);
                if (recP52.p52DateNeeded != null)
                {
                    foreach (var kotel in _lisP27)
                    {
                        kotel.DateInsert = getStartPlanDatePerP27(Convert.ToDateTime(recP52.p52DateNeeded), kotel);
                    }
                }
                handle_create_by_p52(intP52ID);
            }
            return _tasks;
        }
        public List<BO.p41Task> getTasksByP52(int p52id, int p27id)
        {
            if (_lisP27 == null)
            {
                InhaleListP27(p52id, p27id);
            }

            foreach (var kotel in _lisP27)
            {
                kotel.DateInsert = getStartPlanDatePerP27(_dat0, kotel);
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

            double dblTotalKG = cP52.Recalc2Kg - cP52.p52Task_Kg;
            double dblUsedKG = 0;
            DateTime dat0 = _dat0;
            var mq = new BO.myQuery("p41Task");
            mq.p52id = p52id;
            int x = 1;
            string strLastCode = "";
            if (dblTotalKG <= 0)
            {
                _f.CurrentUser.AddMessage(string.Format("Položka objednávky [{0}] je již kompletně rozplánovaná.", cP52.p52Code));
            }

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
                    rec.p41Duration = dur;

                    rec.p41Code = _f.p41TaskBL.EstimateTaskCode(cP52.p52Code, x);
                    while (strLastCode == rec.p41Code)
                    {
                        x += 1;
                        rec.p41Code = _f.p41TaskBL.EstimateTaskCode(cP52.p52Code, x);
                    }

                    //kotel.DateInsert = rec.p41PlanEnd.AddSeconds(1);
                    kotel.DateInsert = rec.p41PlanEnd;

                    _tasks.Add(rec);

                    dblUsedKG += rec.p41PlanUnitsCount;
                    if (dblUsedKG >= dblTotalKG)
                    {
                        return;
                    }

                    x += 1;
                    strLastCode = rec.p41Code;
                }
            }



        }
    }
}
