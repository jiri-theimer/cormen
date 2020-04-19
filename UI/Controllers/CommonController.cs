using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

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

        private System.Data.DataTable CompleteDT(ref string strCols,string entity,  string param1, string pids, string queryfield, string queryvalue)
        {
            var mq = new BO.myQuery(entity);
            if (pids != null) mq.pids = BO.BAS.ConvertString2ListInt(pids);
            if (queryfield != null && queryvalue !=null)
            {                        
               BO.Reflexe.SetPropertyValue(mq, queryfield, queryvalue);    //reflexe
            }
            

            
            strCols = string.Format("{0}Name", entity);

            switch (entity)
            {
                case "j02":
                    strCols = "fullname_desc,j04Name,j02Email,p28Name";
                    break;
                case "p10":
                    strCols = "p10Name,p10Code,b02Name,p13Code,o12Name";
                    break;
                case "p21":
                    strCols = "p21Name,p21Code,p28Name,b02Name";
                    break;
                case "p28":
                    strCols = "p28Name,p28RegID,p28City1,p28Country1";
                    break;
                case "p26":
                    strCols = "p26Name,p26Code,p28Name,b02Name";
                    break;
                case "p13":
                    strCols = "p13Name,p13Code,p13Memo";
                    break;
                case "p14":
                    mq.explicit_orderby = "a.p14RowNum";
                    strCols = "p14RowNum,p14OperNum,p14OperCode,p14Name,p14OperParam,p14MaterialCode,p14MaterialName,p14UnitsCount,p14DurationPreOper,p14DurationOper,p14DurationPostOper";
                    break;
                case "o23":
                    strCols = "o23Name,RecordUrlName,o12Name";
                    break;

                case "b02":
                    strCols = "b02Name,b02Code";
                    mq.query_by_entity_prefix = param1;
                    break;
                case "o12":
                    strCols = "o12Name,o12Code";
                    mq.query_by_entity_prefix = param1;
                    break;
                default:
                    break;
            }

            return Factory.gridBL.GetList(entity, mq);
        }

        public string GetBodyOfTale(string entity, string queryfield, string queryvalue)
        {
            string strCols = "";
            var dt = CompleteDT(ref strCols, entity,null, null, queryfield, queryvalue);
            var intRows = dt.Rows.Count;

            var s = new System.Text.StringBuilder();
            var cols = BO.BAS.ConvertString2List(strCols);
            for (int i = 0; i < intRows; i++)
            {
                s.Append(string.Format("<tr data-v='{0}'>", dt.Rows[i]["pid"]));
                foreach (var strCol in cols)
                {
                    s.Append(string.Format("<td>{0}</td>", dt.Rows[i][strCol]));
                }
                
                s.Append("</tr>");
            }
            return s.ToString();
        }
        public string GetWorkTable(string entity, string tableid, string param1, string pids,string delete_function,string queryfield,string queryvalue)
        {
            string strCols = "";
            var dt = CompleteDT(ref strCols, entity, param1, pids,queryfield,queryvalue);
            var intRows = dt.Rows.Count;

            var s = new System.Text.StringBuilder();
            var cols = BO.BAS.ConvertString2List(strCols);
            s.Append(string.Format("<table id='{0}' class='table table-sm table-hover'>", tableid));

            for (int i = 0; i < intRows; i++)
            {
                s.Append(string.Format("<tr data-v='{0}'>", dt.Rows[i]["pid"]));
                foreach (var strCol in cols)
                {
                    s.Append(string.Format("<td>{0}</td>", dt.Rows[i][strCol]));
                }
                if (delete_function != null)
                {
                    s.Append(string.Format("<td><button type='button' title='Odstranit řádek' onclick='{0}({1})'><i class='fas fa-trash-alt'></i></button></td>", delete_function, dt.Rows[i]["pid"]));
                }
                s.Append("</tr>");
            }
            s.Append("</table>");

            return s.ToString();
        }
        public string GetComboHtmlItems(string entity, string curvalue, string tableid,string param1,string pids) //Vrací HTML zdroj tabulky pro MyCombo
        {
            string strCols = "";
            var dt = CompleteDT(ref strCols,entity,param1, pids,null,null);
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


        public string GetSearchBoxHtml(string expr)
        {
            //Vrací HTML zdroj tabulky pro searchbox
            if (expr.TrimEnd().Length <= 1)
            {
                return "<tr><td>Je třeba zadat minimálně 2 znaky.</td></tr>";
            }
            var mq = new BO.myQuery("p28");
            mq.SearchString = expr;

            var s = new System.Text.StringBuilder();
            s.Append("<div style='background-color:silver;'><button type='button' onclick='searchbox1_destroy()' class='btn btn-secondary py-0'>Zavřít</button></div>");
            s.Append("<table class='table table-sm table-hover' onclick='searchbox1_destroy()'>");
            var dt = Factory.gridBL.GetList("p28", mq);
            var intRows = dt.Rows.Count;
            if (intRows > 20) intRows = 20;

            for (int i = 0; i <= intRows-1; i++){
                s.Append(string.Format("<tr class='table-primary'><td><a href='/p28/?pid={0}'>{1}</a></td><td>{2}</td><td>Klient</td></tr>",dt.Rows[i]["pid"],dt.Rows[i]["p28Name"], dt.Rows[i]["p28Code"]));
            }

            mq.Entity = "p10";
            dt = Factory.gridBL.GetList("p10", mq);
            intRows = dt.Rows.Count;
            if (intRows > 20) intRows = 20;
            for (int i = 0; i <= intRows - 1; i++)
            {
                s.Append(string.Format("<tr class='table-success'><td><a href='/p10/?pid={0}'>{1}</a></td><td>{2}</td><td>Master produkt</td></tr>", dt.Rows[i]["pid"], dt.Rows[i]["p10Name"], dt.Rows[i]["p10Code"]));
            }

            mq.Entity = "o23";
            dt = Factory.gridBL.GetList("o23", mq);
            intRows = dt.Rows.Count;
            if (intRows > 20) intRows = 20;
            for (int i = 0; i <= intRows - 1; i++)
            {
                s.Append(string.Format("<tr class='table-warning'><td><a href='/o23/?pid={0}'>{1}</a></td><td>{2}</td><td>Dokument</td></tr>", dt.Rows[i]["pid"], dt.Rows[i]["o23Name"], dt.Rows[i]["RecordUrlName"]));
            }


            s.Append("</table>");

            return s.ToString();
        }

        public BO.p85Tempbox Save2Temp(int p85id,string guid,string prefix,int recpid,string fieldname,string fieldvalue)
        {
            var rec = Factory.p85TempboxBL.Load(p85id);
            if (rec == null)
            {
                rec = new BO.p85Tempbox() { p85GUID = guid, p85Prefix = prefix };

                
            }
            if (fieldname.ToLower().Contains("freenumber")){
                if (fieldvalue == "0" || fieldvalue == "") fieldvalue = null;
            }

            BO.Reflexe.SetPropertyValue(rec, fieldname, fieldvalue);

            p85id = Factory.p85TempboxBL.Save(rec);
            if (p85id > 0)
            {
                rec = Factory.p85TempboxBL.Load(p85id);
            }
            
            return rec;

        }
    }

   

    
    
}