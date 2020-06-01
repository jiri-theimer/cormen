
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p13Controller : BaseController
    {
        

        public IActionResult Index(int pid)
        {
            var v = new Models.p13PreviewViewModel();
           

            v.Rec = Factory.p13MasterTpvBL.Load(pid);     
           
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                return View(v);
            }
           
        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p13RecordViewModel();
           
            if (pid > 0)
            {
                v.Rec = Factory.p13MasterTpvBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var mq = new BO.myQuery("p14MasterOper");
                mq.p13id = v.Rec.pid;
               
            }
            else
            {
                v.Rec = new BO.p13MasterTpv();
                v.Rec.entity = "p13";
                
            }

            
            v.Toolbar = new MyToolbarViewModel(v.Rec);            
            if (isclone) {
                v.Toolbar.MakeClone();
               
            }
            

            

            return View(v);
        }

        public BO.p18OperCode load_p18_record(int p18id)
        {
            return Factory.p18OperCodeBL.Load(p18id);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p13RecordViewModel v, bool applyonly)
        {
            
            if (ModelState.IsValid)
            {
                BO.p13MasterTpv c = new BO.p13MasterTpv();
                if (v.Rec.pid > 0) c = Factory.p13MasterTpvBL.Load(v.Rec.pid);

                c.p25ID = v.Rec.p25ID;
                c.p13Code = v.Rec.p13Code;
                c.p13Name = v.Rec.p13Name;
                c.p13Memo = v.Rec.p13Memo;

                //c.ValidUntil = v.Toolbar.GetValidUntil(c);
                //c.ValidFrom = v.Toolbar.GetValidFrom(c);
                


                v.Rec.pid = Factory.p13MasterTpvBL.Save(c);
                
                if (v.Rec.pid > 0)
                {
                    if (applyonly == true)
                    {
                        return Record(v.Rec.pid, false);
                    }
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }
                
                
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec) ;
           
            this.Notify_RecNotSaved();
            return View(v);
        }


        
        }

        
        
    }
