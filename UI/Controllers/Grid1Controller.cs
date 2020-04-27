using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Text.Json;
using Newtonsoft.Json;

namespace UI.Controllers
{
    public partial class Grid1Controller : BaseController
    {
        //private BL.Factory _f;
        //public Grid1Controller(BL.Factory f)
        //{
        //    _f = f;
        //}

        public IActionResult Index()
        {
            return View();
        }

        public ActionResult GetGridData([DataSourceRequest]DataSourceRequest request)
        {
            
            var mq = new BO.myQuery("p28Company");
            var lis =this.Factory.p28CompanyBL.GetList(mq);


            


            var result = new DataSourceResult()
            {
                Data = lis,
                Total = lis.Count()
            };

            return Json(lis.ToDataSourceResult(request), new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });

            //return Json(result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });

            //return Json(z.ToDataSourceResult(request));
        }

        public ActionResult GetDataTable([DataSourceRequest]DataSourceRequest request)
        {
            
            var s = DateTime.Now.ToString();
            foreach(Kendo.Mvc.SortDescriptor c in request.Sorts)
            {
                s += "|| sort: " + c.Member;
                s += ",sort dir: "+c.SortDirection.ToString();
            }
            foreach (Kendo.Mvc.FilterDescriptor c in request.Filters)
            {
                s += "|| filter: " + c.Member;
                s += ",operator: " + c.Operator.ToString();
                s += ",value: " + c.ConvertedValue.ToString();
            }
           
            System.IO.File.AppendAllText("c:\\temp\\grid1.txt", s);
            System.IO.File.AppendAllLines("c:\\temp\\grid1.txt", new List<string>() { "Konec hlášení", "" }); ;
            var mq = new BO.myQuery("p28Company");
            var dt = this.Factory.gridBL.GetList(mq);
            var result = new DataSourceResult();

            //return new ContentResult() { Content = DataTableToJSONWithJSONNet(dt), ContentType = "application/json" };
            //return Json(dt.ToDataSourceResult(request), new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });

            return Json(dt.ToDataSourceResult(request), new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });




            //return Json(lis.ToDataSourceResult(request), new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });

            //return Json(result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = false });

            //return Json(z.ToDataSourceResult(request));
        }

        private string DataTableToJSONWithJSONNet(System.Data.DataTable dt)
        {

            return JsonConvert.SerializeObject(dt, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });




        }
    }
}