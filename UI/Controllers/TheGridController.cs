﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Newtonsoft.Json;
using System.Text;



using UI.Models;
using System.ComponentModel;
using DocumentFormat.OpenXml.EMMA;

namespace UI.Controllers
{
    public class TheGridController : BaseController

        
    {
       
        private System.Text.StringBuilder _s;
        private UI.Models.TheGridViewModel _grid;
        private readonly BL.TheColumnsProvider _colsProvider;
        private readonly BL.ThePeriodProvider _pp;

        public TheGridController(BL.TheColumnsProvider cp,BL.ThePeriodProvider pp)
        {
            _colsProvider = cp;
            _pp = pp;
        }

        public IActionResult FlatView(string prefix,int go2pid)    //pouze grid bez subform
        {
            var v = inhaleGridViewInstance(prefix, go2pid);
            v.j72id = Factory.CBL.LoadUserParamInt("flatview-j72id-" + prefix);
           
            
            return View(v);
        }
        public IActionResult MasterView(string prefix,int go2pid)    //grid horní + spodní panel
        {
            TheGridInstanceViewModel v = inhaleGridViewInstance(prefix, go2pid);
            v.j72id= Factory.CBL.LoadUserParamInt("masterview-j72id-" + prefix);

            BO.TheEntity ce = BL.TheEntities.ByPrefix(prefix);
            var tabs = new List<NavTab>();
            
            switch (prefix)
            {
                case "p13":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/p13/Index?pid="+ AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Technologický rozpis operací", Entity = "p14MasterOper", Url = "SlaveView?prefix=p14" });
                    tabs.Add(new NavTab() { Name="Master produkty",Entity = "p10MasterProduct", Url = "SlaveView?prefix=p10" });
                    
                    tabs.Add(new NavTab() { Name = "Klientská receptura", Entity = "p12ClientTpv", Url = "SlaveView?prefix=p12" });
                    tabs.Add(new NavTab() { Name = "Klientské produkty", Entity = "p11ClientProduct", Url = "SlaveView?prefix=p11" });
                    tabs.Add(new NavTab() { Name = "Dokumenty", Entity = "o23Doc", Url = "SlaveView?prefix=o23" });
                    break;
                case "p19":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/p19/Index?pid=" + AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Master receptury", Entity = "p13MasterTpv", Url = "SlaveView?prefix=p13" });
                    tabs.Add(new NavTab() { Name = "Master produkty", Entity = "p10MasterProduct", Url = "SlaveView?prefix=p10" });
                    break;
                case "p28":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/p28/Index?pid="+ AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Lidé", Entity = "j02Person", Url = "SlaveView?prefix=j02" });
                    tabs.Add(new NavTab() { Name = "Objednávky", Entity = "p51Order", Url = "SlaveView?prefix=p51" });
                    tabs.Add(new NavTab() { Name = "Zakázky", Entity = "p41Task", Url = "SlaveView?prefix=p41" });
                    tabs.Add(new NavTab() { Name = "Skupiny zařízení", Entity = "p26Msz", Url = "SlaveView?prefix=p26" });
                    tabs.Add(new NavTab() { Name = "Licence", Entity = "p21License", Url = "SlaveView?prefix=p21" });
                    tabs.Add(new NavTab() { Name = "Klientská receptura", Entity = "p12ClientTpv", Url = "SlaveView?prefix=p12" });
                    tabs.Add(new NavTab() { Name = "Klientské produkty", Entity = "p11ClientProduct", Url = "SlaveView?prefix=p11" });
                    tabs.Add(new NavTab() { Name = "Dokumenty", Entity = "o23Doc", Url = "SlaveView?prefix=o23" });

                    break;
                case "p21":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/p21/Index?pid="+ AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Master produkty", Entity = "p10MasterProduct", Url = "SlaveView?prefix=p10" });
                    tabs.Add(new NavTab() { Name = "Master receptury", Entity = "p13MasterTpv", Url = "SlaveView?prefix=p13" });
                    tabs.Add(new NavTab() { Name = "Klientské receptury", Entity = "p12ClientTpv", Url = "SlaveView?prefix=p12" });
                    tabs.Add(new NavTab() { Name = "Klientské produkty", Entity = "p11ClientProduct", Url = "SlaveView?prefix=p11" });
                    tabs.Add(new NavTab() { Name = "Dokumenty", Entity = "o23Doc", Url = "SlaveView?prefix=o23" });

                    break;
                case "p10":                    
                    tabs.Add(new NavTab() { Name = "Info", Url = "/p10/Index?pid="+ AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Technologický rozpis operací", Entity = "p14MasterOper", Url = "SlaveView?prefix=p14" });
                    tabs.Add(new NavTab() { Name = "Licence", Entity = "p21License", Url = "SlaveView?prefix=p21" });
                    tabs.Add(new NavTab() { Name = "Klientské produkty", Entity = "p11ClientProduct", Url = "SlaveView?prefix=p11" });
                    tabs.Add(new NavTab() { Name = "Dokumenty", Entity = "o23Doc", Url = "SlaveView?prefix=o23" });

                    break;
                case "j02":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/j02/Index?pid="+ AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Dokumenty", Entity = "o23Doc", Url = "SlaveView?prefix=o23" });
                    tabs.Add(new NavTab() { Name = "Založené objednávky", Entity = "p51Order", Url = "SlaveView?prefix=p51" });
                    tabs.Add(new NavTab() { Name = "Založené zakázky", Entity = "p41Task", Url = "SlaveView?prefix=p41" });
                    tabs.Add(new NavTab() { Name = "Outbox", Entity = "x40MailQueue", Url = "SlaveView?prefix=x40" });
                    tabs.Add(new NavTab() { Name = "PING Log", Entity = "j92PingLog", Url = "SlaveView?prefix=j92" });
                    tabs.Add(new NavTab() { Name = "LOGIN Log", Entity = "j90LoginAccessLog", Url = "SlaveView?prefix=j90" });
                    break;
                case "p26":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/p26/Index?pid="+ AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Zařízení", Entity = "p27MszUnit", Url = "SlaveView?prefix=p27" });
                    tabs.Add(new NavTab() { Name = "Zakázky", Entity = "p41Task", Url = "SlaveView?prefix=p41" });
                    tabs.Add(new NavTab() { Name = "Dokumenty", Entity = "o23Doc", Url = "SlaveView?prefix=o23" });
                    break;
                case "o23":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/o23/Index?pid="+ AppendPid2Url(v.go2pid) });
                    break;
                //klientské prostředí
                case "p12":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/p12/Index?pid="+ AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Technologický rozpis operací", Entity = "p15ClientOper", Url = "SlaveView?prefix=p15" });
                    tabs.Add(new NavTab() { Name = "Produkty", Entity = "p11ClientProduct", Url = "SlaveView?prefix=p11" });
                    
                    tabs.Add(new NavTab() { Name = "Dokumenty", Entity = "o23Doc", Url = "SlaveView?prefix=o23" });
                    break;
                case "p11":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/p11/Index?pid="+ AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Technologický rozpis operací", Entity = "p15ClientOper", Url = "SlaveView?prefix=p15" });
                    tabs.Add(new NavTab() { Name = "Licence", Entity = "p21License", Url = "SlaveView?prefix=p21" });
                    tabs.Add(new NavTab() { Name = "Zakázky", Entity = "p41Task", Url = "SlaveView?prefix=p41" });
                    tabs.Add(new NavTab() { Name = "Položky objednávek", Entity = "p52OrderItem", Url = "SlaveView?prefix=p52" });
                    tabs.Add(new NavTab() { Name = "Objednávky", Entity = "p51Order", Url = "SlaveView?prefix=p51" });
                    
                    tabs.Add(new NavTab() { Name = "Dokumenty", Entity = "o23Doc", Url = "SlaveView?prefix=o23" });
                    break;
                case "p41":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/p41/Index?pid="+ AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Plán výrobních operací", Entity = "p44TaskOperPlan", Url = "SlaveView?prefix=p44" });

                    tabs.Add(new NavTab() { Name = "Technologický rozpis operací", Entity = "p15ClientOper", Url = "SlaveView?prefix=p15" });

                    tabs.Add(new NavTab() { Name = "Skutečná výroba", Entity = "p45TaskOperReal", Url = "SlaveView?prefix=p45" });

                    tabs.Add(new NavTab() { Name = "Dokumenty", Entity = "o23Doc", Url = "SlaveView?prefix=o23" });
                    

                    break;
                case "p51":
                    tabs.Add(new NavTab() { Name = "Info", Url = "/p51/Index?pid="+ AppendPid2Url(v.go2pid) });
                    tabs.Add(new NavTab() { Name = "Položky objednávky", Entity = "p52OrderItem", Url = "SlaveView?prefix=p52" });
                    tabs.Add(new NavTab() { Name = "Zakázky", Entity = "p41Task", Url = "SlaveView?prefix=p41" });
                    
                    tabs.Add(new NavTab() { Name = "Dokumenty", Entity = "o23Doc", Url = "SlaveView?prefix=o23" });

                    break;
            }
            string strDefTab = Factory.CBL.LoadUserParam("masterview-tab-" + prefix);
            var deftab = tabs[0];
            
            foreach (var tab in tabs)
            {
                tab.Url += "&master_entity=" + ce.TableName+"&master_pid="+ AppendPid2Url(v.go2pid);
               
                if (strDefTab !="" && tab.Entity== strDefTab)
                {
                    deftab = tab;  //uživatelem naposledy vybraná záložka
                }
            }
            deftab.CssClass += " active";
            if (go2pid > 0)
            {
                v.go2pid_url_in_iframe = deftab.Url;
                //v.go2pid_url_in_iframe = deftab.Url.Replace("@pid", go2pid.ToString());
            }

            v.NavTabs = tabs;
            return View(v);
        }

        private string AppendPid2Url(int go2pid)
        {
            if (go2pid > 0)
            {
                return go2pid.ToString();
            }
            else
            {
                return  "@pid";
            }
        }
        public IActionResult SlaveView(string master_entity,int master_pid, string prefix, int go2pid)    //podřízený subform v rámci MasterView
        {
            TheGridInstanceViewModel v = inhaleGridViewInstance(prefix, go2pid);
            v.j72id = Factory.CBL.LoadUserParamInt("slaveview-j72id-" + prefix+"-"+ master_entity);
            v.master_entity = master_entity;
            v.master_pid = master_pid;
            if (String.IsNullOrEmpty(v.master_entity) || v.master_pid == 0)
            {
                Factory.CurrentUser.AddMessage("Musíte vybrat záznam z nadřízeného panelu.");
            }
            
            
            return View(v);
        }
        private BO.ThePeriod InhaleGridPeriodDates()
        {
            var ret = _pp.ByPid(0);
            var x= Factory.CBL.LoadUserParamInt("grid-period-value");
            if (x > 0)
            {
                ret = _pp.ByPid(x);
            }
            else
            {
                ret.d1 = Factory.CBL.LoadUserParamDate("grid-period-d1");
                ret.d2 = Factory.CBL.LoadUserParamDate("grid-period-d2");

            }
            return ret;
        }
        private TheGridInstanceViewModel inhaleGridViewInstance(string prefix,int go2pid)
        {
            var v = new TheGridInstanceViewModel() { prefix = prefix, go2pid = go2pid,contextmenuflag=1 };
            v.entity = BL.TheEntities.ByPrefix(prefix).TableName;
            if (v.entity == "")
            {
                Factory.CurrentUser.AddMessage("Entity for Grid not found.");
            }
            if (BL.TheEntities.ByPrefix(prefix).IsGlobalPeriodQuery)
            {
                v.period = new PeriodViewModel();
                v.period.IsShowButtonRefresh = true;
                BO.ThePeriod per = InhaleGridPeriodDates();
                v.period.PeriodValue = per.pid;
                v.period.d1 = per.d1;
                v.period.d2 = per.d2;

            }
            
            
            return v;

        }
        public IActionResult Designer(int j72id)
        {
            var v = new Models.TheGridDesignerViewModel();
            v.Rec = Factory.gridBL.LoadTheGridState(j72id);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                if (v.Rec.j72IsSystem==false && v.Rec.j03ID == Factory.CurrentUser.pid)
                {
                    v.HasOwnerPermissions = true;
                    var mq = new BO.myQuery("j04UserRole");
                    mq.j72id = j72id;
                    var lis = Factory.j04UserRoleBL.GetList(mq);
                    v.j04IDs = string.Join(",", lis.Select(p => p.pid));
                    v.j04Names = string.Join(",", lis.Select(p => p.j04Name));
                }
                
                v.lisJ73 = Factory.gridBL.GetList_j73(v.Rec).ToList();
                foreach (var c in v.lisJ73)
                {
                    c.TempGuid = BO.BAS.GetGuid();
                }
                Designer_RefreshState(v);

                return View(v);
            }

        }
        private void Designer_RefreshState(Models.TheGridDesignerViewModel v)
        {            
            var mq = new BO.myQuery(v.Rec.j72Entity);
            var ce = BL.TheEntities.ByPrefix(mq.Prefix);
            v.Relations = BL.TheEntities.getApplicableRelations(mq.Prefix); //návazné relace
            v.Relations.Insert(0, new BO.EntityRelation() { TableName = ce.TableName, AliasSingular = ce.AliasSingular,SqlFrom=ce.SqlFromGrid,RelName="a" });   //primární tabulka a

            v.AllColumns = _colsProvider.AllColumns().ToList();            
            v.AllColumns.RemoveAll(p => p.VisibleWithinEntityOnly !=null && p.VisibleWithinEntityOnly.Contains(v.Rec.j72Entity.Substring(0, 3))==false);    //nepatřičné kategorie/štítky

            v.SelectedColumns = _colsProvider.ParseTheGridColumns(mq.Prefix,v.Rec.j72Columns);
            v.lisQueryFields = new BL.TheQueryFieldProvider(v.Rec.j72Entity.Substring(0, 3)).getPallete();
            v.lisPeriods = _pp.getPallete();
            
            if (v.lisJ73 == null)
            {
                v.lisJ73 = new List<BO.j73TheGridQuery>();
            }
            foreach (var c in v.lisJ73.Where(p => p.j73Column != null))
            {
                if (v.lisQueryFields.Where(p => p.Field == c.j73Column).Count() > 0)
                {
                    var cc = v.lisQueryFields.Where(p => p.Field == c.j73Column).First();
                    c.FieldType = cc.FieldType;
                    c.FieldEntity = cc.SourceEntity;
                    c.MasterPrefix = cc.MasterPrefix;
                    c.MasterPid = cc.MasterPid;
                }
            }
        }
        [HttpPost]
        public IActionResult Designer(Models.TheGridDesignerViewModel v,bool restore2factory, string oper, string guid,string j72name)    //uložení grid sloupců
        {
            Designer_RefreshState(v);

            if (oper == "postback")
            {
                return View(v);
            }
            if (oper=="saveas" && j72name != null)
            {
                var recJ72 = Factory.gridBL.LoadTheGridState(v.Rec.pid);
                var lisJ73 = Factory.gridBL.GetList_j73(recJ72).ToList();
                recJ72.j72IsSystem = false;recJ72.j72ID = 0;recJ72.pid = 0;recJ72.j72Name = j72name;recJ72.j03ID = Factory.CurrentUser.pid;
                List<int> j04ids = BO.BAS.ConvertString2ListInt(v.j04IDs);
                var intJ72ID = Factory.gridBL.SaveTheGridState(recJ72,lisJ73,j04ids);
                return RedirectToActionPermanent("Designer", new { j72id = intJ72ID });
            }
            if (oper == "rename" && j72name != null)
            {
                var recJ72 = Factory.gridBL.LoadTheGridState(v.Rec.pid);
                recJ72.j72Name = j72name;
                var intJ72ID = Factory.gridBL.SaveTheGridState(recJ72, null,null);
                return RedirectToActionPermanent("Designer", new { j72id = intJ72ID });
            }
            if (oper=="delete" && v.HasOwnerPermissions)
            {
                if (Factory.CBL.DeleteRecord("j72", v.Rec.pid) == "1")
                {
                    v.Rec.pid = Factory.gridBL.LoadTheGridState(v.Rec.j72Entity, Factory.CurrentUser.pid, v.Rec.j72MasterEntity).pid;
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }
            }
            if (oper == "changefield" && guid != null)
            {
                if (v.lisJ73.Where(p => p.TempGuid == guid).Count() > 0)
                {
                    var c = v.lisJ73.Where(p => p.TempGuid == guid).First();
                    c.j73Value = null; c.j73ValueAlias = null;
                    c.j73ComboValue = 0;
                    c.j73Date1 = null; c.j73Date2 = null;
                    c.j73Num1 = 0; c.j73Num2 = 0;
                }
                return View(v);
            }
            
            if (oper == "add_j73")
            {
                var c = new BO.j73TheGridQuery() { TempGuid = BO.BAS.GetGuid(), j73Column = v.lisQueryFields.First().Field };
                c.FieldType = v.lisQueryFields.Where(p => p.Field == c.j73Column).First().FieldType;
                c.FieldEntity = v.lisQueryFields.Where(p => p.Field == c.j73Column).First().SourceEntity;
                v.lisJ73.Add(c);

                return View(v);
            }
            if (oper == "delete_j73")
            {
                v.lisJ73.First(p => p.TempGuid == guid).IsTempDeleted = true;
                return View(v);
            }
            if (oper == "clear_j73")
            {
                v.lisJ73.Clear();
                return View(v);
            }
            if (restore2factory == true)
            {
                Factory.CBL.DeleteRecord("j72", v.Rec.pid);
                v.SetJavascript_CallOnLoad(v.Rec.pid);
                return View(v);
            }

            if (ModelState.IsValid)
            {
                
                var c = Factory.gridBL.LoadTheGridState(v.Rec.pid);
                c.j72Columns = v.Rec.j72Columns;
                c.j72Filter = "";   //automaticky vyčistit aktuální sloupcový filtr
                c.j72CurrentPagerIndex = 0;
                c.j72CurrentRecordPid = 0;
                c.j72IsPublic = v.Rec.j72IsPublic;
                if (c.j72SortDataField != null)
                {
                    if (c.j72Columns.IndexOf(c.j72SortDataField)== -1){ //vyčistit sort field, pokud se již nenachází ve vybraných sloupcích
                        c.j72SortDataField = "";
                        c.j72SortOrder = "";
                    }
                }
                List<int> j04ids = BO.BAS.ConvertString2ListInt(v.j04IDs);
                int intJ72ID = Factory.gridBL.SaveTheGridState(c, v.lisJ73.Where(p => p.j73ID > 0 || p.IsTempDeleted == false).ToList(),j04ids);
                if (intJ72ID > 0)
                {
                    if (c.j72MasterEntity == null)
                    {
                        Factory.CBL.SetUserParam("masterview-j72id-" + c.j72Entity.Substring(0, 3), intJ72ID.ToString());
                    }
                    
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }
                else
                {
                    return View(v);
                }

                //return RedirectToActionPermanent("Designer", new { j72id = v.Rec.pid });
            }

            
            return View(v);
           
        }




        public TheGridOutput HandleTheGridFilter(TheGridUIContext tgi, List<BO.TheGridColumnFilter> filter)
        {
            var cJ72 = this.Factory.gridBL.LoadTheGridState(tgi.j72id);
            cJ72.j72MasterPID = tgi.master_pid;
            cJ72.j72ContextMenuFlag = tgi.contextmenuflag;
            cJ72.OnDblClick = tgi.ondblclick;
            var lis = new List<string>();
            foreach (var c in filter)
            {                
                lis.Add(c.field + "###" + c.oper + "###" + c.value);
                
            }
            cJ72.j72CurrentPagerIndex = 0; //po změně filtrovací podmínky je nutné vyčistit paměť stránky
            cJ72.j72CurrentRecordPid = 0;
            
            cJ72.j72Filter = string.Join("$$$", lis);
            
            if (this.Factory.gridBL.SaveTheGridState(cJ72,null,null) > 0)
            {
                return render_thegrid_html(cJ72);
            }
            else
            {
                return render_thegrid_error("Nepodařilo se zpracovat filtrovací podmínku.");
            }
        }
        //public TheGridOutput HandleTheGridOper(int j72id,string oper,string key,string value, int master_pid,int contextmenuflag)
        public TheGridOutput HandleTheGridOper(TheGridUIContext tgi)
        {
            var cJ72 = this.Factory.gridBL.LoadTheGridState(tgi.j72id);
            cJ72.j72MasterPID = tgi.master_pid;
            cJ72.j72ContextMenuFlag = tgi.contextmenuflag;
            cJ72.OnDblClick = tgi.ondblclick;
            switch (tgi.key)
            {
                case "pagerindex":
                    cJ72.j72CurrentPagerIndex = BO.BAS.InInt(tgi.value);
                    break;
                case "pagesize":
                    cJ72.j72PageSize = BO.BAS.InInt(tgi.value);
                    break;
                case "sortfield":
                    if (cJ72.j72SortDataField != tgi.value)
                    {
                        cJ72.j72SortOrder = "asc";
                        cJ72.j72SortDataField = tgi.value;
                    }
                    else
                    {
                        if (cJ72.j72SortOrder == "desc")
                        {
                            cJ72.j72SortDataField = "";//vyčisitt třídění, třetí stav
                            cJ72.j72SortOrder = "";
                        }
                        else
                        {
                            if (cJ72.j72SortOrder == "asc")
                            {                                
                                cJ72.j72SortOrder = "desc";
                            }
                        }
                    }
                    
                    
                    break;
                case "filter":
                    break;
            }

            if (this.Factory.gridBL.SaveTheGridState(cJ72,null,null)> 0)
            {
                return render_thegrid_html(cJ72);
            }
            else
            {
                return render_thegrid_error("Nepodařilo se uložit GRIDSTATE");
            }

            
        }
        
        
        public TheGridOutput GetHtml4TheGrid(TheGridUIContext tgi) //Vrací HTML zdroj tabulky pro TheGrid v rámci j72TheGridState
        {
            
            var cJ72 = this.Factory.gridBL.LoadTheGridState(tgi.j72id);
            if (cJ72 == null)
            {
                return render_thegrid_error(string.Format("Nelze načíst grid state s id!", tgi.j72id.ToString()));
                
            }            
            cJ72.j72CurrentRecordPid = tgi.go2pid;
            cJ72.j72MasterPID = tgi.master_pid;
            cJ72.j72ContextMenuFlag = tgi.contextmenuflag;
            cJ72.OnDblClick = tgi.ondblclick;


            return render_thegrid_html(cJ72);
        }
        
        private System.Data.DataTable prepare_datatable(ref BO.myQuery mq, BO.j72TheGridState cJ72)
        {            
            
            mq.explicit_columns = _colsProvider.ParseTheGridColumns(mq.Prefix,cJ72.j72Columns);
            if (string.IsNullOrEmpty(cJ72.j72SortDataField)==false)
            {
                
                mq.explicit_orderby = _colsProvider.ByUniqueName(cJ72.j72SortDataField).getFinalSqlSyntax_ORDERBY() + " " + cJ72.j72SortOrder;
            }          
            if (String.IsNullOrEmpty(cJ72.j72Filter) == false)
            {
                mq.TheGridFilter = _colsProvider.ParseAdhocFilterFromString(cJ72.j72Filter, mq.explicit_columns);
            }
            mq.lisPeriods = _pp.getPallete();
            if (BL.TheEntities.ByTable(cJ72.j72Entity).IsGlobalPeriodQuery)
            {
                BO.ThePeriod per = InhaleGridPeriodDates();
                mq.global_d1 = per.d1;
                mq.global_d2 = per.d2;
            }
            if (cJ72.j72HashJ73Query)
            {
                mq.lisJ73 = Factory.gridBL.GetList_j73(cJ72);
            }
            mq.InhaleMasterEntityQuery(cJ72.j72MasterEntity, cJ72.j72MasterPID);

            return Factory.gridBL.GetList(mq);
        }
        //private void CompleteGridMyQuery(ref BO.myQuery mq, BO.j72TheGridState cJ72)
        //{
        //    switch (cJ72.j72MasterEntity)
        //    {
        //        case "p28Company":
        //            mq.p28id = cJ72.j72MasterPID;
        //            break;
        //        case "p10MasterProduct":
        //            mq.p10id = cJ72.j72MasterPID;
        //            break;
        //        case "p13MasterTpv":
        //            mq.p13id = cJ72.j72MasterPID;
        //            break;
        //        case "p21License":
        //            mq.p21id = cJ72.j72MasterPID;
        //            break;
        //        case "p26Msz":
        //            mq.p26id = cJ72.j72MasterPID;
        //            break;
        //        case "j02Person":
        //            mq.j02id = cJ72.j72MasterPID;
        //            break;
        //        case "p11ClientProduct":
        //            mq.p11id = cJ72.j72MasterPID;
        //            break;
        //        case "p12ClientTpv":
        //            mq.p12id = cJ72.j72MasterPID;
        //            break;
        //        case "p41Task":
        //            mq.p41id = cJ72.j72MasterPID;
        //            break;
        //        case "p51Order":
        //            mq.p51id = cJ72.j72MasterPID;
        //            break;
        //        default:
        //            break;
        //    }
        //}
        public TheGridOutput render_thegrid_html(BO.j72TheGridState cJ72)
        {
            var ret = new TheGridOutput();
            _grid = new TheGridViewModel() { Entity = cJ72.j72Entity };
            _grid.GridState = cJ72;

            ret.sortfield = cJ72.j72SortDataField;
            ret.sortdir = cJ72.j72SortOrder;
            
            var mq = new BO.myQuery(cJ72.j72Entity);
            if (BL.TheEntities.ByTable(cJ72.j72Entity).IsGlobalPeriodQuery)
            {
                BO.ThePeriod per = InhaleGridPeriodDates();
                mq.global_d1 = per.d1;
                mq.global_d2 = per.d2;
            }
                        
            _grid.Columns =_colsProvider.ParseTheGridColumns(mq.Prefix,cJ72.j72Columns);            
            
            mq.explicit_columns = _grid.Columns;
                        
            if (String.IsNullOrEmpty(cJ72.j72Filter) == false)
            {
                mq.TheGridFilter = _colsProvider.ParseAdhocFilterFromString(cJ72.j72Filter, mq.explicit_columns);
            }
            mq.lisPeriods = _pp.getPallete();
            if (cJ72.j72HashJ73Query)
            {
                mq.lisJ73 = Factory.gridBL.GetList_j73(cJ72);
            }
            mq.InhaleMasterEntityQuery(cJ72.j72MasterEntity, cJ72.j72MasterPID);
            
            
            var dtFooter = Factory.gridBL.GetList(mq, true);
            int intVirtualRowsCount = Convert.ToInt32(dtFooter.Rows[0]["RowsCount"]);

            if (intVirtualRowsCount > 500)
            {   //dotazy nad 500 záznamů budou mít zapnutý OFFSET režim stránkování
                mq.OFFSET_PageSize = cJ72.j72PageSize;
                mq.OFFSET_PageNum = cJ72.j72CurrentPagerIndex / cJ72.j72PageSize;
            }

            //třídění řešit až po spuštění FOOTER summary DOTAZu
            if (String.IsNullOrEmpty(cJ72.j72SortDataField) == false && _grid.Columns.Where(p => p.UniqueName == cJ72.j72SortDataField).Count() > 0)
            {
                var c = _grid.Columns.Where(p => p.UniqueName == cJ72.j72SortDataField).First();
                mq.explicit_orderby = c.getFinalSqlSyntax_ORDERBY() + " " + cJ72.j72SortOrder;
            }

            var dt = Factory.gridBL.GetList(mq);
            
            

            if (_grid.GridState.j72CurrentRecordPid > 0 && intVirtualRowsCount > cJ72.j72PageSize)
            {
                //aby se mohlo skočit na cílový záznam, je třeba najít stránku, na které se záznam nachází
                System.Data.DataRow[] recs = dt.Select("pid=" + _grid.GridState.j72CurrentRecordPid.ToString());
                if (recs.Count() > 0)
                {
                    var intIndex = dt.Rows.IndexOf(recs[0]);
                    _grid.GridState.j72CurrentPagerIndex = intIndex-(intIndex % _grid.GridState.j72PageSize);
                }
            }

            _s = new System.Text.StringBuilder();

            Render_DATAROWS(dt,mq);
            ret.body = _s.ToString();
            _s = new System.Text.StringBuilder();

            Render_TOTALS(dtFooter);
            ret.foot = _s.ToString();
            _s = new System.Text.StringBuilder();

            RENDER_PAGER(intVirtualRowsCount);
            ret.pager = _s.ToString();
            return ret;
        }

        private void Render_DATAROWS(System.Data.DataTable dt,BO.myQuery mq)
        {            
            int intRows = dt.Rows.Count;
            int intStartIndex = 0;
            int intEndIndex = 0;
            
            if (mq.OFFSET_PageSize > 0)
            {   //Zapnutý OFFSET - pouze jedna stránka díky OFFSET
                intStartIndex = 0;
                intEndIndex = intRows - 1;
            }
            else
            {   //bez OFFSET               
                intStartIndex = _grid.GridState.j72CurrentPagerIndex;
                intEndIndex = intStartIndex + _grid.GridState.j72PageSize - 1;
                if (intEndIndex + 1 > intRows) intEndIndex = intRows - 1;
            }

            for (int i = intStartIndex; i <= intEndIndex; i++)
            {
                System.Data.DataRow dbRow = dt.Rows[i];
                string strRowClass = "selectable";
                if (Convert.ToBoolean(dbRow["isclosed"])==true)
                {
                    strRowClass+= " trbin";
                }
                if (_grid.GridState.OnDblClick == null)
                {
                    _s.Append(string.Format("<tr id='r{0}' class='{1}'>", dbRow["pid"], strRowClass));
                }
                else
                {
                    _s.Append(string.Format("<tr id='r{0}' class='{1}' ondblclick='{2}(this)'>", dbRow["pid"], strRowClass, _grid.GridState.OnDblClick));
                }
                

                
                if (_grid.GridState.j72SelectableFlag>0)
                {
                    _s.Append(string.Format("<td class='td0' style='width:20px;'><input type='checkbox' id='chk{0}'/></td>", dbRow["pid"]));
                }
                else
                {
                    _s.Append("<td class='td0' style='width:20px;'></td>");
                }
                
                if (dbRow["bgcolor"] == System.DBNull.Value)
                {
                    _s.Append("<td class='td1' style='width:20px;'></td>");
                }
                else
                {
                    _s.Append(string.Format("<td class='td1' style='width:20px;background-color:{0}'></td>", dbRow["bgcolor"]));
                }
                
                if (_grid.GridState.j72ContextMenuFlag > 0)
                {
                    _s.Append(string.Format("<td class='td2' style='width:20px;'><a class='cm' onclick='tg_cm(event)'>&#9776;</a></td>"));      //hamburger menu
                }
                else
                {
                    _s.Append("<td class='td2' style='width:20px;'>");  //bez hamburger menu
                }
                


                foreach (var col in _grid.Columns)
                {
                    _s.Append("<td");
                    if (col.CssClass != null)
                    {
                        _s.Append(string.Format(" class='{0}'", col.CssClass));                        
                    }
                    
                    if (i==intStartIndex)   //první řádek musí mít explicitně šířky, aby to z něj zdědili další řádky
                    {
                        _s.Append(string.Format(" style='width:{0}'", col.ColumnWidthPixels));
                    }
                    _s.Append(string.Format(">{0}</td>", BO.BAS.ParseCellValueFromDb(dbRow, col)));
                    

                }
                _s.Append("</tr>");
            }
        }

        private void Render_TOTALS(System.Data.DataTable dt)
        {
            _s.Append("<tr id='tabgrid1_tr_totals'>");
            _s.Append(string.Format("<th class='th0' title='Celkový počet záznamů' colspan=3 style='width:60px;'><span class='badge badge-primary'>{0}</span></th>", string.Format("{0:#,0}", dt.Rows[0]["RowsCount"])));
            //_s.Append("<th style='width:20px;'></th>");
            //_s.Append("<th class='th0' style='width:20px;'></th>");
            string strVal = "";
            foreach (var col in _grid.Columns)
            {
                _s.Append("<th");
                if (col.CssClass != null)
                {
                    _s.Append(string.Format(" class='{0}'", col.CssClass));
                }

                strVal = "&nbsp;";
                if (dt.Rows[0][col.UniqueName] != System.DBNull.Value)
                {
                    strVal = BO.BAS.ParseCellValueFromDb(dt.Rows[0], col);
                }
                _s.Append(string.Format(" style='width:{0}'>{1}</th>",col.ColumnWidthPixels, strVal));


            }
            _s.Append("</tr>");
        }



        


        private void render_select_option(string strValue,string strText,string strSelValue)
        {
            if (strSelValue == strValue)
            {
                _s.Append(string.Format("<option selected value='{0}'>{1}</option>", strValue, strText));
            }
            else
            {
                _s.Append(string.Format("<option value='{0}'>{1}</option>", strValue, strText));
            }
            
        }

        private void RENDER_PAGER(int intRowsCount) //pager má maximálně 10 čísel, j72PageNum začíná od 0
        {
            int intPageSize = _grid.GridState.j72PageSize;

            _s.Append("<select title='Stránkování záznamů' onchange='tg_pagesize(this)'>");            
            render_select_option("50", "50", intPageSize.ToString());
            render_select_option("100", "100", intPageSize.ToString());
            render_select_option("200", "200", intPageSize.ToString());
            render_select_option("500", "500", intPageSize.ToString());
            render_select_option("1000", "1000", intPageSize.ToString());            
            _s.Append("</select>");
            if (intRowsCount < 0) return;
            
            if (intRowsCount <= intPageSize) return;

            _s.Append("<button title='První' class='btn btn-light tgp' style='margin-left:6px;' onclick='tg_pager(\n0\n)'>&lt;&lt;</button>");

            int intCurIndex = _grid.GridState.j72CurrentPagerIndex;
            int intPrevIndex = intCurIndex - intPageSize;
            if (intPrevIndex < 0) intPrevIndex = 0;
            _s.Append(string.Format("<button title='Předchozí' class='btn btn-light tgp' style='margin-right:10px;' onclick='tg_pager(\n{0}\n)'>&lt;</button>", intPrevIndex));

            if (intCurIndex >= intPageSize * 10)
            {
                intPrevIndex = intCurIndex - 10 * intPageSize;
                _s.Append(string.Format("<button class='btn btn-light tgp' onclick='tg_pager(\n{0}\n)'>...</button>", intPrevIndex));
            }
            

            int intStartIndex = 0;
            for (int i = 0; i <= intRowsCount; i += intPageSize*10)
            {
                if (intCurIndex>=i && intCurIndex<i+intPageSize*10)
                {
                    intStartIndex = i;
                    break;
                }
            }
                           
            int intEndIndex = intStartIndex+(9 * intPageSize);
            if (intEndIndex+1 > intRowsCount) intEndIndex = intRowsCount-1;

            
            int intPageNum = intStartIndex/intPageSize; string strClass;
            for (var i = intStartIndex; i <= intEndIndex; i+=intPageSize)
            {
                intPageNum += 1;
                if (intCurIndex>=i && intCurIndex < i+ intPageSize)
                {
                    strClass = "btn btn-secondary tgp";
                }
                else
                {
                    strClass = "btn btn-light tgp";
                }
                _s.Append(string.Format("<button class='{0}' onclick='tg_pager(\n{1}\n)'>{2}</button>",strClass, i,intPageNum));
               
            }
            if (intEndIndex+1 < intRowsCount)
            {
                intEndIndex += intPageSize;
                if (intEndIndex + 1 > intRowsCount) intEndIndex = intRowsCount - intPageSize;
                _s.Append(string.Format("<button class='btn btn-light tgp' onclick='tg_pager(\n{0}\n)'>...</button>", intEndIndex));
            }

            int intNextIndex = intCurIndex + intPageSize;
            if (intNextIndex + 1>intRowsCount) intNextIndex = intRowsCount-intPageSize;
            _s.Append(string.Format("<button title='Další' class='btn btn-light tgp' style='margin-left:10px;' onclick='tg_pager(\n{0}\n)'>&gt;</button>", intNextIndex));

            int intLastIndex = intRowsCount - (intRowsCount % intPageSize);  //% je zbytek po celočíselném dělení
            _s.Append(string.Format("<button title='Poslední' class='btn btn-light tgp' onclick='tg_pager(\n{0}\n)'>&gt;&gt;</button>", intLastIndex));


        }





        


        //public ActionResult GetJson4TheCombo(string entity, string text, bool addblankrow)
        //{
            
        //    var mq = new BO.myQuery(entity);
        //    mq.explicit_columns = new BL.TheColumnsProvider(mq).getDefaultPallete();
        //    mq.SearchString = text;//fulltext hledání
        //    var dt = Factory.gridBL.GetList(mq);

        //    if (addblankrow == true)
        //    {
        //        System.Data.DataRow newBlankRow = dt.NewRow();
        //        dt.Rows.InsertAt(newBlankRow, 0);
        //    }


        //    foreach (System.Data.DataRow row in dt.Rows)
        //    {
        //        foreach (System.Data.DataColumn col in dt.Columns)
        //        {
        //            if (col.DataType.Name == "String")
        //            {
        //                if (row[col.ColumnName] == DBNull.Value)
        //                {
        //                    row[col.ColumnName] = "";
        //                }
        //            }

        //        }
        //    }




        //    return new ContentResult() { Content = DataTableToJSONWithJSONNet(dt), ContentType = "application/json" };

        //}



        private TheGridOutput render_thegrid_error(string strError)
        {
            var ret = new TheGridOutput();
            ret.message = strError;
            if (this.Factory.CurrentUser.Messages4Notify.Count > 0)
            {
                ret.message += " | " + string.Join(",", this.Factory.CurrentUser.Messages4Notify.Select(p => p.Value));
            }
            return ret;
        }

        //private string DataTableToJSONWithJSONNet(System.Data.DataTable dt)
        //{

        //    return JsonConvert.SerializeObject(dt, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });




        //}

        public string getHTML_ContextMenu(int j72id)
        {
            var sb = new System.Text.StringBuilder();
            BO.j72TheGridState c = Factory.gridBL.LoadTheGridState(j72id);
                   
            sb.AppendLine("<div style='background-color:#ADD8E6;padding-left:10px;font-weight:bold;'>VYBRANÉ (zaškrtlé) záznamy</div>");
            sb.AppendLine("<div style='padding-left:10px;'>");
            sb.AppendLine(string.Format("<a href='javascript:tg_export(\"xlsx\",\"selected\")'>MS-EXCEL Export</a>", j72id));
            sb.AppendLine(string.Format("<a style='margin-left:20px;' href='javascript:tg_export(\"csv\",\"selected\")'>CSV Export</a>", j72id));
            sb.AppendLine("</div>");

            if (c.j72Entity.Substring(0, 3) == "p10")
            {
                sb.AppendLine("<hr class='hr-mini' />");
                sb.AppendLine("<a class='nav-link' href='javascript:p21_update();'>Aktualizovat licenci vybranými produkty</a>");
                
            }
            if ("j02,p51,p41,p10,p11,p12,p13,p18,p19,p26,p28,p21,o23".Contains(c.j72Entity.Substring(0, 3)))
            {
                sb.AppendLine("<hr class='hr-mini' />");
                sb.AppendLine("<a class='nav-link' href='javascript:tg_tagging();'>Hromadná kategorizace záznamů★</a>");

            }

            string strHeader = BL.TheEntities.ByTable(c.j72Entity).AliasPlural + ":";
        
            sb.AppendLine(string.Format("<div style='margin-top:20px;background-color:#ADD8E6;padding-left:10px;font-weight:bold;'>GRID <kbd>{0}</kbd></div>", strHeader));

            
            var lis = Factory.gridBL.GetList_j72(c.j72Entity, c.j03ID, c.j72MasterEntity);
            sb.AppendLine("<table style='width:100%;margin-bottom:20px;'>");
            foreach (var rec in lis)
            {
                sb.AppendLine("<tr>");
                if (rec.j72IsSystem)
                {
                    rec.j72Name = "Výchozí GRID přehled";
                }
                if (rec.pid == c.pid)
                {
                    rec.j72Name += " ✔";
                }
                sb.Append(string.Format("<td><a class='nav-link py-0' href='javascript:change_grid({0})'>{1}</a></td>", rec.pid,rec.j72Name));

                sb.AppendLine(string.Format("<td style='width:30px;'><a title='Grid Návrhář' class='nav-link py-0' href='javascript:_window_open(\"/TheGrid/Designer?j72id={0}\",2);'><img src='/Images/setting.png'/></a></td>", rec.pid));
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            //sb.AppendLine("<hr class='hr-mini' />");

            sb.AppendLine("<div style='padding-left:10px;'>");
            sb.AppendLine(string.Format("<a href='javascript:tg_export(\"xlsx\")'>MS-EXCEL Export (vše)</a>", j72id));
            sb.AppendLine(string.Format("<a style='margin-left:20px;' href='javascript:tg_export(\"csv\")'>CSV Export (vše)</a>", j72id));
            sb.AppendLine("</div>");

          

            sb.AppendLine("<hr class='hr-mini' />");
            sb.AppendLine("<a  href='javascript:tg_select(20)'>Vybrat prvních 20</a>⌾");
            sb.AppendLine("<a  href='javascript:tg_select(50)'>Vybrat prvních 50</a>⌾");
            sb.AppendLine("<a href='javascript:tg_select(100)'>Vybrat prvních 100</a>⌾");
            sb.AppendLine("<a href='javascript:tg_select(1000)'>Vybrat všechny záznamy na stránce</a>");
            
            return sb.ToString();
        }

        public FileResult GridExport(string format,int j72id,int master_pid,string master_entity,string pids)
        {
            BO.j72TheGridState cJ72 = this.Factory.gridBL.LoadTheGridState(j72id);
            cJ72.j72MasterEntity = master_entity;
            cJ72.j72MasterPID = master_pid;
            var mq = new BO.myQuery(cJ72.j72Entity);
            if (String.IsNullOrEmpty(pids) == false)
            {
                mq.SetPids(pids);
            }
            
           
            System.Data.DataTable dt = prepare_datatable(ref mq,cJ72);
            string filepath = Factory.App.TempFolder+"\\"+BO.BAS.GetGuid()+"."+ format;

            var cExport = new UI.dataExport();
            string strFileClientName = "gridexport_" + mq.Prefix + "." + format;

            if (format == "csv")
            {
                if (cExport.ToCSV(dt, filepath, mq))
                {                    
                    return File(System.IO.File.ReadAllBytes(filepath), "application/CSV", strFileClientName);
                }
            }
            if (format == "xlsx")
            {
                if (cExport.ToXLSX(dt, filepath, mq))
                {                    
                    return File(System.IO.File.ReadAllBytes(filepath), "application/vnd.ms-excel", strFileClientName);
                }
            }


            return null;

        }

        




    }
}