using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UI.ModelsApi;
using BL;

namespace UI.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    
    public class ApiController : ControllerBase
    {
        
        private readonly BL.RunningApp _app;
        private readonly BL.Factory _f;

        public ApiController( BL.RunningApp app)
        {
            
            _app = app;
            var c = new BO.RunningUser() { j03Login = "treti@marktime.cz" };
            _f = new BL.Factory(c, _app);
        }

        

        [HttpGet]
        [Route("test/{pid:int}")]
        public BO.p41Task TestRecord(int pid)
        {
            var c = new BO.p41Task() { p41ID = pid, p41Name = "hovado" };

            return c;
            
        }


        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            //return DateTime.Now.ToString();
            return "Service user: " + _f.CurrentUser.j03Login + " (" + _f.CurrentUser.FullName + ")";
        }


        [HttpGet]
        [Route("technologicky_rozpis_load/{p41id:int}")]
        public TechnologickyRozpis LoadByPid(int p41id)
        {
            var c = _f.p41TaskBL.Load(p41id);
            return handle_one_rozpis(c);
        }
        [HttpGet]
        [Route("technologicky_rozpis_load/{p41code}")]
        public TechnologickyRozpis LoadByCode(string p41code)
        {
            var c = _f.p41TaskBL.LoadByCode(p41code, 0);
            return handle_one_rozpis(c);
        }

        private TechnologickyRozpis handle_one_rozpis(BO.p41Task recP41)
        {
            var ret = new TechnologickyRozpis();


            var recP11 = _f.p11ClientProductBL.Load(recP41.p11ID);
            ret.Hlavicka = new HlavickaZakazky { VyrobniZakazka = recP41.p41Code, KodPolozky = recP11.p11Code, NazevPolozky = recP11.p11Name, Mnozstvi = recP41.p41PlanUnitsCount, Datum = recP41.DateInsert, Cas = recP41.DateInsert };
            ret.Hlavicka.ZPStart = recP41.PlanStartClear;
            ret.Hlavicka.ZPStop = recP41.p41PlanEnd;
            ret.PlanovaneMnozstvi = recP41.p41PlanUnitsCount;
            ret.Stav = recP41.b02ID;

            var mq = new BO.myQuery("p44TaskOperPlan");
            mq.p18flags = new List<int>() { 1, 3 };
            mq.p41id = recP41.pid;
            var lis = _f.p44TaskOperPlanBL.GetList(mq);

            ret.Polozky = new List<PolozkaVyroby>();
            foreach (var c in lis)
            {
                var recP18 = _f.p18OperCodeBL.Load(c.p18ID);
                var polozka = new PolozkaVyroby() { KodOperace = BO.BAS.InInt(recP18.p18Code), CisloOperace = c.p44OperNum, Start = c.p44Start, Stop = c.p44End, Popis = recP18.p18Name };
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


        [HttpGet]
        //[Route("movetaskstatus/{p41code}/{b02code}")]
        [Route("move_task_status")]
        public BO.Result MoveTaskStatus(string p41code, string b02code)
        {
            return _f.p41TaskBL.MoveStatus(p41code, b02code);
        }

        [HttpGet]
        //[Route("movetaskstatus/{p41code}/{b02code}")]
        [Route("technologicky_rozpis_load_by_b02_and_p27")]
        public TechnologickyRozpis LoadByStatusAndUnit(string b02code,string p27code)
        {            
            var recB02 = _f.b02StatusBL.LoadByCode(b02code);
            if (recB02 == null)
            {
                return new TechnologickyRozpis() { ErrorMessage = "Nelze načíst status s tímto kódem." };
            }
            var recP27 = _f.p27MszUnitBL.LoadByCode(p27code,0);
            if (recP27 == null)
            {
                return new TechnologickyRozpis() { ErrorMessage = "Nelze načíst středisko s tímto kódem." };
            }
            var mq = new BO.myQuery("p41Task");
            var lis = _f.p41TaskBL.GetList(mq).Where(p => p.b02ID == recB02.pid && p.p27ID == recP27.pid).OrderBy(p => p.p41PlanStart);
            if (lis.Count() == 0)
            {
                return new TechnologickyRozpis() { ErrorMessage = "0 zakázek s tímto filtrem" };
            }

            
            return handle_one_rozpis(lis.ToList()[0]);
        }


    }
}