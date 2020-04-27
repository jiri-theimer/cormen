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

        public string GetHtml4TheGrid(int j72id) //Vrací HTML zdroj tabulky pro TheGrid v rámci j72TheGridState
        {
            var cJ72 = this.Factory.gridBL.LoadTheGridState(j72id);
            if (cJ72 == null)
            {
                return string.Format("<code>Nelze načíst grid state s id!</code>", j72id.ToString());
            }
            _grid = new TheGridViewModel() { Entity = cJ72.j72Entity };
            _grid.GridState = cJ72;
           
            
            var mq = new BO.myQuery(cJ72.j72Entity);
            _grid.Columns = new BL.TheGridColumns(mq).getSelectedPallete(cJ72.j72Columns);
            
            mq.explicit_columns = _grid.Columns;
            var dt = Factory.gridBL.GetList(mq);
            var dtFooter = Factory.gridBL.GetList(mq,true);

            _s = new System.Text.StringBuilder();

            Render_DATAROWS(dt);

            
            Render_TOTALS(dtFooter);

            return _s.ToString();
        }

        private void Render_DATAROWS(System.Data.DataTable dt)
        {
            var intRows = dt.Rows.Count;
            
            for (int i = 0; i < intRows; i++)
            {
                System.Data.DataRow dbRow = dt.Rows[i];
                var strRowClass = "";
                if (Convert.ToBoolean(dbRow["isclosed"])==true)
                {
                    strRowClass = "class='trbin'";
                }

                _s.Append(string.Format("<tr id='r{0}' {1}>", dbRow["pid"],strRowClass));

                
                if (_grid.GridState.j72SelectableFlag == 1)
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
                    
                    if (col.FixedWidth > 0)
                    {
                        _s.Append(string.Format(" style='width:{0}'", col.FixedWidth));
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
            foreach (var col in _grid.Columns)
            {
                _s.Append("<td");
                if (col.CssClass != null)
                {
                    _s.Append(string.Format(" class='{0}'", col.CssClass));
                }

                
                _s.Append(string.Format(" style='width:{0}'>{1}</td>",col.ColumnWidthPixels, ParseCellValueFromDb(dt.Rows[0], col)));


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










        public string GetHtml4TheCombo(string entity, string curvalue, string tableid, string param1, string pids) //Vrací HTML zdroj tabulky pro MyCombo
        {
            var mq = new BO.myQuery(entity);
            mq.SetPids(pids);
            mq.query_by_entity_prefix = param1;
            var cols = new BL.TheGridColumns(mq).getDefaultPallete();


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
            mq.explicit_columns = new BL.TheGridColumns(mq).getDefaultPallete();
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




        private string DataTableToJSONWithJSONNet(System.Data.DataTable dt)
        {

            return JsonConvert.SerializeObject(dt, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });




        }




    }
}