﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p21Controller : BaseController
    {
        public IActionResult p21AppendRemove(string p10ids)
        {
            var v = new p21AppendRemove() { p10IDs = p10ids };
            if (BO.BAS.ConvertString2ListInt(p10ids).Count == 0)
            {
                return this.StopPage(true, "Na vstupu chybí výběr produktů.");
            }

            return View(v);
        }
        [HttpPost]
        public IActionResult p21AppendRemove(Models.p21AppendRemove v, string oper)
        {
            if (ModelState.IsValid)
            {
                var p10ids = BO.BAS.ConvertString2ListInt(v.p10IDs);
                if (oper == "append")
                {
                    if (Factory.p21LicenseBL.AppendP10IDs(v.p21ID, p10ids))
                    {
                        v.SetJavascript_CallOnLoad(v.p21ID);
                    }
                    
                    

                }
                if (oper == "remove")
                {

                    if (Factory.p21LicenseBL.RemoveP10IDs(v.p21ID, p10ids))
                    {
                        v.SetJavascript_CallOnLoad(v.p21ID);
                    }

                }
            }
            return View(v);
        }
        public IActionResult Index(int pid)
        {
            var v = new Models.p21PreviewViewModel();
            v.Rec = Factory.p21LicenseBL.Load(pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                var tg = Factory.o51TagBL.GetTagging("p21", pid);
                v.Rec.TagHtml = tg.TagHtml;
                
                return View(v);
            }

        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p21RecordViewModel();
            
            if (pid > 0)
            {
                v.Rec = Factory.p21LicenseBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var tg = Factory.o51TagBL.GetTagging("p21", pid);
                v.TagPids = tg.TagPids;
                v.TagNames = tg.TagNames;
                v.TagHtml = tg.TagHtml;

                
            }
            else
            {
                v.Rec = new BO.p21License();
                v.Rec.ValidFrom = DateTime.Today;
                v.Rec.ValidUntil = new DateTime(3000, 1, 1);
                v.Rec.p21Code = Factory.CBL.EstimateRecordCode("p21");
                v.Rec.p21PermissionFlag = BO.p21PermENUM.Standard;
                v.Rec.entity = "p21";
            }

            RefreshState(v);

            
                        
            if (isclone) { v.Toolbar.MakeClone(); v.Rec.p21Code = Factory.CBL.EstimateRecordCode("p21"); }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p21RecordViewModel v)
        {            
            if (ModelState.IsValid)
            {
                BO.p21License c = new BO.p21License();
                if (v.Rec.pid > 0) c = Factory.p21LicenseBL.Load(v.Rec.pid);

                c.p21Code = v.Rec.p21Code;
                c.p21Name = v.Rec.p21Name;
                c.p21Memo = v.Rec.p21Memo;
                c.b02ID = v.Rec.b02ID;
                c.p28ID = v.Rec.p28ID;
                
                c.p21Price = v.Rec.p21Price;
                c.p21PermissionFlag = v.Rec.p21PermissionFlag;

                c.ValidFrom = v.Rec.ValidFrom;
                c.ValidUntil = v.Rec.ValidUntil;


                v.Rec.pid = Factory.p21LicenseBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p21", v.Rec.pid, v.TagPids);
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }

            }
           
            
            RefreshState(v);
            
            this.Notify_RecNotSaved();
            return View(v);


        }


        
        public string GetAllP10IDs()
        {
            string s = string.Join(",", Factory.p10MasterProductBL.GetList(new BO.myQuery("p10MasterProduct")).Select(p => p.pid));
            return s;
        }

        public BO.Result CreateClientProducts(int p21id)
        {
            
            return Factory.p21LicenseBL.CreateClientProducts(p21id);
            
            
        }

        private void RefreshState(p21RecordViewModel v)
        {
            v.Toolbar = new MyToolbarViewModel(v.Rec);
           
                
            
        }



    }
}