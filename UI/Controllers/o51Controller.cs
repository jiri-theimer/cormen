using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL;
using BO;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class o51Controller : BaseController
    {
        ///štítky
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.o51RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.o51TagBL.Load(pid);
                if (v.Rec.o51Entities != null)
                {
                    v.SelectedEntities = new List<int>();
                    foreach(var s in BO.BAS.ConvertString2List(v.Rec.o51Entities))
                    {
                        v.SelectedEntities.Add(BL.TheEntities.ByPrefix(s).IntPrefix);
                    }
                }
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                
            }
            else
            {
                v.Rec = new BO.o51Tag();
                v.Rec.o51Code = Factory.CBL.EstimateRecordCode("o51");
                v.Rec.entity = "o51";

            }
            RefreshState(ref v);
            if (isclone) { v.Toolbar.MakeClone(); v.Rec.o51Code = Factory.CBL.EstimateRecordCode("o51"); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.o51RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.o51Tag c = new BO.o51Tag();
                if (v.Rec.pid > 0) c = Factory.o51TagBL.Load(v.Rec.pid);
                c.o53ID = v.Rec.o53ID;
                c.o51Code = v.Rec.o51Code;
                c.o51Name = v.Rec.o51Name;
                var prefixes = new List<string>();
                foreach(var x in v.SelectedEntities.Where(p=>p>0))
                {
                    prefixes.Add(TheEntities.ByIntPrefix(x).Prefix);
                }
                c.o51Entities = String.Join(",", prefixes);

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.o51TagBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }

            }

            RefreshState(ref v);
            this.Notify_RecNotSaved();
            return View(v);
        }

        private void RefreshState(ref o51RecordViewModel v)
        {
            v.ApplicableEntities = new List<TheEntity>();
            v.ApplicableEntities.Add(BL.TheEntities.ByPrefix("p41"));
            v.ApplicableEntities.Add(BL.TheEntities.ByPrefix("j02"));
            v.ApplicableEntities.Add(BL.TheEntities.ByPrefix("p28"));
            v.ApplicableEntities.Add(BL.TheEntities.ByPrefix("p51"));
            v.ApplicableEntities.Add(BL.TheEntities.ByPrefix("p21"));
            v.ApplicableEntities.Add(BL.TheEntities.ByPrefix("p11"));
            v.ApplicableEntities.Add(BL.TheEntities.ByPrefix("p10"));
            v.ApplicableEntities.Add(BL.TheEntities.ByPrefix("p26"));
            v.ApplicableEntities.Add(BL.TheEntities.ByPrefix("o23"));
            v.ApplicableEntities.Add(BL.TheEntities.ByPrefix("p19"));
            v.ApplicableEntities.Add(BL.TheEntities.ByPrefix("p18"));




            v.Toolbar = new MyToolbarViewModel(v.Rec);
        }
    }

    
}