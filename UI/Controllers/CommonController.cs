using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class CommonController : BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult StopModal(string message)
        {
            ViewBag.message = message;
            return View();
        }
        public IActionResult Stop(string message)
        {
            ViewBag.message = message;
            return View();
        }

        public string DeleteRecord(string entity, int pid)  //Univerzální mazání záznamů
        {

            return this.Factory.CBL.DeleteRecord(entity, pid);
        }


        public string GetComboHtmlItems(string entity, string curvalue, string tableid,string param1) //Vrací HTML zdroj tabulky pro MyCombo
        {
            var mq = new BO.myQuery(entity);                        
            string strCols = string.Format("{0}Name", entity);

            switch (entity)
            {
                case "j02":
                    strCols = "fullname_desc,j04Name,j02Email,p28Name";
                    break;
                case "p10":
                    strCols = "p10Name,p10Code,b02Name,p13Name,o12Name";
                    break;
                case "p28":
                    strCols = "p28Name,p28RegID,p28City1,p28Country1";
                    break;
                case "p26":
                    strCols = "p26Name,p26Code";
                    break;
                case "p13":
                    strCols = "p13Name,p13Code,p13Memo";
                    break;
                case "b02":
                    mq.query_by_entity_prefix = param1;
                    break;
                case "o12":
                    mq.query_by_entity_prefix = param1;
                    break;
                default:
                    break;
            }

            var dt = Factory.gridBL.GetList(entity, mq);
            var intRows = dt.Rows.Count;

            var s = new System.Text.StringBuilder();
            var cols = BO.BAS.ConvertString2List(strCols);

            s.Append(string.Format("<table id='{0}' class='table table-sm table-hover' style='font-size:90%;'>", tableid));
            for (int i = 0; i < intRows; i++)
            {
                s.Append(string.Format("<tr class='txz' data-v='{0}'>", dt.Rows[i]["pid"]));
                foreach (var strCol in cols)
                {
                    s.Append(string.Format("<td>{0}</td>", dt.Rows[i][strCol]));
                }
                s.Append("</tr>");
            }
            s.Append("</table>");
          
            return s.ToString();
        }
        public string GetAutoCompleteHtmlItems(int o15flag, string tableid) //Vrací HTML zdroj tabulky pro MyAutoComplete
        {
            var lis = Factory.FBL.GetListAutoComplete(o15flag);
            var s = new System.Text.StringBuilder();
            s.Append(string.Format("<table id='{0}' class='table table-sm table-hover'>", tableid));
            foreach (var item in lis)
            {
                s.Append(string.Format("<tr class='txz'><td>{0}</td></tr>", item.Value));
            }
            s.Append("</table>");

            return s.ToString();
        }
    }

   

    
    
}