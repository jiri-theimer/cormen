using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase        
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly BL.RunningApp _app;
        private readonly BL.Factory _f;

        public ApiController(ILogger<WeatherForecastController> logger, BL.RunningApp app)
        {
            _logger = logger;
            _app = app;
            var c = new BO.RunningUser() { j03Login = "treti@marktime.cz" };
            _f = new BL.Factory(c, _app); 
        }

        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            
            return "Service user: "+_f.CurrentUser.j03Login+" ("+_f.CurrentUser.FullName+")";
        }

        [HttpGet]
        [Route("load/{p41id:int}")]
        public TechnologickyRozpis LoadByPid(int p41id)
        {

            return handle_one_rozpis(p41id);
        }
        [HttpGet]
        [Route("load/{p41code:string}")]
        public TechnologickyRozpis LoadByCode(int p41id)
        {

            return handle_one_rozpis(p41id);
        }


        private TechnologickyRozpis handle_one_rozpis(int p41id)
        {
            var ret = new TechnologickyRozpis();
            
            var recP41 = _f.p41TaskBL.Load(p41id);
            var recP11 = _f.p11ClientProductBL.Load(recP41.p11ID);
            ret.Hlavicka = new HlavickaZakazky { VyrobniZakazka = recP41.p41Code, KodPolozky = recP11.p11Code, NazevPolozky = recP11.p11Name,Mnozstvi=recP41.p41PlanUnitsCount,Datum=recP41.DateInsert,Cas=recP41.DateInsert };
            ret.Hlavicka.ZPStart = recP41.PlanStartClear;
            ret.Hlavicka.ZPStop = recP41.p41PlanEnd;
            ret.PlanovaneMnozstvi = recP41.p41PlanUnitsCount;
            ret.Stav = recP41.b02ID;

            var mq = new BO.myQuery("p44TaskOperPlan");            
            mq.p18flags= new List<int>() { 1, 3 };
            mq.p41id = p41id;
            var lis = _f.p44TaskOperPlanBL.GetList(mq);
            foreach(var c in lis)
            {
                var recP18 = _f.p18OperCodeBL.Load(c.p18ID);
                var polozka = new PolozkaVyroby() { KodOperace = BO.BAS.InInt(recP18.p18Code), CisloOperace = c.p44OperNum,Start=c.p44Start,Stop=c.p44End,Popis=recP18.p18Name };
                polozka.Parametr = c.p44OperParam;
                if (c.p19ID > 0)
                {
                    var recP19 = _f.p19MaterialBL.Load(c.p19ID);
                    polozka.MaterialKod = recP19.p19Code;
                }
                
            }



            return ret;
        }


    }
}