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
        public BO.Result RecoveryPlanInTask(int p41id)
        {
            Factory.p41TaskBL.RecoveryPlanInTask(p41id);            
            return new BO.Result(false);
        }
        public IActionResult p41CreateChild(int p41id, int p18flag)
        {
            var v = new Models.p41CreateChildViewModel();
            v.MasterID = p41id;
            v.p18flag = p18flag;
            v.lisDestOper = new List<BO.AppendPostPreP44Oper>();
            RefreshState_p41CreateChild(ref v);
            if (p18flag == 2 && Factory.p41TaskBL.LoadSuccessor(p41id) != null)
            {
                return this.StopPage(true, "Pro tuto zakázku již existuje PRE zakázka (předchůdce).");
            }
            if (p18flag == 3 && v.RecMasterP41.p41SuccessorID>0)
            {
                return this.StopPage(true, "Pro tuto zakázku již existuje POST zakázka (následovník).");
            }

            v.p41Name = v.RecMasterP41.p41Name;
            

            return View(v);

        }
        [HttpPost]
        public IActionResult p41CreateChild(Models.p41CreateChildViewModel v, string oper)
        {
            RefreshState_p41CreateChild(ref v);
            
            if (ModelState.IsValid)
            {
                if (oper == "postback")
                {
                    return View(v);
                }
                if (oper == "save" && (v.lisDestOper==null || v.lisDestOper.Where(p=>p.IsSelected==true).Count() == 0))
                {
                    this.AddMessage("Musíte zaškrtnout minimálně jednu operaci.");
                    return View(v);
                }
                
                var c = new BO.p41Task() { p41Name = v.p41Name, p27ID = v.SelectedP27ID };
                int x = Factory.p41TaskBL.CreateChild(c, v.RecMasterP41, v.lisDestOper.Where(p=>p.IsSelected==true).ToList(), v.p18flag);
                if (x > 0)
                {

                    v.SetJavascript_CallOnLoad(0, "p41");
                    return View(v);
                }


            }


            return View(v);
        }
        private void RefreshState_p41CreateChild(ref p41CreateChildViewModel v)
        {
            v.RecMasterP41 = Factory.p41TaskBL.Load(v.MasterID);
            if (v.SelectedP27ID > 0 && (v.lisDestOper==null || v.lisDestOper.Count()==0))
            {                
                var mq = new BO.myQuery("p18OperCode");
                mq.p27id = v.SelectedP27ID;
                mq.p18flag = v.p18flag;
                mq.explicit_orderby = "a.p18Code";  //nutno setřídit podle kódu/pořadí operace
                var lisP18 = Factory.p18OperCodeBL.GetList(mq);
                v.lisDestOper = new List<BO.AppendPostPreP44Oper>();
                foreach (var recP18 in lisP18)
                {
                    var c = new BO.AppendPostPreP44Oper() { p18ID = recP18.pid, p18Flag = recP18.p18Flag, p18Code = recP18.p18Code, OperCodePlusName = recP18.CodePlusName, p44MaterialUnitsCount = v.RecMasterP41.p41PlanUnitsCount, p18IsManualAmount = recP18.p18IsManualAmount };
                    v.lisDestOper.Add(c);
                }
            }


            
            //if (v.RecMasterP41.p52ID > 0)
            //{
            //    var recP52 = Factory.p52OrderItemBL.Load(v.RecMasterP41.p52ID);                
            //    BO.p11ClientProduct cP11 = Factory.p11ClientProductBL.Load(recP52.p11ID);
            //    if (cP11.p10ID_Master > 0)
            //    {
            //        v.p25ID = Factory.p10MasterProductBL.Load(cP11.p10ID_Master).p25ID; //z RecP10 se bere typ zařízení pro combo nabídku zařízení
            //    }
            //    else
            //    {
            //        if (cP11.p12ID > 0)
            //        {
            //            v.p25ID = Factory.p12ClientTpvBL.Load(cP11.p12ID).p25ID;    //vlastní klientská receptura
            //        }
            //    }
            //}



        }

        public IActionResult p41AppendPo(int p41id,int p18flag)
        {
            var v = new Models.p41AppendPoViewModel();
            v.p41ID = p41id;
            v.p18flag = p18flag;
            
            RefreshState_p41AppendPo(ref v);

            var mq = new BO.myQuery("p44TaskOperPlan");
            mq.p41id = p41id;
            var lisP44 = Factory.p44TaskOperPlanBL.GetList(mq);
            foreach(var c in lisP44)
            {
                if (v.lisDestOper.Where(p => p.p18ID == c.p18ID).Count() > 0)
                {
                    v.lisDestOper.Where(p => p.p18ID == c.p18ID).First().IsSelected = true;
                    v.lisDestOper.Where(p => p.p18ID == c.p18ID).First().p44MaterialUnitsCount = c.p44MaterialUnitsCount;
                }
                
            }

            return View(v);
           
        }
        [HttpPost]        
        public IActionResult p41AppendPo(Models.p41AppendPoViewModel v,string oper)
        {
            RefreshState_p41AppendPo(ref v);
            if (oper == "clear")
            {
                v.lisDestOper = new List<BO.AppendPostPreP44Oper>();
            }
            if (ModelState.IsValid)
            {
                if (oper=="save" && v.lisDestOper.Where(p => p.IsSelected==true).Count() == 0)
                {
                    this.AddMessage("Musíte zaškrtnout minimálně jednu operaci.");
                    return View(v);
                }
                //var mq = new BO.myQuery("p18OperCode");
                //mq.pids = v.SelectedP18IDs.Where(p=>p !=0).ToList();                
                //var lis = new List<BO.p18OperCode>();
                //if (mq.pids.Count() > 0 && oper=="save")
                //{
                //    lis = Factory.p18OperCodeBL.GetList(mq).ToList();
                //}

                int x = Factory.p41TaskBL.AppendPos(v.RecP41, v.lisDestOper.Where(p=>p.IsSelected==true).ToList(), v.p18flag,true);
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
            

            if (v.lisDestOper == null || v.lisDestOper.Count()==0)
            {
                var mq = new BO.myQuery("p18OperCode");
                mq.p41id = v.p41ID;
                mq.p18flag = v.p18flag;
                mq.explicit_orderby = "a.p18Code";  //nutno setřídit podle kódu/pořadí operace
                var lisP18 = Factory.p18OperCodeBL.GetList(mq);
                v.lisDestOper = new List<BO.AppendPostPreP44Oper>();
                foreach (var recP18 in lisP18)
                {
                    var c = new BO.AppendPostPreP44Oper() { p18ID = recP18.pid, p18Flag=recP18.p18Flag, p18Code=recP18.p18Code, OperCodePlusName = recP18.CodePlusName, p44MaterialUnitsCount = v.RecP41.p41PlanUnitsCount, p18IsManualAmount=recP18.p18IsManualAmount };
                    v.lisDestOper.Add(c);
                }
            }
            

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
            v.localQuery.SelectedP26IDs = Factory.CBL.LoadUserParam("p41Timeline-p26ids");
            //v.localQuery.SelectedP26Names = Factory.CBL.LoadUserParam("p41Timeline-p26names");
            v.localQuery.SelectedP27IDs = Factory.CBL.LoadUserParam("p41Timeline-p27ids");
            //v.localQuery.SelectedP27Names = Factory.CBL.LoadUserParam("p41Timeline-p27names");
            v.localQuery.SelectedB02IDs = Factory.CBL.LoadUserParam("p41Timeline-b02ids");
            v.localQuery.SelectedB02Names = Factory.CBL.LoadUserParam("p41Timeline-b02names");
            v.localQuery.IsPoPre = Factory.CBL.LoadUserParamBool("p41Timeline-ispopre",true);
            v.localQuery.IsTo = Factory.CBL.LoadUserParamBool("p41Timeline-isto", true);
            v.localQuery.IsPoPost = Factory.CBL.LoadUserParamBool("p41Timeline-ispopost", true);

            var mq = new BO.myQuery("p26Msz");
            mq.IsRecordValid = true;
            if (v.localQuery.SelectedP26IDs != "")
            {
                mq.SetPids(v.localQuery.SelectedP26IDs);
            }
            v.lisP26 = Factory.p26MszBL.GetList(mq).ToList();
            if (string.IsNullOrEmpty(v.localQuery.SelectedP26IDs)==false)
            {
                v.localQuery.SelectedP26Names = string.Join(",", v.lisP26.Select(p => p.p26Name));
            }
                
            mq = new BO.myQuery("p27MszUnit");
            mq.IsRecordValid = true;
            if (string.IsNullOrEmpty(v.localQuery.SelectedP26IDs)==false)
            {
                mq.p26ids = BO.BAS.ConvertString2ListInt(v.localQuery.SelectedP26IDs);
            }
            if (string.IsNullOrEmpty(v.localQuery.SelectedP27IDs)==false)
            {
                mq.SetPids(v.localQuery.SelectedP27IDs);
            }                        
            v.lisP27 = Factory.p27MszUnitBL.GetList(mq).ToList();
            if (string.IsNullOrEmpty(v.localQuery.SelectedP27IDs)==false)
            {
                v.localQuery.SelectedP27Names = string.Join(",", v.lisP27.Select(p => p.p27Name));
            }
                

            

            mq = new BO.myQuery("p41Task");
            mq.DateBetween = v.CurrentDate;
            mq.DateBetweenDays = 1;
            mq.p18flags = new List<int>() {999 };
            if (v.localQuery.IsPoPre) mq.p18flags.Add(2);
            if (v.localQuery.IsTo) mq.p18flags.Add(1);
            if (v.localQuery.IsPoPost) mq.p18flags.Add(3);
            mq.b02ids = BO.BAS.ConvertString2ListInt(v.localQuery.SelectedB02IDs);
            mq.p26ids = BO.BAS.ConvertString2ListInt(v.localQuery.SelectedP26IDs);
            mq.p27ids = BO.BAS.ConvertString2ListInt(v.localQuery.SelectedP27IDs);

            mq.explicit_orderby = "a.p41PlanStart"; //je důležité setřídit zakázky podle času-od, aby se v grid matici načítali postupně po řádcích!
            v.Tasks = Factory.p41TaskBL.GetList(mq).Where(p=>(p.p41PlanStart >= v.CurrentDT1 && p.p41PlanStart <= v.CurrentDT2) || (p.p41PlanEnd >= v.CurrentDT1 && p.p41PlanEnd <= v.CurrentDT2));     //.OrderBy(p=>p.p41PlanStart); 
            v.Slots = new List<Slot>();
            Slot lastSlot = null;
            foreach(BO.p41Task task in v.Tasks)
            {
                //var slot = new Slot() { p41ID = task.pid,p27ID=task.p27ID,Start=task.p41PlanStart.AddMinutes(task.p41DurationPoPre),End=task.p41PlanEnd.AddMinutes(-1*task.p41DurationPoPost),CssName="onetask" };
                var slot = new Slot() { p41ID = task.pid, p27ID = task.p27ID, Start = task.p41PlanStart, End = task.p41PlanEnd, CssName = "onetask",p41Code=task.p41Code,b02Color=task.b02Color,b02Name=task.b02Name };
                slot.Title = task.p11Code + ": " + BO.BAS.ObjectDateTime2String(slot.Start, "HH:mm") + " - " + BO.BAS.ObjectDateTime2String(slot.End, "HH:mm") + ": " + BO.BAS.OM2(task.p41Name, 30);
                slot.DurationHours = task.DurationHours;
                if (task.p41Duration>task.p41DurationPoPre+task.p41DurationPoPost)
                {
                    slot.TitleClear = BO.BAS.ObjectDateTime2String(task.p41PlanStart.AddMinutes(task.p41DurationPoPre), "HH:mm") + " - " + BO.BAS.ObjectDateTime2String(task.p41PlanEnd.AddMinutes(-1 * task.p41DurationPoPost), "HH:mm");
                }
                
                if (slot.Start < v.CurrentDT1)
                {
                    slot.Start = v.CurrentDT1;
                    slot.CssName = "onetask_overday";
                }
                if (slot.End > v.CurrentDT2)
                {
                    slot.End = v.CurrentDT2;
                    slot.CssName = "onetask_overday";
                }
                slot.ColStart = (slot.Start.Hour * 60 + slot.Start.Minute) / 10;

                if (task.p41Code.Contains("PRE"))
                {
                    slot.CssName = "pretask";
                    if (lastSlot !=null && (slot.ColStart + slot.ColSpan > lastSlot.ColStart))
                    {
                        slot.ColSpanKorekce = -1;
                    }
                }
                if (task.p41Code.Contains("POST"))
                {
                    slot.CssName = "posttask";
                    if (lastSlot !=null && (slot.ColStart < lastSlot.ColStart + lastSlot.ColSpan))
                    {
                        slot.ColStart += 1;
                    }
                }

                if (task.p41DurationPoPre > 0 && task.p41PlanStart >= v.CurrentDT1)
                {
                    slot.TitlePre = "PO-PRE: " + BO.BAS.ObjectDateTime2String(task.p41PlanStart, "HH:mm") + " - " + BO.BAS.ObjectDateTime2String(task.p41PlanStart.AddMinutes(task.p41DurationPoPre), "HH:mm");
                    if (task.p41Duration == task.p41DurationPoPre)
                    {
                        slot.ColEndPre = slot.ColSpan;
                    }
                    else
                    {
                        
                        slot.ColEndPre = Convert.ToInt32(task.p41DurationPoPre) / 10;
                    }
                }
                if (task.p41DurationPoPost > 0 && task.p41PlanEnd.AddMinutes(-1 * task.p41DurationPoPost) < v.CurrentDT2)
                {
                    slot.TitlePost = "PO-POST: " + BO.BAS.ObjectDateTime2String(task.p41PlanEnd.AddMinutes(-1 * task.p41DurationPoPost), "HH:mm") + " - " + BO.BAS.ObjectDateTime2String(task.p41PlanEnd, "HH:mm");
                    if (task.p41Duration == task.p41DurationPoPost)
                    {
                        slot.ColStartPost = 1;
                    }
                    else
                    {
                        
                        slot.ColStartPost = Convert.ToInt32(task.p41Duration - task.p41DurationPoPost) / 10;
                    }
                    
                }

                lastSlot = slot;

                v.Slots.Add(slot);
                
            }



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
                if (v.Rec.p52ID > 0)
                {
                    v.RecP52 = Factory.p52OrderItemBL.Load(v.Rec.p52ID);
                    v.RecP51 = Factory.p51OrderBL.Load(v.RecP52.p51ID);
                }
                
                if (v.Rec.p41SuccessorID > 0)
                {
                    v.RecSuccessor = Factory.p41TaskBL.Load(v.Rec.p41SuccessorID);
                }
                v.RecPredecessor = Factory.p41TaskBL.LoadSuccessor(pid);
                
                
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
        public IActionResult p45List(int pid)
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

        public IActionResult Create(int p52id, int p51id,string p52ids)
        {
            var v = new p41CreateViewModel();
            v.Tasks = new List<BO.p41Task>();
            v.lisP27 = new List<BO.p27MszUnit>();
            if (p52id > 0) { v.p52ID = p52id; };
            if (p51id > 0) { v.p51ID = p51id; };
            if (p52ids != null)
            {
                v.SelectedP52IDs = p52ids;
                foreach(int intP52ID in BO.BAS.ConvertString2ListInt(p52ids))
                {

                    v.SelectedP52Codes += ", " + Factory.p52OrderItemBL.Load(intP52ID).p52Code;
                }
            }
            if (v.p51ID > 0)
            {
                v.RecP51 = Factory.p51OrderBL.Load(v.p51ID);
                v.p51Code = v.RecP51.p51Code;
                //var simul = new UI.TaskSimulation(Factory);
                //simul.Date0 = getDate0(v);
                //v.Tasks = simul.getTasksByP51(v.p51ID);
            }
            if (v.p52ID > 0)
            {
                v.RecP52 = Factory.p52OrderItemBL.Load(v.p52ID);
                v.p52Code = v.RecP52.p52Code;
                v.RecP51 = Factory.p51OrderBL.Load(v.RecP52.p51ID);
                v.p51Code = v.RecP51.p51Code;
                if (v.RecP52.p52DateNeeded != null)
                {
                    v.Date0 = v.RecP52.p52DateNeeded;
                    
                }
                //var simul = new UI.TaskSimulation(Factory);
                //simul.Date0 = getDate0(v);
                //v.Tasks = simul.getTasksByP52(v.p52ID,v.p27ID);
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
                    v.Tasks = simul.getTasksByP52(v.p52ID,v.p27ID);
                }                
            }
            if (rec_oper == "simulation_p52ids")
            {
                var simul = new UI.TaskSimulation(Factory);
                simul.Date0 = getDate0(v);
                v.Tasks = simul.getTasksByP52IDs(BO.BAS.ConvertString2ListInt(v.SelectedP52IDs));
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

            if (v.RecP52 != null)
            {
                BO.p11ClientProduct cP11 = Factory.p11ClientProductBL.Load(v.RecP52.p11ID);
                if (cP11.p10ID_Master > 0)
                {
                    v.p25ID = Factory.p10MasterProductBL.Load(cP11.p10ID_Master).p25ID; //z RecP10 se bere typ zařízení pro combo nabídku zařízení
                }
                else
                {
                    if (cP11.p12ID > 0)
                    {
                        v.p25ID = Factory.p12ClientTpvBL.Load(cP11.p12ID).p25ID;    //vlastní klientská receptura
                    }
                }
                var mq = new BO.myQuery("p27MszUnit");
                mq.p25id = v.p25ID;
                v.lisP27 = Factory.p27MszUnitBL.GetList(mq).ToList();
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
            if (v.Rec.p41MasterID>0 && v.Rec.p52ID == 0)
            {
                var c = Factory.p41TaskBL.Load(v.Rec.p41MasterID);
                v.Rec.p52ID = c.p52ID;  //zjistit ID objednávky master zakázky
            }
            if (v.Rec.p52ID > 0)
            {
                v.RecP52 = Factory.p52OrderItemBL.Load(v.Rec.p52ID);
                v.RecP51 = Factory.p51OrderBL.Load(v.RecP52.p51ID);
                BO.p11ClientProduct cP11 = Factory.p11ClientProductBL.Load(v.RecP52.p11ID);
                if (cP11.p10ID_Master > 0)
                {
                    v.p25ID = Factory.p10MasterProductBL.Load(cP11.p10ID_Master).p25ID; //z RecP10 se bere typ zařízení pro combo nabídku zařízení
                }
                else
                {
                    if (cP11.p12ID > 0)
                    {
                        v.p25ID = Factory.p12ClientTpvBL.Load(cP11.p12ID).p25ID;    //vlastní klientská receptura
                    }
                }
            }
            
            



        }


        private DateTime getDate0(Models.p41CreateViewModel v)
        {
            if (v.Date0 == null)
            {
                //return DateTime.Now;
                return new DateTime(2000, 1, 1);    //rok 2000 znamená výchozí datum, které se má v simulaci změnit dle kapacitního fondu stroje
               
            }
            else
            {
                return v.Date0.Value;
            }
        }

    }
}