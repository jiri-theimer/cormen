﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p41Controller : BaseController
    {

        public IActionResult p41AppendPo(int p41id,int p18flag)
        {
            var v = new Models.p41AppendPoViewModel();
            v.p41ID = p41id;
            v.p18flag = p18flag;
            v.SelectedP18IDs = new List<int>();
            RefreshState_p41AppendPo(ref v);

            var mq = new BO.myQuery("p44TaskOperPlan");
            mq.p41id = p41id;
            var lisP44 = Factory.p44TaskOperPlanBL.GetList(mq);
            foreach(var c in lisP44)
            {
                if (v.lisP18.Where(p => p.p18ID == c.p18ID).Count() > 0)
                {
                    v.SelectedP18IDs.Add(c.p18ID);
                }
            }

            return View(v);
           
        }
        [HttpPost]        
        public IActionResult p41AppendPo(Models.p41AppendPoViewModel v,string oper)
        {
            RefreshState_p41AppendPo(ref v);

            if (ModelState.IsValid)
            {
                if (oper=="save" && v.SelectedP18IDs.Where(p => p > 0).Count() == 0)
                {
                    this.AddMessage("Musíte zaškrtnout minimálně jednu operaci.");
                    return View(v);
                }
                var mq = new BO.myQuery("p18OperCode");
                mq.pids = v.SelectedP18IDs.Where(p=>p !=0).ToList();                
                var lis = new List<BO.p18OperCode>();
                if (mq.pids.Count() > 0 && oper=="save")
                {
                    lis = Factory.p18OperCodeBL.GetList(mq).ToList();
                }

                int x = Factory.p41TaskBL.AppendPos(v.RecP41, lis, v.p18flag);
                if (x > 0)
                {

                    v.SetJavascript_CallOnLoad(0, "p41");
                    return View(v);
                }                


            }
            

            return View(v);
        }
        private void RefreshState_p41AppendPo(ref p41AppendPoViewModel v)
        {
            v.RecP41 = Factory.p41TaskBL.Load(v.p41ID);
            var recP27 = Factory.p27MszUnitBL.Load(v.RecP41.p27ID);
            var mq = new BO.myQuery("p18OperCode");
            mq.p25id = recP27.p25ID;
            mq.p18flag = v.p18flag;
            mq.explicit_orderby = "a.p18Code";  //nutno setřídit podle kódu/pořadí operace
            v.lisP18 = Factory.p18OperCodeBL.GetList(mq);
        }

        public IActionResult p41Timeline(string d)
        {
            var v = new Models.p41TimelineViewModel();
            if (String.IsNullOrEmpty(d) == true)
            {
                v.CurrentDate = DateTime.Today;
            }
            else
            {
                v.CurrentDate = BO.BAS.String2Date(d);                
            }
            v.HourFrom = Factory.CBL.LoadUserParamInt("p41Timeline-hourfrom", 6) ;
            v.HourUntil = Factory.CBL.LoadUserParamInt("p41Timeline-houruntil", 20);
            v.localQuery = new p41TimelineQuery();            
            v.localQuery.SelectedP27IDs = Factory.CBL.LoadUserParam("p41Timeline-p27ids");
            v.localQuery.SelectedP27Names = Factory.CBL.LoadUserParam("p41Timeline-p27names");
            
            var mq = new BO.myQuery("p26Msz");
            mq.IsRecordValid = true;
            
            mq = new BO.myQuery("p27MszUnit");
            mq.IsRecordValid = true;
            mq.SetPids(v.localQuery.SelectedP27IDs);            
            v.lisP27 = Factory.p27MszUnitBL.GetList(mq).ToList();
            v.localQuery.SelectedP27Names = string.Join(",", v.lisP27.Select(p => p.p27Name));

            mq = new BO.myQuery("p41Task");
            mq.DateBetween = v.CurrentDate;
            mq.DateBetweenDays = 1;
            mq.explicit_orderby = "a.p41PlanStart"; //je důležité setřídit zakázky podle času-od, aby se v grid matici načítali postupně po řádcích!
            v.Tasks = Factory.p41TaskBL.GetList(mq);     //.OrderBy(p=>p.p41PlanStart); 

            v.lisFond = Factory.p31CapacityFondBL.GetCells(v.CurrentDate, v.CurrentDate);
            
            return View(v);
        }
        public IActionResult Index(int pid)
        {
            var v = new Models.p41PreviewViewModel();
            v.Rec = Factory.p41TaskBL.Load(pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                var tg = Factory.o51TagBL.GetTagging("p41", pid);
                v.Rec.TagHtml = tg.TagHtml;
                v.RecP52 = Factory.p52OrderItemBL.Load(v.Rec.p52ID);
                v.RecP51 = Factory.p51OrderBL.Load(v.RecP52.p51ID);
                return View(v);
            }
            

        }

        public IActionResult p44List(int pid)
        {
            var v = new Models.p41PreviewViewModel();
            v.Rec = Factory.p41TaskBL.Load(pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {                
                return View(v);
            }


        }

        public IActionResult Create(int p52id, int p51id)
        {
            var v = new p41CreateViewModel();
            v.Tasks = new List<BO.p41Task>();
            v.lisP27 = new List<BO.p27MszUnit>();
            if (p52id > 0) { v.p52ID = p52id; };
            if (p51id > 0) { v.p51ID = p51id; };
            if (v.p51ID > 0)
            {
                v.RecP51 = Factory.p51OrderBL.Load(v.p51ID);
                v.p51Code = v.RecP51.p51Code;
                var simul = new UI.TaskSimulation(Factory);
                simul.Date0 = getDate0(v);
                v.Tasks = simul.getTasksByP51(v.p51ID);
            }
            if (v.p52ID > 0)
            {
                v.RecP52 = Factory.p52OrderItemBL.Load(v.p52ID);
                v.p52Code = v.RecP52.p52Code;
                v.RecP51 = Factory.p51OrderBL.Load(v.RecP52.p51ID);
                v.p51Code = v.RecP51.p51Code;
                var simul = new UI.TaskSimulation(Factory);
                simul.Date0 = getDate0(v);
                v.Tasks = simul.getTasksByP52(v.p52ID);
            }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Models.p41CreateViewModel v, string rec_oper,int p27id)
        {            
            if (rec_oper == "p51id_change")
            {
                v.p52ID = 0;
                v.p52Code = "";
            }
            if (v.p51ID > 0)
            {
                v.RecP51 = Factory.p51OrderBL.Load(v.p51ID);
            }
            if (v.p52ID > 0)
            {
                v.RecP52 = Factory.p52OrderItemBL.Load(v.p52ID);
                v.RecP51 = Factory.p51OrderBL.Load(v.RecP52.p51ID);
            }
            if (v.p26ID > 0)
            {
                v.RecP26 = Factory.p26MszBL.Load(v.p26ID);
                var mq = new BO.myQuery("p27MszUnit");
                mq.p26id = v.p26ID;
                v.lisP27 = Factory.p27MszUnitBL.GetList(mq).ToList();
            }
            else
            {
                v.lisP27 = new List<BO.p27MszUnit>();
            }
            if (rec_oper == "simulation_p51")
            {
                if (v.p51ID == 0)
                {
                    Factory.CurrentUser.AddMessage("Musíte vybrat objednávku.");
                }
                else
                {
                    var simul = new UI.TaskSimulation(Factory);
                    simul.Date0 = getDate0(v);                    
                    v.Tasks = simul.getTasksByP51(v.p51ID);
                }
            }
            if (rec_oper== "simulation_p52")
            {
                if (v.p52ID == 0)
                {
                    Factory.CurrentUser.AddMessage("Musíte vybrat položku objednávky.");
                }
                else
                {
                    var simul = new UI.TaskSimulation(Factory);
                    simul.Date0 = getDate0(v);
                    v.Tasks = simul.getTasksByP52(v.p52ID);
                }                
            }
            if (rec_oper == "newitem")
            {
                if (v.Tasks == null) v.Tasks = new List<BO.p41Task>();
                var c = new BO.p41Task();
                if (v.Tasks.Where(p=>p.p41PlanEnd !=null).Count() > 0)
                {
                    c.p41PlanStart = v.Tasks.Where(p => p.p41PlanEnd != null).Last().p41PlanEnd;
                }
                else
                {
                    c.p41PlanStart = DateTime.Now.AddHours(1);
                }
                c.p41Duration = 60;

                if (p27id > 0)
                {
                    c.p27ID = p27id;
                    c.p27Name = Factory.p27MszUnitBL.Load(p27id).p27Name;
                }
                if (v.RecP52 != null)
                {
                    c.p52ID = v.RecP52.pid;
                    c.p52Code = v.RecP52.p52Code;
                    //c.p41Name = v.RecP52.p11Name + " [" + v.RecP52.p11Code + "]";
                }
                v.Tasks.Add(c);

                return View(v);
            }
            if (rec_oper == "clear")
            {
                v.Tasks = new List<BO.p41Task>();
            }
            if (rec_oper == "delete")
            {
                //došlo k virtuálnímu odstranění řádku zakázky - pouze postback
            }
            if (rec_oper == "postback")     //pouze postback
            {

            }
            if (v.Tasks == null)
            {
                v.Tasks = new List<BO.p41Task>();
            }

            if (ModelState.IsValid)
            {

                if (rec_oper == "save")
                {
                    int x = Factory.p41TaskBL.SaveBatch(v.Tasks.Where(p=>p.IsTempDeleted==false).ToList());
                    if (x > 0)
                    {

                        v.SetJavascript_CallOnLoad(0, "p41");
                        return View(v);
                    }
                }



            }

            
            return View(v);

        }

        public IActionResult Record(int pid, bool isclone)
        {
            if (Factory.CurrentUser.j03EnvironmentFlag == 1)
            {
                return this.StopPageClientPageOnly(true);
            }
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p41RecordViewModel();
            v.Rec = Factory.p41TaskBL.Load(pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            var tg = Factory.o51TagBL.GetTagging("p41", pid);
            v.TagPids = tg.TagPids;
            v.TagNames = tg.TagNames;
            v.TagHtml = tg.TagHtml;

            RefreshState_Record(v);


            if (isclone)
            {
                
                v.Toolbar.MakeClone();
                v.Rec.p41Code = Factory.CBL.EstimateRecordCode("p41");
            }



            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p41RecordViewModel v)
        {
           
            if (ModelState.IsValid)
            {
                BO.p41Task c = new BO.p41Task();
                if (v.Rec.pid > 0) c = Factory.p41TaskBL.Load(v.Rec.pid);

                c.p52ID = v.Rec.p52ID;
                c.b02ID = v.Rec.b02ID;
                c.p27ID = v.Rec.p27ID;
                c.j02ID_Owner = v.Rec.j02ID_Owner;
                c.p41IsDraft = v.Rec.p41IsDraft;
                c.p41Code = v.Rec.p41Code;
                c.p41Name = v.Rec.p41Name;
                c.p41Memo = v.Rec.p41Memo;
                c.p41StockCode = v.Rec.p41StockCode;

                c.p41PlanStart = v.Rec.p41PlanStart;
                //c.p41Duration = v.Rec.p41Duration;
                c.p41PlanUnitsCount = v.Rec.p41PlanUnitsCount;


                v.Rec.pid = Factory.p41TaskBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p41", v.Rec.pid, v.TagPids);
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }


            }

            RefreshState_Record(v);

            this.Notify_RecNotSaved();
            return View(v);

        }


        private void RefreshState_Record(p41RecordViewModel v)
        {
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.RecP52 = Factory.p52OrderItemBL.Load(v.Rec.p52ID);
            v.RecP51 = Factory.p51OrderBL.Load(v.RecP52.p51ID); 
            BO.p11ClientProduct cP11 = Factory.p11ClientProductBL.Load(v.RecP52.p11ID);
            if (cP11.p10ID_Master > 0)
            {
                v.p25ID = Factory.p10MasterProductBL.Load(cP11.p10ID_Master).p25ID; //z RecP10 se bere typ zařízení pro combo nabídku středisek
            }
            else
            {
                if (cP11.p12ID > 0)
                {
                    v.p25ID = Factory.p12ClientTpvBL.Load(cP11.p12ID).p25ID;    //vlastní klientská receptura
                }
            }
            



        }


        private DateTime getDate0(Models.p41CreateViewModel v)
        {
            if (v.Date0 == null)
            {
                return DateTime.Now;
            }
            else
            {
                return v.Date0.Value;
            }
        }

    }
}