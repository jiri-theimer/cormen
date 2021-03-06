﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;


namespace UI.Controllers
{
    public class b02Controller : BaseController
    {

        //WORKFLOW STAV
        public IActionResult StatusBatchUpdate(string prefix,string pids)
        {
            var v = new b02BatchUpdateViewModel() { pids = pids,prefix=prefix,Entity=BL.TheEntities.ByPrefix(prefix).TableName };
            if (string.IsNullOrEmpty(prefix) == true)
            {
                return this.StopPage(true, "prefix missing.");
            }
            if (BO.BAS.ConvertString2ListInt(pids).Count == 0)
            {
                return this.StopPage(true, "Na vstupu chybí výběr záznamů.");
            }

            return View(v);
        }
        [HttpPost]
        public IActionResult StatusBatchUpdate(Models.b02BatchUpdateViewModel v, string oper)
        {            
            if (oper == "update" && v.b02ID==0)
            {
                this.AddMessage("Musíte vybrat cílový stav.");
                return View(v);
            }
            if (oper == "clear")
            {
                v.b02ID = 0;
            }
            if (ModelState.IsValid)
            {
                var arr = BO.BAS.ConvertString2ListInt(v.pids);
                if (Factory.b02StatusBL.StatusBatchUpdate(v.prefix, arr, v.b02ID))
                {
                    v.SetJavascript_CallOnLoad(v.b02ID);
                }

            }
            return View(v);
        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.b02RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.b02StatusBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.b02Status();
                v.Rec.b02Code = Factory.CBL.EstimateRecordCode("b02");
                v.Rec.entity = "b02";
            }
          
            v.Toolbar = new MyToolbarViewModel(v.Rec);            
            if (isclone) { v.Toolbar.MakeClone(); v.Rec.b02Code = Factory.CBL.EstimateRecordCode("b02"); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.b02RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.b02Status c = new BO.b02Status();
                if (v.Rec.pid > 0) c = Factory.b02StatusBL.Load(v.Rec.pid);

                c.b02Code = v.Rec.b02Code;
                c.b02Name = v.Rec.b02Name;
                c.b02Entity = v.Rec.b02Entity;
                c.b02Ordinary = v.Rec.b02Ordinary;
                c.b02Memo = v.Rec.b02Memo;
                c.b02MoveFlag = v.Rec.b02MoveFlag;
                c.b02StartFlag = v.Rec.b02StartFlag;
                c.b02MoveBySql = v.Rec.b02MoveBySql;
                if (v.Rec.b02Color== "#000000")
                {
                    v.Rec.b02Color = "";
                }
                c.b02Color = v.Rec.b02Color;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.b02StatusBL.Save(c);
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