using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            
            return View(v);
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