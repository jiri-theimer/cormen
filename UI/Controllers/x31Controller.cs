using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML;
using Microsoft.AspNetCore.Mvc;

using System.IO;
using System.IO.Packaging;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using UI.Models;
using System.Data;

namespace UI.Controllers
{
    public class x31Controller : BaseController
    {
        private readonly BL.ThePeriodProvider _pp;
        public x31Controller(BL.ThePeriodProvider pp)
        {            
            _pp = pp;
        }

        public IActionResult ReportNoContext(int x31id)
        {
            var v = new ReportNoContextViewModel();
                      
            v.SelectedX31ID = x31id;
            RefreshStateReportNoContext(v);

          

            return View(v);
        }
        [HttpPost]
        public IActionResult ReportNoContext(ReportNoContextViewModel v, string oper)
        {
            
            RefreshStateReportNoContext(v);

            
            return View(v);
        }
        private void RefreshStateReportNoContext(ReportNoContextViewModel v)
        {
            if (v.SelectedX31ID > 0)
            {
                v.RecX31 = Factory.x31ReportBL.Load(v.SelectedX31ID);
                v.SelectedReport = v.RecX31.x31Name;

               
                if (System.IO.File.Exists(Factory.App.ReportFolder + "\\" + v.RecX31.x31FileName))
                {
                    var xmlReportSource = new Telerik.Reporting.XmlReportSource();
                    var strXmlContent = System.IO.File.ReadAllText(Factory.App.ReportFolder + "\\" +v.RecX31.x31FileName);
                    if (strXmlContent.Contains("datfrom") && strXmlContent.Contains("datuntil"))
                    {
                        v.IsPeriodFilter = true;
                        v.PeriodFilter = new PeriodViewModel();
                        v.PeriodFilter.IsShowButtonRefresh = true;
                        var per = InhalePeriodFilter();
                        v.PeriodFilter.PeriodValue = per.pid;
                        v.PeriodFilter.d1 = per.d1;
                        v.PeriodFilter.d2 = per.d2;
                    }
                    else
                    {
                        v.IsPeriodFilter = false;
                    }
                    if (strXmlContent.Contains("1=1"))
                    {
                        v.lisJ72 = Factory.j72TheGridTemplateBL.GetList(v.RecX31.x31Entity, Factory.CurrentUser.pid, null).Where(p => p.j72HashJ73Query == true);
                        foreach (var c in v.lisJ72.Where(p => p.j72IsSystem == true))
                        {
                            c.j72Name = "Výchozí GRID";
                        }
                    }
                }
                else
                {
                    this.AddMessage("Na serveru nelze dohledat soubor šablony zvolené tiskové sestavy.");
                }

            }

        }


        public IActionResult ReportContext(int pid, string prefix,int x31id)
        {
            var v = new ReportContextViewModel() { rec_pid = pid, rec_prefix = prefix };
            if (string.IsNullOrEmpty(v.rec_prefix)==true || v.rec_pid == 0)
            {
                return StopPage(true, "pid or prefix missing");
            }
            if (x31id == 0)
            {
                v.UserParamKey = "ReportContext-" + prefix + "-x31id";
                x31id = Factory.CBL.LoadUserParamInt(v.UserParamKey);

            }
            v.SelectedX31ID = x31id;
            
                        
            RefreshStateReportContext(v);
            return View(v);
        }
        [HttpPost]
        public IActionResult ReportContext(ReportContextViewModel v,string oper)
        {
            RefreshStateReportContext(v);

            if (oper == "change_x31id" && v.SelectedX31ID>0)
            {
                Factory.CBL.SetUserParam(v.UserParamKey, v.SelectedX31ID.ToString());
                v.GeneratedTempFileName = "";
            }
            
            
            return View(v);
        }

        private void RefreshStateReportContext(ReportContextViewModel v)
        {
            v.Prefix = v.rec_prefix;
            if (v.SelectedX31ID > 0)
            {
                v.RecX31 = Factory.x31ReportBL.Load(v.SelectedX31ID);
                v.SelectedReport = v.RecX31.x31Name;

                if (!System.IO.File.Exists(Factory.App.ReportFolder + "\\" + v.RecX31.x31FileName))
                {                  
                    this.AddMessage("Na serveru nelze dohledat soubor šablony zvolené tiskové sestavy.");
                }

               
            }            

        }

        public IActionResult Record(int pid, bool isclone)
        {
            var v = new Models.x31RecordViewModel() { UploadGuid = BO.BAS.GetGuid() };
            if (pid > 0)
            {
                v.Rec = Factory.x31ReportBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
               
            }
            else
            {
                v.Rec = new BO.x31Report();
                v.Rec.entity = "x31";

            }

            RefreshState(v);

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(x31RecordViewModel v,string oper)
        {
            RefreshState(v);
            if (oper == "postback")
            {
                return View(v);
            }
            if (ModelState.IsValid)
            {
                
                BO.x31Report c = new BO.x31Report();
                if (v.Rec.pid > 0) c = Factory.x31ReportBL.Load(v.Rec.pid);
                var lisO27 = BO.BASFILE.GetUploadedFiles(Factory.App.TempFolder, v.UploadGuid);
                if (lisO27.Count() > 0)
                {
                    c.x31FileName = lisO27.First().o27Name;
                }
                c.x31Entity = v.Rec.x31Entity;
                c.x31Name = v.Rec.x31Name;
                c.x31Code = v.Rec.x31Code;
                c.x31Description = v.Rec.x31Description;
                c.x31Is4SingleRecord = v.Rec.x31Is4SingleRecord;
                c.x31ReportFormat = v.Rec.x31ReportFormat;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);
                
                c.pid = Factory.x31ReportBL.Save(c);
                if (c.pid > 0)
                {                    
                    if (lisO27.Count() > 0)
                    {
                        System.IO.File.Copy(Factory.App.TempFolder + "\\" + lisO27.First().o27ArchiveFileName, Factory.App.ReportFolder + "\\" + lisO27.First().o27Name, true);                        
                    }
                    v.SetJavascript_CallOnLoad(c.pid);
                    return View(v);
                    


                }
            }
            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshState(x31RecordViewModel v)
        {
            v.Toolbar = new MyToolbarViewModel(v.Rec);

        }




        private BO.ThePeriod InhalePeriodFilter()
        {
            var ret = _pp.ByPid(0);
            int x = Factory.CBL.LoadUserParamInt("report-period-value");
            if (x > 0)
            {
                ret = _pp.ByPid(x);
            }
            else
            {
                ret.d1 = Factory.CBL.LoadUserParamDate("report-period-d1");
                ret.d2 = Factory.CBL.LoadUserParamDate("report-period-d2");

            }
            
            return ret;
        }

        
       

        

    }
}