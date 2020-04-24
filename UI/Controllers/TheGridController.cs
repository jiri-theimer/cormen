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
        
        public ActionResult GetJson4TheCombo(string entity,string fields, string text)
        {
            var mq = new BO.myQuery(entity);
            mq.explicit_selectfields = fields;
            mq.SearchString = text;//fulltext hledání
            var dt = Factory.gridBL.GetList( mq);


            
            return new ContentResult() { Content = UI.DATA.DataTableToJSONWithJSONNet(dt), ContentType = "application/json" };
            
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