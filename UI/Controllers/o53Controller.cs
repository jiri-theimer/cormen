using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BL;
using BO;
using UI.Models;

namespace UI.Controllers
{
    public class o53Controller : BaseController
        
    {
        private readonly BL.TheColumnsProvider _cp;
        public o53Controller(BL.TheColumnsProvider cp)
        {
            _cp = cp;
        }
        ///KATEGORIE
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.o53RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.o53TagGroupBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                if (v.Rec.o53Entities != null)
                {
                    v.SelectedEntities = new List<int>();
                    foreach (var s in BO.BAS.ConvertString2List(v.Rec.o53Entities))
                    {
                        v.SelectedEntities.Add(BL.TheEntities.ByPrefix(s).IntPrefix);
                    }
                }

            }
            else
            {
                v.Rec = new BO.o53TagGroup();                
                v.Rec.entity = "o53";
                v.Rec.o53IsMultiSelect = true;
            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.ApplicableEntities = GetApplicableEntities();
            if (isclone) { v.Toolbar.MakeClone(); }
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.o53RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.o53TagGroup c = new BO.o53TagGroup();
                if (v.Rec.pid > 0) c = Factory.o53TagGroupBL.Load(v.Rec.pid);

                
                c.o53Name = v.Rec.o53Name;
                var prefixes = new List<string>();
                foreach (var x in v.SelectedEntities.Where(p => p > 0))
                {
                    prefixes.Add(BL.TheEntities.ByIntPrefix(x).Prefix);
                }
                c.o53Entities = String.Join(",", prefixes);
                c.o53IsMultiSelect = v.Rec.o53IsMultiSelect;
                c.o53Ordinary = v.Rec.o53Ordinary;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.o53TagGroupBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    _cp.Refresh();   //obnovit názvy sloupců kategorií

                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }
                            
            }
            
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.ApplicableEntities = GetApplicableEntities();
            this.Notify_RecNotSaved();
            return View(v);
            
        }


        private List<BO.TheEntity> GetApplicableEntities()
        {
            var lis = new List<TheEntity>();
            lis.Add(BL.TheEntities.ByPrefix("p41"));
            lis.Add(BL.TheEntities.ByPrefix("j02"));
            lis.Add(BL.TheEntities.ByPrefix("p28"));
            lis.Add(BL.TheEntities.ByPrefix("p51"));
            lis.Add(BL.TheEntities.ByPrefix("p21"));
            lis.Add(BL.TheEntities.ByPrefix("p11"));
            lis.Add(BL.TheEntities.ByPrefix("p10"));
            lis.Add(BL.TheEntities.ByPrefix("p26"));
            lis.Add(BL.TheEntities.ByPrefix("o23"));
            lis.Add(BL.TheEntities.ByPrefix("p19"));
            lis.Add(BL.TheEntities.ByPrefix("p18"));
            lis.Add(BL.TheEntities.ByPrefix("p13"));
            lis.Add(BL.TheEntities.ByPrefix("p12"));
            return lis;
        }



    }
}