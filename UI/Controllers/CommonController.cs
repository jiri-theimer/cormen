using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class CommonController : BaseController
    {
        
        public IActionResult Index()
        {
            return View();
        }
      

        public string DeleteRecord(string entity, int pid)  //Univerzální mazání záznamů
        {

            return this.Factory.CBL.DeleteRecord(entity, pid);
        }


        public BO.Result SetUserParam(string key, string value)
        {
            if (Factory.CBL.SetUserParam(key, value))
            {
                return new BO.Result(false);
            }
            else
            {
                return new BO.Result(true);
            }
                       
        }
        public string LoadUserParam(string key)
        {
            return Factory.CBL.LoadUserParam(key);
        }


        public string GetBodyOfTale(string entity, string queryfield, string queryvalue)
        {           
            var mq = new BO.myQuery(entity);
            if (string.IsNullOrEmpty(queryfield) == false)
            {
                BO.Reflexe.SetPropertyValue(mq, queryfield, queryvalue);
            }
            mq.explicit_columns = new BL.TheColumnsProvider(mq).getDefaultPallete();
            var dt = Factory.gridBL.GetList(mq);
            var intRows = dt.Rows.Count;
            
            var s = new System.Text.StringBuilder();
            foreach(var col in mq.explicit_columns)
            {

            }
            for (int i = 0; i < intRows; i++)
            {
                s.Append(string.Format("<tr data-v='{0}'>", dt.Rows[i]["pid"]));
                foreach (var col in mq.explicit_columns)
                {
                    s.Append(string.Format("<td>{0}</td>", dt.Rows[i][col.Field]));
                }
                s.Append("</tr>");

               
            }
            return s.ToString();
        }
        public string GetWorkTable(string entity, string tableid, string param1, string pids,string delete_function,string queryfield,string queryvalue)
        {                
            var mq = new BO.myQuery(entity);
            mq.SetPids(pids);
            if (string.IsNullOrEmpty(queryfield) == false)
            {
                BO.Reflexe.SetPropertyValue(mq, queryfield, queryvalue);
            }
            
            mq.explicit_columns = new BL.TheColumnsProvider(mq).getDefaultPallete();

            var dt = Factory.gridBL.GetList(mq);
            var intRows = dt.Rows.Count;

            var s = new System.Text.StringBuilder();
           
            s.Append(string.Format("<table id='{0}' class='table table-sm table-hover'>", tableid));

            for (int i = 0; i < intRows; i++)
            {
                s.Append(string.Format("<tr data-v='{0}'>", dt.Rows[i]["pid"]));
                foreach (var col in mq.explicit_columns)
                {
                    s.Append(string.Format("<td>{0}</td>", dt.Rows[i][col.Field]));
                }
                if (delete_function != null)
                {
                    s.Append(string.Format("<td><button type='button' class='btn btn-sm btn-danger' title='Odstranit řádek' onclick='{0}({1})'>&times;</button></td>", delete_function, dt.Rows[i]["pid"]));
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
            var mq = new BO.myQuery("p28Company");
            mq.SearchString = expr;

            var s = new System.Text.StringBuilder();
            s.Append("<div style='background-color:silver;'><button type='button' onclick='searchbox1_destroy()' class='btn btn-secondary py-0'>Zavřít</button></div>");
            s.Append("<table class='table table-sm table-hover' onclick='searchbox1_destroy()'>");
            var dt = Factory.gridBL.GetList(mq);
            var intRows = dt.Rows.Count;
            if (intRows > 20) intRows = 20;

            for (int i = 0; i <= intRows-1; i++){
                s.Append(string.Format("<tr class='table-primary'><td><a href='/p28/?pid={0}'>{1}</a></td><td>{2}</td><td>Klient</td></tr>",dt.Rows[i]["pid"],dt.Rows[i]["p28Name"], dt.Rows[i]["p28Code"]));
            }

            mq.Entity = "p10MasterProduct";
            dt = Factory.gridBL.GetList(mq);
            intRows = dt.Rows.Count;
            if (intRows > 20) intRows = 20;
            for (int i = 0; i <= intRows - 1; i++)
            {
                s.Append(string.Format("<tr class='table-success'><td><a href='/p10/?pid={0}'>{1}</a></td><td>{2}</td><td>Master produkt</td></tr>", dt.Rows[i]["pid"], dt.Rows[i]["p10Name"], dt.Rows[i]["p10Code"]));
            }

            mq.Entity = "o23Document";
            dt = Factory.gridBL.GetList(mq);
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