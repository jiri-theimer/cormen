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

        public string GetHtml4TheCombo(string entity, string curvalue, string tableid, string param1, string pids) //Vrací HTML zdroj tabulky pro MyCombo
        {
            var mq = new BO.myQuery(entity);
            mq.SetPids(pids);
            var cols = new BL.TheGridColumns(mq).getDefaultPallete();

            var dt = Factory.gridBL.GetList(mq);
            var intRows = dt.Rows.Count;

            var s = new System.Text.StringBuilder();


            s.Append(string.Format("<table id='{0}' class='table table-sm table-hover' style='font-size:90%;'>", tableid));
            s.Append("<thead><tr>");
            foreach (var col in cols)
            {
                s.Append(string.Format("<th>{0}</th>", col.Header));
            }
            s.Append(string.Format("</tr></thead><tbody id='{0}_tbody'>",tableid));
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


        public ActionResult GetJson4TheCombo(string entity,string text,bool addblankrow)
        {
            System.IO.File.AppendAllText("c:\\temp\\hovado.txt", "entity: " + entity+", čas: "+DateTime.Now.ToString());
            var mq = new BO.myQuery(entity);          
            mq.explicit_columns = new BL.TheGridColumns(mq).getDefaultPallete();
            mq.SearchString = text;//fulltext hledání
            var dt = Factory.gridBL.GetList( mq);

            if (addblankrow==true)
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

            
            

            return new ContentResult() { Content =DataTableToJSONWithJSONNet(dt), ContentType = "application/json" };
            
        }




        private string DataTableToJSONWithJSONNet(System.Data.DataTable dt)
        {

            return JsonConvert.SerializeObject(dt, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });




        }











        public IActionResult Index(string entity)
        {
            var v = new TheGridViewMode();
            v.Entity = entity;
            if (v.Entity == null) v.Entity = "p10";
            var mq = new BO.myQuery(v.Entity);



            v.grid1 = new MyGridViewModel(v.Entity, "pid", "grid1_" + v.Entity);

            switch (v.Entity)
            {
                case "p10":
                    v.grid1.AddLinkCol("Název produktu", "p10");
                    v.grid1.AddStringCol("Kód", "p10Code");
                    v.grid1.AddStringCol("Stav", "b02Name");
                    v.grid1.AddStringCol("TPV", "p13Code");
                    v.grid1.AddStringCol("Kategorie", "o12Name");
                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
                case "p13":
                    v.grid1.AddLinkCol("Název", "p13");
                    v.grid1.AddStringCol("Číslo postupu", "p13Code");
                    v.grid1.AddStringCol("Popis", "p13Memo");
                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
                case "j02":
                    v.grid1.AddStringCol("", "j02TitleBeforeName");
                    v.grid1.AddStringCol("Jméno", "j02FirstName");
                    v.grid1.AddStringCol("Příjmení", "j02LastName");
                    v.grid1.AddStringCol("E-mail", "j02Email");
                    v.grid1.AddStringCol("Role", "j04Name");
                    v.grid1.AddLinkCol("Firma", "p28");
                    v.grid1.AddStringCol("Mobil", "j02Tel1");
                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
                case "p21":
                    v.grid1.AddLinkCol("Název", "p21");
                    v.grid1.AddStringCol("Kód", "p21Code");
                    v.grid1.AddStringCol("Stav", "b02Name");
                    v.grid1.AddLinkCol("Klient", "p28");
                    v.grid1.AddDateCol("Platnost od", "ValidFrom");
                    v.grid1.AddDateCol("Platnost do", "ValidUntil");
                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
                case "p26":
                    v.grid1.AddLinkCol("Název", "p26");
                    v.grid1.AddStringCol("Kód", "p26Code");
                    v.grid1.AddLinkCol("Klient", "p28");
                    v.grid1.AddStringCol("Stav", "b02Name");
                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
                case "p28":
                    v.grid1.AddLinkCol("Název", "p28");
                    v.grid1.AddStringCol("Město", "p28City1");
                    v.grid1.AddStringCol("Ulice", "p28Street1");
                    v.grid1.AddStringCol("Kód", "p28Code");
                    v.grid1.AddStringCol("IČ", "p28RegID");
                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
                case "b02":
                    v.grid1.AddStringCol("Název", "b02Name");
                    v.grid1.AddStringCol("Kód", "b02Code");
                    v.grid1.AddStringCol("Vazba", "EntityAlias");
                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
                case "o12":
                    v.grid1.AddStringCol("Název", "o12Name");
                    v.grid1.AddStringCol("Kód", "o12Name");
                    v.grid1.AddStringCol("Vazba", "EntityAlias");
                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
                case "o23":
                    v.grid1.AddLinkCol("Název", "o23");
                    v.grid1.AddLinkCol("Vazba", "RecordUrl");
                    v.grid1.AddStringCol("", "EntityAlias");

                    v.grid1.AddStringCol("Kategorie", "o12Name");
                    v.grid1.AddStringCol("Stav", "b02Name");


                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
            }



            v.grid1.DT = Factory.gridBL.GetList( mq);

            return View(v);
        }

    }
}