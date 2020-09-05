using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using ClosedXML.Excel;


namespace UI.Controllers
{
    public class p51Controller : BaseController
    {
        public IActionResult Import()
        {
            var v = new p52ImportViewModel() { Guid = BO.BAS.GetGuid() };
            return View(v);
        }

        [HttpPost]
        public IActionResult Import(p52ImportViewModel v)
        {
            if (ModelState.IsValid)
            {
                var lisO27 = BO.BASFILE.GetUploadedFiles(Factory.App.TempFolder, v.Guid);
                if (lisO27.Count == 0)
                {
                    this.AddMessage("Na vstupu chybí XLS soubor.");
                    return View(v);
                }
                foreach (var c in lisO27)
                {
                    v.Protokol=Handle_Import(c.FullPath,v.Guid);
                }
                
                
            }
            return View(v);
        }

        private string Handle_Import(string strFullPath,string strGuid)
        {
            using (var workbook = new XLWorkbook(strFullPath))
            {
                var worksheet = workbook.Worksheets.First();

                for (int row = 2; row < 10000; row++)
                {
                    if (worksheet.Cell(row, 1).Value != null && worksheet.Cell(row, 1).Value.ToString() != "")
                    {
                        string strP51Code = worksheet.Cell(row, 1).Value.ToString();
                        string strP11Code = worksheet.Cell(row, 2).Value.ToString();
                        double dblP52UnitsCount = Convert.ToDouble(worksheet.Cell(row, 3).Value);
                        DateTime? datP52DateNeeded = null;
                        try
                        {
                            datP52DateNeeded=Convert.ToDateTime(worksheet.Cell(row, 4).Value);
                        }
                        catch
                        {

                        }

                        string strFce= worksheet.Cell(row, 5).Value.ToString();
                        if (string.IsNullOrEmpty(strFce) == true) strFce = "A";

                        var recP51 = Factory.p51OrderBL.LoadByCode(strP51Code, 0);
                        var recP11 = Factory.p11ClientProductBL.LoadByCode(strP11Code, 0);
                        if (recP51 != null && recP11 != null && dblP52UnitsCount > 0)
                        {
                            var c = new BO.p52OrderItem() { p11ID = recP11.pid, p51ID = recP51.pid, p52UnitsCount = dblP52UnitsCount,p52DateNeeded=datP52DateNeeded };
                            if (strFce == "U")
                            {
                                var lisP52 = Factory.p52OrderItemBL.GetList(recP51.pid);
                                if (lisP52.Where(p => p.p11ID == recP11.pid).Count() > 0)
                                {
                                    c = lisP52.Where(p => p.p11ID == recP11.pid).First();
                                    c.p52UnitsCount = dblP52UnitsCount;
                                }
                            }
                            var intP52ID = Factory.p52OrderItemBL.Save(c);
                            if (intP52ID > 0)
                            {
                                var recP52 = Factory.p52OrderItemBL.Load(intP52ID);
                                
                                worksheet.Cell(row, 5).Value = recP52.p52Code;
                            }


                        }
                    }
                }
                worksheet.Cell(1, 5).Value = "Naimportovaný kód";
                workbook.SaveAs(Factory.App.TempFolder + "\\import_vysledek"+strGuid+".xlsx");
                this.AddMessage("Import dokončen.","info");
                return "import_vysledek" + strGuid + ".xlsx";
            }
        }
        public IActionResult Index(int pid)
        {
            var v = new Models.p51PreviewViewModel();
            v.Rec = Factory.p51OrderBL.Load(pid);
            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                var tg = Factory.o51TagBL.GetTagging("p51", pid);

                v.OrderItems = Factory.p52OrderItemBL.GetList(pid);
                return View(v);
            }

        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (Factory.CurrentUser.j03EnvironmentFlag == 1)
            {
                return this.StopPageClientPageOnly(true);
            }
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.p51RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.p51OrderBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                var tg = Factory.o51TagBL.GetTagging("p51", pid);
                v.TagPids = tg.TagPids;
                v.TagNames = tg.TagNames;
                v.TagHtml = tg.TagHtml;

            }
            else
            {
                v.Rec = new BO.p51Order();
                v.Rec.entity = "p51";
                v.Rec.p51Code = Factory.CBL.EstimateRecordCode("p51");
                v.Rec.p51Date = DateTime.Now;
                v.Rec.p51DateDelivery = DateTime.Today.AddDays(10).AddSeconds(-1);
                //v.NewItems = new List<BO.p52OrderItem>();
                //v.NewItems.Add(new BO.p52OrderItem());
            }

            RefreshState(v);


            if (isclone)
            {
                v.NewItems = new List<BO.p52OrderItem>();
                foreach (var c in Factory.p52OrderItemBL.GetList(v.Rec.pid))
                {
                    v.NewItems.Add(new BO.p52OrderItem() { p11ID = c.p11ID, p11Name = c.p11Name, p52UnitsCount = c.p52UnitsCount });
                }
                v.Toolbar.MakeClone();
                v.Rec.p51Code = Factory.CBL.EstimateRecordCode("p51");
            }



            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p51RecordViewModel v, string rec_oper)
        {
            if (rec_oper == "newitem")
            {
                if (v.NewItems == null) v.NewItems = new List<BO.p52OrderItem>();
                v.NewItems.Add(new BO.p52OrderItem() { p51ID = v.Rec.p51ID });

                v.Toolbar = new MyToolbarViewModel(v.Rec);
                return View(v);
            }



            if (ModelState.IsValid)
            {
                BO.p51Order c = new BO.p51Order();
                if (v.Rec.pid > 0) c = Factory.p51OrderBL.Load(v.Rec.pid);

                c.p51IsDraft = v.Rec.p51IsDraft;
                c.p51Code = v.Rec.p51Code;
                c.p51Name = v.Rec.p51Name;
                c.p51Memo = v.Rec.p51Memo;
                c.b02ID = v.Rec.b02ID;
                c.p28ID = v.Rec.p28ID;
                c.j02ID_Owner = v.Rec.j02ID_Owner;
                c.p51CodeByClient = v.Rec.p51CodeByClient;


                c.p51Date = v.Rec.p51Date;
                c.p51DateDelivery = v.Rec.p51DateDelivery;
                c.p51DateDeliveryConfirmed = v.Rec.p51DateDeliveryConfirmed;

                v.Rec.pid = Factory.p51OrderBL.Save(c, v.NewItems);
                if (v.Rec.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("p51", v.Rec.pid, v.TagPids);
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }


            }

            RefreshState(v);

            this.Notify_RecNotSaved();
            return View(v);

        }

        private void RefreshState(p51RecordViewModel v)
        {

            v.Toolbar = new MyToolbarViewModel(v.Rec);
        }





    }
}