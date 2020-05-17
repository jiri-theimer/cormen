using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p28Controller : BaseController
    {
        public IActionResult Index(int pid)
        {
            var v = new Models.p28PreviewViewModel();

            v.Rec = Factory.p28CompanyBL.Load(pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                if (Factory.CurrentUser.j03EnvironmentFlag == 1)
                {
                    if (v.Rec.p28CloudID != null || Factory.p28CompanyBL.LoadValidSwLicense(pid) != null)
                    {
                        v.IsPossible2SetupCloudID = true;
                    }
                }
                var mq = new BO.myQuery("j02Person");
                mq.p28id = pid;
                v.Persons = Factory.j02PersonBL.GetList(mq);
                
                return View(v);
            }
            

        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p28RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p28CompanyBL.Load(pid);
                if (v.Rec == null)
                {                    
                    return RecNotFound(v);
                }
                if (!this.TestIfRecordEditable(v.Rec.j02ID_Owner))
                {
                    return this.StopPageEdit(true);
                }

            }
            else
            {

                v.Rec = new BO.p28Company();
                v.Rec.entity = "p28";
                v.Rec.p28Code = Factory.CBL.EstimateRecordCode("p28");
                v.Rec.j02ID_Owner = Factory.CurrentUser.j02ID;
                v.Rec.RecordOwner = Factory.CurrentUser.FullName;
                v.FirstPerson = new BO.j02Person();
                v.IsFirstPerson = true;
                
            }

            
            v.Toolbar = new MyToolbarViewModel(v.Rec);            
            if (isclone) {
                v.Toolbar.MakeClone();
                v.Rec.p28Code = Factory.CBL.EstimateRecordCode("p28");
            }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p28RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p28Company c = new BO.p28Company();
                if (v.Rec.pid > 0) c = Factory.p28CompanyBL.Load(v.Rec.pid);
                
                c.p28Code = v.Rec.p28Code;
                c.p28Name = v.Rec.p28Name;
                c.p28ShortName = v.Rec.p28ShortName;
                c.p28RegID = v.Rec.p28RegID;
                c.p28VatID = v.Rec.p28VatID;
                c.p28Street1 = v.Rec.p28Street1;
                c.p28City1 = v.Rec.p28City1;
                c.p28PostCode1 = v.Rec.p28PostCode1;
                c.p28Country1 = v.Rec.p28Country1;
                c.p28Street2 = v.Rec.p28Street2;
                c.p28City2 = v.Rec.p28City2;
                c.p28PostCode2 = v.Rec.p28PostCode2;
                c.p28Country2 = v.Rec.p28Country2;
                c.j02ID_Owner = v.Rec.j02ID_Owner;
               
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                
                if (c.pid>0 || (c.pid==0 && v.IsFirstPerson == false))
                {
                    v.FirstPerson = null;
                    //cFirstPerson = new BO.j02Person() { j02TitleBeforeName = v.FirstPerson.j02TitleBeforeName,j02FirstName=v.FirstPerson.j02FirstName,j02LastName=v.FirstPerson.j02LastName,j02TitleAfterName=v.FirstPerson.j02TitleAfterName,j02Email=v.FirstPerson.j02Email };
                }

                v.Rec.pid = Factory.p28CompanyBL.Save(c, v.FirstPerson);
                if (v.Rec.pid > 0)
                {

                    v.SetJavascript_CallOnLoad(v.Rec.pid,"p28");
                    return View(v);                    
                }
                
               
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
          
            this.Notify_RecNotSaved();
            return View(v);


        }

        public BO.Result UpdateCloudID(int p28id, string cloudid)
        {
            if (Factory.p28CompanyBL.UpdateCloudID(p28id, cloudid))
            {
                return new BO.Result(false);
            }
            else
            {
                return new BO.Result(true, Factory.CurrentUser.Messages4Notify[0].Value);
            }
        }
    }
}