using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UI.ModelsApi;
using BL;
using DocumentFormat.OpenXml.Drawing;

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

        [HttpGet]        
        [Route("skutecna_vyroba_save")]
        public BO.Result skutecna_vyroba_save(string p41Code,string p45OperCode,string p45MaterialCode,string p45MaterialBatch,double p45MaterialUnitsCount,double p45TotalDurationOperMin,DateTime p45Start, DateTime p45End,string p45OperStatus,int p45OperNum,double p45OperParam,string p45Operator,int p45RowNum)
        {
            var c = new SkutecnaVyroba() {
                p41Code = p41Code, p45OperCode= p45OperCode, p45MaterialCode= p45MaterialCode
                , p45MaterialBatch= p45MaterialBatch, p45MaterialUnitsCount= p45MaterialUnitsCount, p45TotalDurationOperMin= p45TotalDurationOperMin                
                ,p45Start= p45Start
                ,p45End= p45End
                ,p45OperStatus= p45OperStatus                
                ,p45OperNum= p45OperNum
                ,p45OperParam= p45OperParam
                ,p45Operator= p45Operator
                ,p45RowNum= p45RowNum
            };

            var recP41 = _f.p41TaskBL.LoadByCode(c.p41Code,0);
            if (recP41 == null)
            {
                return new BO.Result(true,string.Format("Nelze načíst zakázku s kódem: {0}.", c.p41Code));
            }
            if (string.IsNullOrEmpty(c.p45OperCode) == true)
            {
                return new BO.Result(true, "Na vstupu chybí kód operace: p45OperCode.");
            }

            var ret = new BO.p45TaskOperReal() { p41ID = recP41.pid };

            if (string.IsNullOrEmpty(c.p45MaterialCode) == false)
            {
                var recP19 = _f.p19MaterialBL.LoadByCode(c.p45MaterialCode,0);
                if (recP19 == null)
                {
                    return new BO.Result(true, string.Format("Nelze načíst materiál s kódem: {0}.", c.p45MaterialCode));
                }
                ret.p45MaterialCode = c.p45MaterialCode;                
                ret.p45MaterialBatch = c.p45MaterialBatch;                
                ret.p19ID = recP19.pid;
            }
            var recP27 = _f.p27MszUnitBL.Load(recP41.p27ID);
            var recP18 = _f.p18OperCodeBL.LoadByCode(c.p45OperCode, recP27.p25ID_HW, 0);
            if (recP18 == null)
            {
                return new BO.Result(true, string.Format("Nelze načíst operaci s kódem: {0}.", c.p45OperCode));
            }
            ret.p18ID = recP18.pid;
            ret.p45Name = recP18.p18Name;
            ret.p45OperCode = c.p45OperCode;
            
            ret.p45MaterialUnitsCount = c.p45MaterialUnitsCount;
            ret.p45TotalDurationOperMin = c.p45TotalDurationOperMin;
            ret.p45Start = c.p45Start;
            ret.p45End = c.p45End;
            ret.p45OperStatus = c.p45OperStatus;
            ret.p45OperNum = c.p45OperNum;
            ret.p45OperParam = c.p45OperParam;
            ret.p45RowNum = c.p45RowNum;

            ret.p45Operator = c.p45Operator;

            int intP45ID = _f.p45TaskOperRealBL.Save(ret);
            
            if (intP45ID > 0)
            {
                return new BO.Result(false, "Uloženo");
            }
            else
            {
                string strErrs = string.Join(" ** ", _f.CurrentUser.Messages4Notify.Select(p => p.Value));
                return new BO.Result(true, strErrs);
            }

            
        }

        [HttpGet]        
        [Route("receptura_hlavicka_save")]
        public BO.Result receptura_hlavicka_save(string p13Code,string p13Name,string p25Code,string p13Memo)
        {
            if (string.IsNullOrEmpty(p13Code) == true || string.IsNullOrEmpty(p25Code) == true)
            {
                return new BO.Result(true, "Na vstupu chybí kód receptury nebo kód typu zařízení.");
            }
            var recP25 = _f.p25MszTypeBL.LoadByCode(p25Code, 0);            
            if (recP25 == null)
            {
                return new BO.Result(true, string.Format("Nelze načíst typ zařízení s kódem: {0}.",p25Code));
            }            
            var recP13 = _f.p13MasterTpvBL.LoadByCode(p13Code, 0);
            if (recP13 == null)
            {
                recP13 = new BO.p13MasterTpv() { p13Code = p13Code };               
            }
            recP13.p13Name = p13Name;
            recP13.p25ID = recP25.pid;
            recP13.p13Memo = p13Memo;

            int intP13ID = _f.p13MasterTpvBL.Save(recP13, 0);

            if (intP13ID > 0)
            {
                return new BO.Result(false, "Uloženo");
            }
            else
            {
                string strErrs = string.Join(" ** ", _f.CurrentUser.Messages4Notify.Select(p => p.Value));
                return new BO.Result(true, strErrs);
            }
        }

        [HttpGet]
        [Route("receptura_operace_save")]
        public BO.Result receptura_operace_save(string p13Code,string p18Code, string p19Code, double p14UnitsCount,double p14DurationPreOper, double p14DurationOper,double p14DurationPostOper,int p14OperNum, double p14OperParam, int p14RowNum)
        {
            if (string.IsNullOrEmpty(p13Code) == true || string.IsNullOrEmpty(p18Code) == true)
            {
                return new BO.Result(true, "Na vstupu je povinný kód receptury [p13Code] a kód operace [p18Code].");
            }
            if (p14RowNum <= 0 || p14OperNum<=0)
            {
                return new BO.Result(true, "[p14RowNum] a [p14OperNum] musí být kladné a nenulové číslo.");
            }
            var recP13 = _f.p13MasterTpvBL.LoadByCode(p13Code, 0);
            if (recP13 == null)
            {
                return new BO.Result(true, string.Format("Nelze načíst recepturu s kódem: {0}.", p13Code));
            }
            var recP18 = _f.p18OperCodeBL.LoadByCode(p18Code,recP13.p25ID, 0);
            if (recP18 == null)
            {
                return new BO.Result(true, string.Format("Nelze načíst šablonu operace s kódem: {0}.", p18Code));
            }            
            BO.p19Material recP19 = null;
            if (string.IsNullOrEmpty(p19Code) == false)
            {
                recP19 = _f.p19MaterialBL.LoadByCode(p19Code, 0);
                if (recP19 == null)
                {
                    return new BO.Result(true, string.Format("Nelze načíst surovinu s kódem: {0}.", p19Code));
                }
            }

            var mq = new BO.myQuery("p14MasterOper");
            mq.p13id = recP13.pid;
            var lisP14 = _f.p14MasterOperBL.GetList(mq);

            var recP14 = new BO.p14MasterOper() { p14RowNum = p14RowNum,p13ID=recP13.pid };
            if (lisP14.Where(p => p.p14RowNum == p14RowNum).Count() > 0)
            {
                recP14 = lisP14.Where(p => p.p14RowNum == p14RowNum).First();
            }
            recP14.p14OperNum = p14OperNum;
            recP14.p18ID = recP18.pid;
            recP14.p14UnitsCount = p14UnitsCount;
            recP14.p14OperParam = p14OperParam;
            recP14.p14DurationOper = p14DurationOper;
            recP14.p14DurationPreOper = p14DurationPreOper;
            recP14.p14DurationPostOper = p14DurationPostOper;

            if (recP19 != null)
            {
                recP14.p19ID = recP19.pid;
            }

            int intP14ID = _f.p14MasterOperBL.Save(recP14);

            if (intP14ID > 0)
            {
                return new BO.Result(false, "Uloženo");
            }
            else
            {
                string strErrs = string.Join(" ** ", _f.CurrentUser.Messages4Notify.Select(p => p.Value));
                return new BO.Result(true, strErrs);
            }
        }


        [HttpGet]
        [Route("master_produkt_save")]
        public BO.Result master_produkt_save(string p10Code, string p13Code,string p10Name, string p20Code_MJ,string p20Code_VJ,int p10TypeFlag,double p10RecalcUnit2Kg,string p10Memo,string p10PackagingCode)
        {
            if (string.IsNullOrEmpty(p10Code) == true || string.IsNullOrEmpty(p10Name) == true || string.IsNullOrEmpty(p20Code_MJ)==true || string.IsNullOrEmpty(p20Code_VJ) == true || p10RecalcUnit2Kg<=0)
            {
                return new BO.Result(true, "Na vstupu je povinné: kód produktu [p10Code], název produktu [p10Name], MJ [p20Code_MJ], VJ [p20Code_VJ], typ produktu [p10TypeFlag], přepočet MJ na VJ [p10RecalcUnit2Kg].");
            }
            if (p10TypeFlag<=0 || p10TypeFlag > 3)
            {
                return new BO.Result(true, "Hodnota [p10TypeFlag] může být 1, 2, 3.");
            }
            var recP20MJ = _f.p20UnitBL.LoadByCode(p20Code_MJ, 0);
            if (recP20MJ == null)
            {
                return new BO.Result(true, string.Format("Nelze načíst MJ s kódem: {0}.", p20Code_MJ));
            }
            var recP20VJ = _f.p20UnitBL.LoadByCode(p20Code_VJ, 0);
            if (recP20VJ == null)
            {
                return new BO.Result(true, string.Format("Nelze načíst VJ s kódem: {0}.", p20Code_VJ));
            }
            BO.p13MasterTpv recP13 = null;
            if (string.IsNullOrEmpty(p13Code) == false)
            {
                recP13 = _f.p13MasterTpvBL.LoadByCode(p13Code, 0);
                if (recP13 == null)
                {
                    return new BO.Result(true, string.Format("Nelze načíst recepturu s kódem: {0}.", p13Code));
                }
            }

            var recP10 = _f.p10MasterProductBL.LoadByCode(p10Code,0);
            if (recP10 == null)
            {
                recP10 = new BO.p10MasterProduct() { p10Code = p10Code };
            }
            if (recP13 != null)
            {
                recP10.p13ID = recP13.pid;
            }
            recP10.p10Name = p10Name;
            recP10.p10Memo = p10Memo;
            recP10.p10TypeFlag =(BO.ProductTypeEnum) p10TypeFlag;
            recP10.p10RecalcUnit2Kg = p10RecalcUnit2Kg;
            recP10.p20ID = recP20MJ.pid;
            recP10.p20ID_Pro = recP20VJ.pid;
            recP10.p10PackagingCode = p10PackagingCode;

            int intP10ID = _f.p10MasterProductBL.Save(recP10);

            if (intP10ID > 0)
            {
                return new BO.Result(false, "Uloženo");
            }
            else
            {
                string strErrs = string.Join(" ** ", _f.CurrentUser.Messages4Notify.Select(p => p.Value));
                return new BO.Result(true, strErrs);
            }
        }


        [HttpGet]
        [Route("surovina_save")]
        public BO.Result surovina_save(string p19Code, string p19Name, string p20Code, int p19TypeFlag, string p19Memo, string p19Supplier, string p19Intrastat, string p19NameAlias)
        {
            if (string.IsNullOrEmpty(p19Code) == true || string.IsNullOrEmpty(p19Name) == true || string.IsNullOrEmpty(p20Code) == true || p19TypeFlag <= 0)
            {
                return new BO.Result(true, "Na vstupu je povinné: kód suroviny [p19Code], název suroviny [p19Name], MJ [p20Code], typ suroviny [p19TypeFlag].");
            }
            if (p19TypeFlag <= 0 || p19TypeFlag > 3)
            {
                return new BO.Result(true, "Hodnota [p19TypeFlag] může být 1, 2, 3.");
            }
            var recP20 = _f.p20UnitBL.LoadByCode(p20Code, 0);
            if (recP20 == null)
            {
                return new BO.Result(true, string.Format("Nelze načíst MJ s kódem: {0}.", p20Code));
            }                        

            var recP19 = _f.p19MaterialBL.LoadByCode(p19Code, 0);
            if (recP19 == null)
            {
                recP19 = new BO.p19Material() { p19Code = p19Code };
            }

            recP19.p19Name = p19Name;
            recP19.p19Memo = p19Memo;
            recP19.p19TypeFlag = (BO.p19TypeFlagEnum)p19TypeFlag;
            recP19.p19Supplier = p19Supplier;
            recP19.p20ID = recP20.pid;
            recP19.p19Intrastat = p19Intrastat;
            recP19.p19NameAlias = p19NameAlias;

            int intP19ID = _f.p19MaterialBL.Save(recP19);

            if (intP19ID > 0)
            {
                return new BO.Result(false, "Uloženo");
            }
            else
            {
                string strErrs = string.Join(" ** ", _f.CurrentUser.Messages4Notify.Select(p => p.Value));
                return new BO.Result(true, strErrs);
            }
        }

        [HttpGet]
        [Route("zaznam_exist")]
        public BO.Result zaznam_exist(string entity_prefix, string record_code)
        {
            if (string.IsNullOrEmpty(entity_prefix) == true || string.IsNullOrEmpty(record_code) == true)
            {
                return new BO.Result(true, "Na vstupu chybí [entity_prefix] nebo [record_code].");
            }
            var ret = new BO.Result(false) { Flag = BO.ResultEnum.InfoOnly };
            ret.Message = string.Format("Záznam s kódem '{0}' neexistuje.", record_code);
            
            switch (entity_prefix)
            {
                case "p41":
                    if (_f.p41TaskBL.LoadByCode(record_code, 0) != null)
                    {
                        ret.pid = _f.p41TaskBL.LoadByCode(record_code, 0).pid;
                        ret.Message = "Záznam existuje, pid: " + ret.pid.ToString();
                    }
                    return ret;
                case "p10":
                    if (_f.p10MasterProductBL.LoadByCode(record_code, 0) != null)
                    {
                        ret.pid = _f.p10MasterProductBL.LoadByCode(record_code, 0).pid;
                        ret.Message = "Záznam existuje, pid: " + ret.pid.ToString();
                    }
                    return ret;
                case "p13":
                    if (_f.p13MasterTpvBL.LoadByCode(record_code, 0) != null)
                    {
                        ret.pid = _f.p13MasterTpvBL.LoadByCode(record_code, 0).pid;
                        ret.Message = "Záznam existuje, pid: " + ret.pid.ToString();
                    }
                    return ret;
                case "p19":
                    if (_f.p19MaterialBL.LoadByCode(record_code, 0) != null)
                    {
                        ret.pid = _f.p19MaterialBL.LoadByCode(record_code, 0).pid;
                        ret.Message = "Záznam existuje, pid: " + ret.pid.ToString();
                    }
                    return ret;
                case "p20":
                    if (_f.p20UnitBL.LoadByCode(record_code, 0) != null)
                    {
                        ret.pid = _f.p20UnitBL.LoadByCode(record_code, 0).pid;
                        ret.Message = "Záznam existuje, pid: " + ret.pid.ToString();
                    }
                    return ret;
                case "p25":
                    if (_f.p25MszTypeBL.LoadByCode(record_code, 0) != null)
                    {
                        ret.pid = _f.p25MszTypeBL.LoadByCode(record_code, 0).pid;
                        ret.Message = "Záznam existuje, pid: " + ret.pid.ToString();
                    }
                    return ret;
                case "p27":
                    if (_f.p27MszUnitBL.LoadByCode(record_code, 0) != null)
                    {
                        ret.pid = _f.p27MszUnitBL.LoadByCode(record_code, 0).pid;
                        ret.Message = "Záznam existuje, pid: " + ret.pid.ToString();
                    }
                    return ret;
                
                default:
                    return new BO.Result(true, string.Format("Zadaný prefix '{0}' není v této metodě podporován.", entity_prefix));
            }
            
        }

    }
}