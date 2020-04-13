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
            if (entity == null) entity = "p10";
            var mq = new BO.myQuery(entity);

            var v = new TheGridViewMode();
            v.grid1 = new MyGridViewModel();            

            v.grid1.AddStringCol("Název produktu", "p10Name");
            v.grid1.AddStringCol("Kód", "p10Code");
            v.grid1.AddStringCol("Stav", "b02Name");
            v.grid1.AddStringCol("TPV", "p13Name");
            v.grid1.AddStringCol("Kategorie", "o12Name");

            v.grid1.DT = Factory.gridBL.GetList(entity, mq);

            return View(v);
        }
    }
}