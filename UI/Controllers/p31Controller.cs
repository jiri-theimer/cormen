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
            v.CurrentP31ID = p31id;
            if (String.IsNullOrEmpty(d) == true)
            {
                v.CurrentDate = DateTime.Today;
            }
            else
            {
                string[] arr=d.Split(".");
                v.CurrentDate = new DateTime(int.Parse(arr[2]), int.Parse(arr[1]), int.Parse(arr[0]));
                //v.CurrentDate = DateTime.ParseExact(d, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            v.Rec = Factory.p31CapTemplateBL.Load(p31id);

            return View(v);
        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p31RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p31CapTemplateBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p31CapTemplate();                
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
                BO.p31CapTemplate c = new BO.p31CapTemplate();
                if (v.Rec.pid > 0) c = Factory.p31CapTemplateBL.Load(v.Rec.pid);

                
                c.p31Name = v.Rec.p31Name;
                
               
                v.Rec.pid = Factory.p31CapTemplateBL.Save(c);
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