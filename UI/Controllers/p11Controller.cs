﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p11Controller : BaseController
    {
        public IActionResult Index(int pid,double SimulateUnitsCount,int p27ID_Simulation)
        {

            var v = new Models.p11PreviewViewModel();
            v.Rec = Factory.p11ClientProductBL.Load(pid);
            if (v.Rec == null) return RecNotFound(v);
            var tg = Factory.o51TagBL.GetTagging("p11", pid);
            v.Rec.TagHtml = tg.TagHtml;
            v.RecP10 = Factory.p10MasterProductBL.Load(v.Rec.p10ID_Master);
            v.RecP21 = Factory.p21LicenseBL.Load(v.Rec.p21ID);

            if (SimulateUnitsCount > 0)
            {
                v.SimulateUnitsCount = SimulateUnitsCount;
                v.p27ID_Simulation = p27ID_Simulation;
                v.p27Name_Simulation = Factory.p27MszUnitBL.Load(p27ID_Simulation).p27Name;
                v.SimulationResult = Factory.p12ClientTpvBL.Simulate_Total_Duration(v.Rec.p12ID, v.SimulateUnitsCount*v.Rec.p11RecalcUnit2Kg,v.p27ID_Simulation);
            }
            return View(v);

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

            var v = new Models.p11RecordViewModel();

            if (pid > 0)
            {
                v.Rec = Factory.p11ClientProductBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var tg = Factory.o51TagBL.GetTagging("p11", pid);
                v.TagPids = tg.TagPids;
                v.TagNames = tg.TagNames;
                v.TagHtml = tg.TagHtml;

            }
            else
            {

                v.Rec = new BO.p11ClientProduct();
                v.Rec.entity = "p11";
                
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);


            if (isclone) {
                v.Toolbar.MakeClone();
                v.Rec.p11Code += "-COPY";
            }

            if (v.Rec.pid == 0)
            {
                if (Factory.p21LicenseBL.GetList(new BO.myQuery("p21License")).Where(p => p.p21PermissionFlag == BO.p21PermENUM.Extend || p.p21PermissionFlag == BO.p21PermENUM.Full).Count() == 0)
                {
                    return this.StopPage(true, "Systém nepovolí uložit produkt bez vazby na vzorový Master produkt, protože ani jedna z vašich licencí k tomu nemá oprávnění.");
                    //Factory.CurrentUser.AddMessage("Systém nepovolí uložit produkt bez vazby na vzorový Master produkt, protože ani jedna z vašich licencí k tomu nemá oprávnění.", "warning");
                }
            }


            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p11RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p11ClientProduct c = new BO.p11ClientProduct();
                if (v.Rec.pid > 0) c = Factory.p11ClientProductBL.Load(v.Rec.pid);

                c.b02ID = v.Rec.b02ID;
                c.p11Code = v.Rec.p11Code;
                c.p11Name = v.Rec.p11Name;
                c.p11Memo = v.Rec.p11Memo;
                c.b02ID = v.Rec.b02ID;
                c.p11TypeFlag = v.Rec.p11TypeFlag;
                c.p12ID = v.Rec.p12ID;
                c.p10ID_Master = v.Rec.p10ID_Master;
                c.p11UnitPrice = v.Rec.p11UnitPrice;
                c.p20ID = v.Rec.p20ID;
                c.p20ID_Pro = v.Rec.p20ID_Pro;
                c.p11RecalcUnit2Kg = v.Rec.p11RecalcUnit2Kg;
                c.p11PackagingCode = v.Rec.p11PackagingCode;
                c.p11Davka = v.Rec.p11Davka;
                c.p11DavkaMin = v.Rec.p11DavkaMin;
                c.p11DavkaMax = v.Rec.p11DavkaMax;
                c.p11SalesPerMonth = v.Rec.p11SalesPerMonth;
                c.p11UnitsPerPalette = v.Rec.p11UnitsPerPalette;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.p11ClientProductBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p11", v.Rec.pid, v.TagPids);
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