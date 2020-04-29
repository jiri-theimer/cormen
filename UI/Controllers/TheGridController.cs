using System;
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

        public TheGridOutput HandleTheGridFilter(int j72id, List<BO.TheGridColumnFilter> filter)
        {
            var cJ72 = this.Factory.gridBL.LoadTheGridState(j72id);
            var lis = new List<string>();
            foreach (var c in filter)
            {                
                lis.Add(c.field + "###" + c.oper + "###" + c.value);
                
            }
            cJ72.j72CurrentPagerIndex = 0; //po změně filtrovací podmínky je nutné vyčistit paměť stránky
            cJ72.j72CurrentRecordPid = 0;
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
        public TheGridOutput HandleTheGridOper(int j72id,string oper,string key,string value)
        {
            var cJ72 = this.Factory.gridBL.LoadTheGridState(j72id);
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
        

        public TheGridOutput GetHtml4TheGrid(int j72id) //Vrací HTML zdroj tabulky pro TheGrid v rámci j72TheGridState
        {
            
            var cJ72 = this.Factory.gridBL.LoadTheGridState(j72id);
            if (cJ72 == null)
            {
                return render_thegrid_error(string.Format("Nelze načíst grid state s id!", j72id.ToString()));
                
            }
            return render_thegrid_html(cJ72);
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
                mq.explicit_orderby = c.FinalSqlSyntaxOrderBy + " " + cJ72.j72SortOrder;

            }
            mq.j72Filter = cJ72.j72Filter;
            
            var dt = Factory.gridBL.GetList(mq);
            mq.explicit_orderby = "";
            var dtFooter = Factory.gridBL.GetList(mq, true);

            _s = new System.Text.StringBuilder();

            Render_DATAROWS(dt);
            ret.body = _s.ToString();
            _s = new System.Text.StringBuilder();

            Render_TOTALS(dtFooter);
            ret.foot = _s.ToString();
            _s = new System.Text.StringBuilder();

            RENDER_PAGER(Convert.ToInt32(dtFooter.Rows[0]["RowsCount"]));
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
                

                _s.Append("<td class='td1' style='width:20px;'>");
                _s.Append("<td class='td2' style='width:20px;'>");


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
            _s.Append(string.Format("<th class='th0' title='Celkový počet záznamů' colspan=3 style='width:60px;'>{0}</th>", string.Format("{0:#,0}", dt.Rows[0]["RowsCount"])));
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


        public ActionResult GetJson4TheCombo(string entity, string text, bool addblankrow)
        {
            System.IO.File.AppendAllText("c:\\temp\\hovado.txt", "entity: " + entity + ", čas: " + DateTime.Now.ToString());
            var mq = new BO.myQuery(entity);
            mq.explicit_columns = new BL.TheColumnsProvider(mq).getDefaultPallete();
            mq.SearchString = text;//fulltext hledání
            var dt = Factory.gridBL.GetList(mq);

            if (addblankrow == true)
            {
                System.Data.DataRow newBlankRow = dt.NewRow();
                dt.Rows.InsertAt(newBlankRow, 0);
            }


            foreach (System.Data.DataRow row in dt.Rows)
            {
                foreach (System.Data.DataColumn col in dt.Columns)
                {
                    if (col.DataType.Name == "String")
                    {
                        if (row[col.ColumnName] == DBNull.Value)
                        {
                            row[col.ColumnName] = "";
                        }
                    }

                }
            }




            return new ContentResult() { Content = DataTableToJSONWithJSONNet(dt), ContentType = "application/json" };

        }



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

        private string DataTableToJSONWithJSONNet(System.Data.DataTable dt)
        {

            return JsonConvert.SerializeObject(dt, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });




        }




    }
}