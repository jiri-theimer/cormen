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
        private readonly ILogger<ApiController> _logger;
        private readonly BL.RunningApp _app;
        private readonly BL.Factory _f;

        public ApiController(ILogger<ApiController> logger, BL.RunningApp app)
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
            //return DateTime.Now.ToString();
            return "Service user: "+_f.CurrentUser.j03Login+" ("+_f.CurrentUser.FullName+")";
        }


        [HttpGet]
        [Route("trload/{p41id:int}")]
        public TechnologickyRozpis LoadByPid(int p41id)
        {
            var c = _f.p41TaskBL.Load(p41id);
            return handle_one_rozpis(c);
        }
        [HttpGet]
        [Route("trload/{p41code}")]
        public TechnologickyRozpis LoadByCode(string p41code)
        {
            var c = _f.p41TaskBL.LoadByCode(p41code, 0);
            return handle_one_rozpis(c);
        }

        private TechnologickyRozpis handle_one_rozpis(BO.p41Task recP41)
        {
            var ret = new TechnologickyRozpis();
            
            
            var recP11 = _f.p11ClientProductBL.Load(recP41.p11ID);
            ret.Hlavicka = new HlavickaZakazky { VyrobniZakazka = recP41.p41Code, KodPolozky = recP11.p11Code, NazevPolozky = recP11.p11Name,Mnozstvi=recP41.p41PlanUnitsCount,Datum=recP41.DateInsert,Cas=recP41.DateInsert };
            ret.Hlavicka.ZPStart = recP41.PlanStartClear;
            ret.Hlavicka.ZPStop = recP41.p41PlanEnd;
            ret.PlanovaneMnozstvi = recP41.p41PlanUnitsCount;
            ret.Stav = recP41.b02ID;

            var mq = new BO.myQuery("p44TaskOperPlan");            
            mq.p18flags= new List<int>() { 1, 3 };
            mq.p41id = recP41.pid;
            var lis = _f.p44TaskOperPlanBL.GetList(mq);

            ret.Polozky = new List<PolozkaVyroby>();
            foreach(var c in lis)
            {
                var recP18 = _f.p18OperCodeBL.Load(c.p18ID);
                var polozka = new PolozkaVyroby() { KodOperace = BO.BAS.InInt(recP18.p18Code), CisloOperace = c.p44OperNum,Start=c.p44Start,Stop=c.p44End,Popis=recP18.p18Name };
                polozka.Parametr = c.p44OperParam;
                if (c.p19ID > 0)
                {
                    var recP19 = _f.p19MaterialBL.Load(c.p19ID);
                    polozka.MaterialKod = recP19.p19Code;
                    polozka.MaterialNazev = recP19.p19Name;                    
                }
                polozka.VahaPozadovana = c.p44MaterialUnitsCount;
                polozka.DelkaTrvani = c.p44TotalDurationOperMin;
                ret.Polozky.Add(polozka);
            }



            return ret;
        }


    }
}