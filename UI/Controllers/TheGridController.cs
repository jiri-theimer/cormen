using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class TheGridController : BaseController
    {
        public IActionResult Index(string entity)
        {
            var v = new TheGridViewMode();
            v.Entity = entity;
            if (v.Entity == null) v.Entity = "p10";
            var mq = new BO.myQuery(v.Entity);

            
            
            v.grid1 = new MyGridViewModel(v.Entity,"pid","grid1_"+ v.Entity);
            
            switch (v.Entity)
            {
                case "p10":
                    v.grid1.AddStringCol("Název produktu", "p10Name");
                    v.grid1.AddStringCol("Kód", "p10Code");
                    v.grid1.AddStringCol("Stav", "b02Name");
                    v.grid1.AddLinkCol("TPV", "p13");
                    v.grid1.AddStringCol("Kategorie", "o12Name");
                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
                case "p13":
                    v.grid1.AddStringCol("Název", "p13Name");
                    v.grid1.AddStringCol("Kód", "p13Code");
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
                    v.grid1.AddStringCol("Název", "p21Name");
                    v.grid1.AddStringCol("Kód", "p21Code");
                    v.grid1.AddStringCol("Stav", "b02Name");
                    v.grid1.AddLinkCol("Klient", "p28");                    
                    v.grid1.AddDateCol("Platnost od", "ValidFrom");
                    v.grid1.AddDateCol("Platnost do", "ValidUntil");
                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
                case "p26":
                    v.grid1.AddStringCol("Název", "p26Name");
                    v.grid1.AddStringCol("Kód", "p26Code");
                    v.grid1.AddLinkCol("Klient", "p28");
                    v.grid1.AddStringCol("Stav", "b02Name");                            
                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
                case "p28":
                    v.grid1.AddStringCol("Název", "p28Name");                    
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
                    v.grid1.AddStringCol("Název", "o23Name");
                    v.grid1.AddLinkCol("Vazba", "RecordUrl");
                    v.grid1.AddStringCol("", "EntityAlias");

                    v.grid1.AddStringCol("Kategorie", "o12Name");
                    v.grid1.AddStringCol("Stav", "b02Name");
                    
                    
                    v.grid1.AddDateTimeCol("Založeno", "DateInsert");
                    break;
            }
            

            v.grid1.DT = Factory.gridBL.GetList(v.Entity, mq);

            return View(v);
        }
    }
}