using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p12Controller : BaseController
    {
        public IActionResult Index(int pid)
        {
            var v = new Models.p12PreviewViewModel();


            v.Rec = Factory.p12ClientTpvBL.Load(pid);

            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                if (v.Rec.p13ID_Master > 0)
                {
                    v.RecP13 = Factory.p13MasterTpvBL.Load(v.Rec.p13ID_Master);
                }
                v.RecP21 = Factory.p21LicenseBL.Load(v.Rec.p21ID);
                return View(v);
            }

        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (Factory.CurrentUser.j03EnvironmentFlag == 1)
            {
                return this.StopPageClientPageOnly(true);
            }
            if (!this.TestIfUserEditor(false, true))
            {
                return this.StopPageCreateEdit(true);
            }
           
            var v = new Models.p12RecordViewModel();

            if (pid > 0)
            {                

                v.Rec = Factory.p12ClientTpvBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                BO.p21License cP21 = Factory.p21LicenseBL.Load(v.Rec.p21ID);
                if (isclone == false && v.Rec.p13ID_Master>0)
                {
                    return this.StopPage(true, "Recepturu s Master vzorem nelze upravovat.<hr>Zkopírujte si ji do nové receptury, kterou můžete upravovat.");
                }
                if (cP21.p21PermissionFlag != BO.p21PermENUM.Independent2Master)
                {
                    return this.StopPage(true, string.Format("S licencí typu {2} [{0} - {1}]  nemáte oprávnění zakládat vlastní receptury.", cP21.p21Code,cP21.p21Name,cP21.PermFlagAlias));
                }

                var mq = new BO.myQuery("p15ClientOper");
                mq.p12id = v.Rec.pid;
                v.lisP15 = Factory.p15ClientOperBL.GetList(mq).ToList();
                for (var i = 0; i < v.lisP15.Count(); i++)
                {
                    v.lisP15[i].TempRecGuid = BO.BAS.GetGuid();
                    v.lisP15[i].TempRecDisplay = "table-row";
                }

            }
            else
            {
                
                v.Rec = new BO.p12ClientTpv();
                v.Rec.entity = "p12";
                if (Factory.p21LicenseBL.GetList(new BO.myQuery("p21License")).Where(p => p.p21PermissionFlag == BO.p21PermENUM.Independent2Master).Count() == 0)
                {
                    Factory.CurrentUser.AddMessage("Systém nepovolí uložit vlastní recepturu, protože ani jedna z vašich licencí k tomu nemá oprávnění.", "warning");
                }

                v.lisP15 = new List<BO.p15ClientOper>();
                v.lisP15.Add(new BO.p15ClientOper() { TempRecDisplay = "table-row", TempRecGuid = BO.BAS.GetGuid() });
            }

            
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.Toolbar.MakeClone();
                v.Rec.p12Code += "-COPY";
                for (var i = 0; i < v.lisP15.Count(); i++)
                {
                    v.lisP15[i].p15ID = 0;
                    v.lisP15[i].pid = 0;
                }
            }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p12RecordViewModel v, string rec_oper, string rec_guid)
        {
            if (rec_oper != null)
            {
                if (rec_oper == "add")
                {
                    if (v.lisP15 == null) v.lisP15 = new List<BO.p15ClientOper>();
                    v.lisP15.Add(new BO.p15ClientOper() { TempRecDisplay = "table-row", TempRecGuid = BO.BAS.GetGuid(), p15RowNum = v.lisP15.Count() + 1 });

                }
                if (rec_oper == "postback")
                {
                    //pouze postback

                }

                v.Toolbar = new MyToolbarViewModel(v.Rec);

                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.p12ClientTpv c = new BO.p12ClientTpv();
                if (v.Rec.pid > 0) c = Factory.p12ClientTpvBL.Load(v.Rec.pid);

                c.p12Code = v.Rec.p12Code;
                c.p12Name = v.Rec.p12Name;
                c.p12Memo = v.Rec.p12Memo;
                c.p21ID = v.Rec.p21ID;
                c.p25ID = v.Rec.p25ID;
                int x = 1;
                foreach (var row in v.lisP15.OrderBy(p => p.p15RowNum))
                {
                    row.p15RowNum = x;  //narovnat rownum na postupku od jedničky
                    x += 1;
                }

                v.Rec.pid = Factory.p12ClientTpvBL.Save(c, v.lisP15);
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