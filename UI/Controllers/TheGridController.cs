﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Newtonsoft.Json;
using System.Text;



using UI.Models;

namespace UI.Controllers
{
    public class TheGridController : BaseController
    {
        private System.Text.StringBuilder _s;
        private UI.Models.TheGridViewModel _grid;

        public IActionResult FlatView(string prefix,int go2pid)    //pouze grid bez subform
        {
            return View(inhaleGridViewInstance(prefix, go2pid));
        }
        public IActionResult MasterView(string prefix,int go2pid)    //grid horní + spodní panel
        {
            TheGridInstanceViewModel v = inhaleGridViewInstance(prefix, go2pid);
            var tabs = new List<NavTab>();
            
            switch (prefix)
            {
                case "p13":
                    tabs.Add(new NavTab() { Name = "Detail", Url = "/p13/Index?pid=@pid" });
                    tabs.Add(new NavTab() { Name="Master produkty",Entity = "p10MasterProduct", Url = "SlaveView?prefix=p10" });
                    tabs.Add(new NavTab() { Name = "Technologický rozpis operací", Entity = "p14MasterOper", Url = "SlaveView?prefix=p14" });
                    break;
                case "p28":
                    tabs.Add(new NavTab() { Name = "Detail", Url = "/p28/Index?pid=@pid" });
                    tabs.Add(new NavTab() { Name = "Lidé", Entity = "j02Person", Url = "SlaveView?prefix=j02" });
                    tabs.Add(new NavTab() { Name = "Stroje", Entity = "p26Msz", Url = "SlaveView?prefix=p26" });
                    break;
                case "p21":
                    tabs.Add(new NavTab() { Name = "Detail", Url = "/p21/Index?pid=@pid" });
                    tabs.Add(new NavTab() { Name = "Master produkty", Entity = "p10MasterProduct", Url = "SlaveView?prefix=p10" });
                    break;
                case "p10":                    
                    tabs.Add(new NavTab() { Name = "Detail", Url = "/p10/Index?pid=@pid" });
                    tabs.Add(new NavTab() { Name = "Licence", Entity = "p21License", Url = "SlaveView?prefix=p21" });
                    break;
                case "j02":
                    tabs.Add(new NavTab() { Name = "Detail", Url = "/j02/Index?pid=@pid" });
                    break;
                case "p26":
                    tabs.Add(new NavTab() { Name = "Detail", Url = "/p26/Index?pid=@pid" });
                    break;
                case "o23":
                    tabs.Add(new NavTab() { Name = "Detail", Url = "/o23/Index?pid=@pid" });
                    break;

            }
            foreach(var tab in tabs)
            {
                tab.Url += "&master_entity=" + BO.BAS.getEntityFromPrefix(prefix) + "&master_pid=@pid";
            }
            tabs[0].CssClass += " active";
            v.NavTabs = tabs;
            return View(v);
        }
        public IActionResult SlaveView(string master_entity,int master_pid, string prefix, int go2pid)    //podřízený subform v rámci MasterView
        {
            TheGridInstanceViewModel v = inhaleGridViewInstance(prefix, go2pid);
            v.master_entity = master_entity;
            v.master_pid = master_pid;
            if (String.IsNullOrEmpty(v.master_entity) || v.master_pid == 0)
            {
                Factory.CurrentUser.AddMessage("master_entity or master_pid missing.");
            }
            
            return View(v);
        }
        private TheGridInstanceViewModel inhaleGridViewInstance(string prefix,int go2pid)
        {
            var v = new TheGridInstanceViewModel() { prefix = prefix, go2pid = go2pid,contextmenuflag=1 };
            v.entity = BO.BAS.getEntityFromPrefix(prefix);
            if (v.entity == "")
            {
                Factory.CurrentUser.AddMessage("Entity for Grid not found.");
            }
            if (prefix == "p14") v.contextmenuflag = 0;  //nezobrazovat kontextové menu
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
                Designer_RefreshState(v);

                return View(v);
            }

        }
        private void Designer_RefreshState(Models.TheGridDesignerViewModel v)
        {
            var mq = new BO.myQuery(v.Rec.j72Entity);
            var cProvider = new BL.TheColumnsProvider(mq);
            v.ApplicableCollumns = cProvider.ApplicableColumns();
            v.SelectedColumns = cProvider.getSelectedPallete(v.Rec.j72Columns);
        }
        [HttpPost]
        public IActionResult Designer(Models.TheGridDesignerViewModel v)    //uložení grid sloupců
        {
            
            if (ModelState.IsValid)
            {
                

                var c = Factory.gridBL.LoadTheGridState(v.Rec.pid);
                c.j72Columns = v.Rec.j72Columns;
                c.j72Filter = "";   //automaticky vyčistit aktuální sloupcový filtr
                c.j72CurrentPagerIndex = 0;
                c.j72CurrentRecordPid = 0;
                if (c.j72SortDataField != null)
                {
                    if (c.j72Columns.IndexOf(c.j72SortDataField)== -1){ //vyčistit sort field, pokud se již nenachází ve vybraných sloupcích
                        c.j72SortDataField = "";
                        c.j72SortOrder = "";
                    }
                }
                if (Factory.gridBL.SaveTheGridState(c) > 0)
                {
                    //Factory.CurrentUser.AddMessage("Změny uloženy.","info");
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }

                return RedirectToActionPermanent("Designer", new { j72id = v.Rec.pid });
            }

            Designer_RefreshState(v);

            return View(v);
           
        }




        public TheGridOutput HandleTheGridFilter(int j72id, List<BO.TheGridColumnFilter> filter, int master_pid,int contextmenuflag)
        {
            var cJ72 = this.Factory.gridBL.LoadTheGridState(j72id);
            var lis = new List<string>();
            foreach (var c in filter)
            {                
                lis.Add(c.field + "###" + c.oper + "###" + c.value);
                
            }
            cJ72.j72CurrentPagerIndex = 0; //po změně filtrovací podmínky je nutné vyčistit paměť stránky
            cJ72.j72CurrentRecordPid = 0;
            cJ72.j72MasterPID = master_pid;
            cJ72.j72ContextMenuFlag = contextmenuflag;
            cJ72.j72Filter = string.Join("$$$", lis);
            
            if (this.Factory.gridBL.SaveTheGridState(cJ72) > 0)
            {
                return render_thegrid_html(cJ72);
            }
            else
            {
                return render_thegrid_error("Nepodařilo se zpracovat filtrovací podmínku.");
            }
        }
        public TheGridOutput HandleTheGridOper(int j72id,string oper,string key,string value, int master_pid,int contextmenuflag)
        {
            var cJ72 = this.Factory.gridBL.LoadTheGridState(j72id);
            cJ72.j72MasterPID = master_pid;
            cJ72.j72ContextMenuFlag = contextmenuflag;
            switch (key)
            {
                case "pagerindex":
                    cJ72.j72CurrentPagerIndex = BO.BAS.InInt(value);
                    break;
                case "pagesize":
                    cJ72.j72PageSize = BO.BAS.InInt(value);
                    break;
                case "sortfield":
                    if (cJ72.j72SortDataField != value)
                    {
                        cJ72.j72SortOrder = "asc";
                        cJ72.j72SortDataField = value;
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

            if (this.Factory.gridBL.SaveTheGridState(cJ72)> 0)
            {
                return render_thegrid_html(cJ72);
            }
            else
            {
                return render_thegrid_error("Nepodařilo se uložit GRIDSTATE");
            }

            
        }
        
        
        public TheGridOutput GetHtml4TheGrid(int j72id,int go2pid,int master_pid,int contextmenuflag) //Vrací HTML zdroj tabulky pro TheGrid v rámci j72TheGridState
        {
            
            var cJ72 = this.Factory.gridBL.LoadTheGridState(j72id);
            if (cJ72 == null)
            {
                return render_thegrid_error(string.Format("Nelze načíst grid state s id!", j72id.ToString()));
                
            }            
            cJ72.j72CurrentRecordPid = go2pid;
            cJ72.j72MasterPID = master_pid;
            cJ72.j72ContextMenuFlag = contextmenuflag;


            return render_thegrid_html(cJ72);
        }
        
        private System.Data.DataTable prepare_datatable(ref BO.myQuery mq, BO.j72TheGridState cJ72)
        {
            
            var colProvider = new BL.TheColumnsProvider(mq);
            mq.explicit_columns = colProvider.getSelectedPallete(cJ72.j72Columns);
            if (string.IsNullOrEmpty(cJ72.j72SortDataField)==false)
            {
                
                mq.explicit_orderby = colProvider.FindOneColumn(cJ72.j72SortDataField).getFinalSqlSyntax_ORDERBY(mq.Prefix) + " " + cJ72.j72SortOrder;
            }                           
            mq.j72Filter = cJ72.j72Filter;

            return Factory.gridBL.GetList(mq);
        }
        private TheGridOutput render_thegrid_html(BO.j72TheGridState cJ72)
        {
            var ret = new TheGridOutput();
            _grid = new TheGridViewModel() { Entity = cJ72.j72Entity };
            _grid.GridState = cJ72;

            ret.sortfield = cJ72.j72SortDataField;
            ret.sortdir = cJ72.j72SortOrder;
            
            var mq = new BO.myQuery(cJ72.j72Entity);            
            _grid.Columns = new BL.TheColumnsProvider(mq).getSelectedPallete(cJ72.j72Columns);            

            mq.explicit_columns = _grid.Columns;
            if (cJ72.j72SortDataField != "" && _grid.Columns.Where(p=>p.UniqueName==cJ72.j72SortDataField).Count()>0)
            {
                var c = _grid.Columns.Where(p => p.UniqueName == cJ72.j72SortDataField).First();
                mq.explicit_orderby = c.getFinalSqlSyntax_ORDERBY(cJ72.j72Entity.Substring(0,3)) + " " + cJ72.j72SortOrder;

            }
            mq.j72Filter = cJ72.j72Filter;
            switch (cJ72.j72MasterEntity)
            {
                case "p28Company":
                    mq.p28id = cJ72.j72MasterPID;
                    break;
                case "p10MasterProduct":
                    mq.p10id = cJ72.j72MasterPID;
                    break;
                case "p13MasterTpv":
                    mq.p13id = cJ72.j72MasterPID;               
                    break;
                case "p21License":
                    mq.p21id = cJ72.j72MasterPID;
                    break;
                case "p26Msz":
                    mq.p26id = cJ72.j72MasterPID;
                    break;
                default:
                    break;
            }
           
            
            var dt = Factory.gridBL.GetList(mq);
            mq.explicit_orderby = "";
            var dtFooter = Factory.gridBL.GetList(mq, true);
            int intVirtualRowsCount = Convert.ToInt32(dtFooter.Rows[0]["RowsCount"]);

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

            Render_DATAROWS(dt);
            ret.body = _s.ToString();
            _s = new System.Text.StringBuilder();

            Render_TOTALS(dtFooter);
            ret.foot = _s.ToString();
            _s = new System.Text.StringBuilder();

            RENDER_PAGER(intVirtualRowsCount);
            ret.pager = _s.ToString();
            return ret;
        }

        private void Render_DATAROWS(System.Data.DataTable dt)
        {
            int intRows = dt.Rows.Count;
            int intStartIndex = _grid.GridState.j72CurrentPagerIndex;
            int intEndIndex = intStartIndex + _grid.GridState.j72PageSize-1;
            if (intEndIndex+1 > intRows) intEndIndex = intRows-1;

            for (int i = intStartIndex; i <= intEndIndex; i++)
            {
                System.Data.DataRow dbRow = dt.Rows[i];
                var strRowClass = "class='selectable'";
                if (Convert.ToBoolean(dbRow["isclosed"])==true)
                {
                    strRowClass+= "class='trbin'";
                }

                _s.Append(string.Format("<tr id='r{0}' {1}>", dbRow["pid"],strRowClass));

                
                if (_grid.GridState.j72SelectableFlag>0)
                {
                    _s.Append(string.Format("<td class='td0' style='width:20px;'><input type='checkbox' id='chk{0}'/></td>", dbRow["pid"]));
                }
                else
                {
                    _s.Append("<td class='td0' style='width:20px;'></td>");
                }
                

                _s.Append("<td class='td1' style='width:20px;'></td>");
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
                    _s.Append(string.Format(">{0}</td>", ParseCellValueFromDb(dbRow, col)));
                    

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
                if (dt.Rows[0][col.Field] != System.DBNull.Value)
                {
                    strVal = ParseCellValueFromDb(dt.Rows[0], col);
                }
                _s.Append(string.Format(" style='width:{0}'>{1}</th>",col.ColumnWidthPixels, strVal));


            }
            _s.Append("</tr>");
        }



        string ParseCellValueFromDb(System.Data.DataRow dbRow, BO.TheGridColumn c)
        {
            if (dbRow[c.Field] == System.DBNull.Value)
            {
                return "";
            }
            switch (c.FieldType)
            {
                case "bool":
                    if (Convert.ToBoolean(dbRow[c.Field]) == true)
                    {
                        return "&#10004;";
                    }
                    else
                    {
                        return "";
                    }
                case "num0":
                    return string.Format("{0:#,0}", dbRow[c.Field]);

                case "num":
                    return string.Format("{0:#,0.00}", dbRow[c.Field]);
                  

                case "date":
                    return Convert.ToDateTime(dbRow[c.Field]).ToString("dd.MM.yyyy");


                case "datetime":
                    
                    return Convert.ToDateTime(dbRow[c.Field]).ToString("dd.MM.yyyy HH:mm");
                default:                    
                    return dbRow[c.Field].ToString();
            }


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

            _s.Append("<button title='První' class='btn btn-light tgp' style='margin-left:6px;' onclick='tg_pager(\n0\n)'>&#11207;|</button>");

            int intCurIndex = _grid.GridState.j72CurrentPagerIndex;
            int intPrevIndex = intCurIndex - intPageSize;
            if (intPrevIndex < 0) intPrevIndex = 0;
            _s.Append(string.Format("<button title='Předchozí' class='btn btn-light tgp' style='margin-right:10px;' onclick='tg_pager(\n{0}\n)'>&#11207;</button>", intPrevIndex));

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
            _s.Append(string.Format("<button title='Další' class='btn btn-light tgp' style='margin-left:10px;' onclick='tg_pager(\n{0}\n)'>&#11208;</button>", intNextIndex));

            int intLastIndex = intRowsCount - (intRowsCount % intPageSize);  //% je zbytek po celočíselném dělení
            _s.Append(string.Format("<button title='Poslední' class='btn btn-light tgp' onclick='tg_pager(\n{0}\n)'>|&#11208;</button>", intLastIndex));


        }





        public string GetHtml4TheCombo(string entity, string curvalue, string tableid, string param1, string pids) //Vrací HTML zdroj tabulky pro MyCombo
        {
            var mq = new BO.myQuery(entity);
            mq.SetPids(pids);
            mq.query_by_entity_prefix = param1;
            var cols = new BL.TheColumnsProvider(mq).getDefaultPallete();


            var dt = Factory.gridBL.GetList(mq);
            var intRows = dt.Rows.Count;

            var s = new System.Text.StringBuilder();


            s.Append(string.Format("<table id='{0}' class='table table-hover'>", tableid));
            s.Append("<thead><tr>");
            foreach (var col in cols)
            {
                s.Append(string.Format("<th>{0}</th>", col.Header));
            }
            s.Append(string.Format("</tr></thead><tbody id='{0}_tbody'>", tableid));
            for (int i = 0; i < intRows; i++)
            {
                s.Append(string.Format("<tr class='txz' data-v='{0}'>", dt.Rows[i]["pid"]));
                foreach (var col in cols)
                {
                    s.Append(string.Format("<td>{0}</td>", dt.Rows[i][col.Field]));
                }
                s.Append("</tr>");
            }
            s.Append("</tbody></table>");

            return s.ToString();
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

        public FileResult GridExport(string format,int j72id)
        {
            BO.j72TheGridState cJ72 = this.Factory.gridBL.LoadTheGridState(j72id);
            var mq = new BO.myQuery(cJ72.j72Entity);
            System.Data.DataTable dt = prepare_datatable(ref mq,cJ72);
            string filepath = Factory.App.TempFolder+"\\"+BO.BAS.GetGuid()+"."+ format;

            ToCSV(dt, filepath,mq);

            string s = "application/CSV";
            if (format == "xlsx") s = "application/vnd.ms-excel";

            return File(System.IO.File.ReadAllBytes(filepath), s,"gridexport."+ format);

        }

        private void ToCSV(System.Data.DataTable dt, string strFilePath,BO.myQuery mq)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(strFilePath,false,System.Text.Encoding.UTF8);
            //headers  
            foreach (var col in mq.explicit_columns)
            {
                sw.Write("\""+col.Header+"\"");
                sw.Write(";");
            }
            
            sw.Write(sw.NewLine);
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                foreach (var col in mq.explicit_columns)
                {
                    string value = "";

                    if (!Convert.IsDBNull(dr[col.Field]))
                    {
                        value = dr[col.Field].ToString();
                        if (col.FieldType == "string")
                        {
                            value = "\"" + value + "\"";
                        }
                    }
                    sw.Write(value);

                    sw.Write(";");


                }

                sw.Write(sw.NewLine);

            }
            sw.Close();
        }




    }
}