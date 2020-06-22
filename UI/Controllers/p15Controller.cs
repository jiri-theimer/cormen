using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p15Controller : BaseController
    {
        public IActionResult Record(int pid, int p12id, bool isclone)
        {
            if (!this.TestIfUserEditor(false, true))
            {
                return this.StopPageCreateEdit(true);
            }


            var v = new Models.p15RecordViewModel();

            if (Factory.p21LicenseBL.GetList(new BO.myQuery("p21License")).Where(p => p.p21PermissionFlag == BO.p21PermENUM.Extend || p.p21PermissionFlag == BO.p21PermENUM.Full).Count() == 0)
            {
                return this.StopPage(true,"Systém nepovolí uložit vlastní produkt, protože ani jedna z vašich licencí k tomu nemá oprávnění.");
            }

            if (pid > 0)
            {
                v.Rec = Factory.p15ClientOperBL.Load(pid);

                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                if (isclone)
                {
                    v.Rec.p15RowNum += 1;
                }

                v.RecP12 = Factory.p12ClientTpvBL.Load(v.Rec.p12ID);


            }
            else
            {
                v.Rec = new BO.p15ClientOper();
                v.RecP12 = Factory.p12ClientTpvBL.Load(p12id);
                v.Rec.p12ID = p12id;
                v.Rec.entity = "p15";


            }

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.Toolbar.AllowArchive = false;
            if (isclone)
            {
                v.Toolbar.MakeClone();

            }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p15RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p15ClientOper c = new BO.p15ClientOper();
                if (v.Rec.pid > 0) c = Factory.p15ClientOperBL.Load(v.Rec.pid);

                c.p12ID = v.Rec.p12ID;
                c.p19ID = v.Rec.p19ID;
                c.p18ID = v.Rec.p18ID;
                c.p15RowNum = v.Rec.p15RowNum;
                c.p15OperParam = v.Rec.p15OperParam;
                c.p15OperNum = v.Rec.p15OperNum;
                c.p15UnitsCount = v.Rec.p15UnitsCount;
                
                c.p15DurationOper = v.Rec.p15DurationOper;
                c.p15DurationPreOper = v.Rec.p15DurationPreOper;
                c.p15DurationPostOper = v.Rec.p15DurationPostOper;



                v.Rec.pid = Factory.p15ClientOperBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }

            }

            v.RecP12 = Factory.p12ClientTpvBL.Load(v.Rec.p12ID);
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.Toolbar.AllowArchive = false;
            this.Notify_RecNotSaved();
            return View(v);
        }

        public bool PrecislujOperNum(int p15id_start)
        {
            Factory.p15ClientOperBL.PrecislujOperNum(p15id_start);
            return true;
        }
    }
}