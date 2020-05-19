using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p31Controller : BaseController
    {
        public IActionResult p31Timeline(int p31id,string d)
        {            
            var v = new Models.p31TimelineViewModel();
            if (p31id == 0)
            {
                var lis=Factory.p31CapacityFondBL.GetList(new BO.myQuery("p31CapacityFond"));
                if (lis.Count() == 0)
                {
                    return (this.StopPage(false, "Nejdříve musíte založit minimálně jeden kapacitní fond."));                    
                }
                p31id = lis.First().pid;
            }
            
            v.RecP31 = Factory.p31CapacityFondBL.Load(p31id);
            if (String.IsNullOrEmpty(d) == true)
            {
                v.CurrentDate = DateTime.Today;
            }
            else
            {                
                v.CurrentDate = BO.BAS.String2Date(d);
                //v.CurrentDate = DateTime.ParseExact(d, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            v.Rec = Factory.p31CapacityFondBL.Load(p31id);
            DateTime d1 = new DateTime(v.CurrentDate.Year, v.CurrentDate.Month, 1);
            v.lisP33 = Factory.p31CapacityFondBL.GetCells(p31id, d1, d1.AddMonths(1).AddDays(-1));
            return View(v);
        }
       public BO.Result SaveCells(int p31id, string d, List<BO.fondcell> cells)
        {           
            var lis = new List<BO.p33CapacityTimeline>();
            var d0 = BO.BAS.String2Date(d);
            foreach (BO.fondcell cell in cells){
                var d1 = new DateTime(d0.Year, d0.Month, cell.dayindex);
                var d2 = d1.AddHours(cell.hour).AddMinutes(cell.minute);
                var c = new BO.p33CapacityTimeline() { p31ID = p31id, p33Day = cell.dayindex, p33Hour = cell.hour, p33Minute = cell.minute, p33Date = d1, p33DateTime = d2 };
                lis.Add(c);
            }
            if (lis.Count() == 0)
            {
                return new BO.Result(true, "K uložení není ani jedna buňka");

            }
            int x=Factory.p31CapacityFondBL.SaveCells(lis);
            if (x == 0)
            {
                return new BO.Result(true, "Ani jedna buňka nebyla uložena.");
            }
            else
            {
                return new BO.Result(false, string.Format("Počet uložených buněk: {0}, počet chyb: {1}.",x,lis.Count-x));
            }
            

        }
        public BO.Result ClearCells(int p31id, string d, List<BO.fondcell> cells)
        {           
            var lis = new List<BO.p33CapacityTimeline>();
            var d0 = BO.BAS.String2Date(d);
            foreach (BO.fondcell cell in cells)
            {
                var d1 = new DateTime(d0.Year, d0.Month, cell.dayindex);
                var d2 = d1.AddHours(cell.hour).AddMinutes(cell.minute);
                var c = new BO.p33CapacityTimeline() { p31ID = p31id, p33Day = cell.dayindex, p33Hour = cell.hour, p33Minute = cell.minute, p33Date = d1, p33DateTime = d2 };
                lis.Add(c);
            }
            if (lis.Count() == 0)
            {
                return new BO.Result(true, "K vyčištění není ani jedna buňka");

            }
            int x = Factory.p31CapacityFondBL.ClearCells(lis);
            return new BO.Result(false, string.Format("Počet vyčištěných buněk: {0}, počet chyb: {1}.", x, lis.Count - x));


        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p31RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p31CapacityFondBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p31CapacityFond();                
                v.Rec.entity = "p31";

            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p31RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p31CapacityFond c = new BO.p31CapacityFond();
                if (v.Rec.pid > 0) c = Factory.p31CapacityFondBL.Load(v.Rec.pid);

                
                c.p31Name = v.Rec.p31Name;
                c.j02ID_Owner = v.Rec.j02ID_Owner;
               
                v.Rec.pid = Factory.p31CapacityFondBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }

            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            this.Notify_RecNotSaved();
            return View(v);
        }
    }
}