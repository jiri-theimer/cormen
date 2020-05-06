using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p12Controller : BaseController
    {
        public IActionResult Index(int pid)
        {
            var v = new Models.p12PreviewViewModel();


            v.Rec = Factory.p12ClientTpvBL.Load(pid);

            if (v.Rec == null)
            {
                return RecNotFound(v);
            }
            else
            {
                return View(v);
            }

        }
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new Models.p12RecordViewModel();

            if (pid > 0)
            {
                v.Rec = Factory.p12ClientTpvBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p12ClientTpv();
                v.Rec.entity = "p12";
            }

            v.lisTemp = new List<BO.p85Tempbox>();

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone)
            {
                v.Toolbar.MakeClone();
                v.Guid = BO.BAS.GetGuid();
                PrepareTempTable(pid, v.Guid, true);
            }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p12RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.p12ClientTpv c = new BO.p12ClientTpv();
                if (v.Rec.pid > 0) c = Factory.p12ClientTpvBL.Load(v.Rec.pid);

                c.p12Code = v.Rec.p12Code;
                c.p12Name = v.Rec.p12Name;
                c.p12Memo = v.Rec.p12Memo;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                if (string.IsNullOrEmpty(v.Guid)) { v.Guid = BO.BAS.GetGuid(); };

                v.Rec.pid = Factory.p12ClientTpvBL.Save(c, v.Guid);
                if (v.Rec.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }


            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            this.Notify_RecNotSaved();
            return View(v);
        }


        private void PrepareTempTable(int intp12ID, string guid, bool isclone)
        {
            if (intp12ID == 0) return;
            var mq = new BO.myQuery("p15ClientOper");
            mq.p12id = intp12ID;
            mq.explicit_orderby = "a.p15RowNum";
            var lis = Factory.p15ClientOperBL.GetList(mq);
            int x = 1000;
            foreach (var c in lis)
            {
                var rec = new BO.p85Tempbox() { p85GUID = guid, p85Prefix = "p15" };
                if (isclone == false)
                {
                    rec.p85RecordPid = c.pid;
                    rec.p85OtherKey1 = c.p12ID;
                }

                rec.p85FreeNumber01 = c.p15RowNum;
                rec.p85FreeText01 = c.p15OperNum;
                rec.p85FreeText02 = c.p15OperCode;
                rec.p85FreeText03 = c.p15Name;
                rec.p85FreeNumber02 = c.p15OperParam;

                rec.p85FreeText04 = c.p15MaterialCode;
                rec.p85FreeText05 = c.p15MaterialName;

                rec.p85FreeNumber03 = c.p15UnitsCount;
                rec.p85FreeNumber04 = c.p15DurationPreOper;
                rec.p85FreeNumber05 = c.p15DurationOper;
                rec.p85FreeNumber06 = c.p15DurationPostOper;

                rec.p85OtherKey5 = x;
                Factory.p85TempboxBL.Save(rec);
                x += 1000;
            }

        }

        public BO.p85Tempbox InsertTempRow(string guid)
        {
            var lis = Factory.p85TempboxBL.GetList(guid, false, "p15").OrderBy(p => p.p85OtherKey5);
            var intPoradi = 1000;
            if (lis.Count() > 0)
            {
                intPoradi = 1000 + lis.Last(p => p.pid > 0).p85OtherKey5;
            }
            var rec = new BO.p85Tempbox() { p85GUID = guid, p85Prefix = "p15", p85OtherKey5 = intPoradi };
            var x = Factory.p85TempboxBL.Save(rec);
            return Factory.p85TempboxBL.Load(x);
        }
        public BO.p85Tempbox DeleteTempRow(string guid, int p85id)
        {
            return Factory.p85TempboxBL.VirtualDelete(p85id);


        }
        public BO.p85Tempbox MoveTempRow(string guid, int p85id, string direction)
        {
            var lis = Factory.p85TempboxBL.GetList(guid, false, "p15").OrderBy(p => p.p85OtherKey5);

            var c = lis.First(p => p.pid == p85id);
            //var c = Factory.p85TempboxBL.Load(p85id);

            if (direction == "up")
            {
                var cc = lis.Last(p => p.p85OtherKey5 < c.p85OtherKey5);
                c.p85OtherKey5 = cc.p85OtherKey5 - 100;
            }
            if (direction == "down")
            {
                var cc = lis.First(p => p.p85OtherKey5 > c.p85OtherKey5);
                c.p85OtherKey5 = cc.p85OtherKey5 + 100;
            }
            Factory.p85TempboxBL.Save(c);
            lis = Factory.p85TempboxBL.GetList(guid, false, "p15").OrderBy(p => p.p85OtherKey5);
            int x = 1000;

            foreach (var cc in lis)
            {
                cc.p85OtherKey5 = x;
                Factory.p85TempboxBL.Save(cc);
                x += 1000;
            }



            return c;


        }
        public string getHtmlTempBody(string guid, int p12id)
        {
            var lis = Factory.p85TempboxBL.GetList(guid, true, "p15");
            if (lis.Count() == 0)
            {
                PrepareTempTable(p12id, guid, false);
                lis = Factory.p85TempboxBL.GetList(guid, false, "p15");
            }
            lis = lis.Where(p => p.p85IsDeleted == false).OrderBy(p => p.p85OtherKey5);
            int x = 1;
            foreach (var c in lis)
            {
                c.p85FreeNumber01 = x;
                x += 1;
            }

            var s = new System.Text.StringBuilder();
            int xx = 0;
            foreach (var c in lis)
            {
                s.Append(string.Format("<tr data-p15id='{0}' data-p85id='{1}'>", c.p85RecordPid, c.pid));

                s.Append(string.Format("<td>{0}</td>", c.p85FreeNumber01)); //p15RowNum
                s.Append(string.Format("<td contenteditable='true' data-field='p85FreeText01'>{0}</td>", c.p85FreeText01)); //p15OperNum
                s.Append(string.Format("<td contenteditable='true' data-field='p85FreeText02'>{0}</td>", c.p85FreeText02)); //p15OperCode
                s.Append(string.Format("<td contenteditable='true' data-field='p85FreeText03'>{0}</td>", c.p85FreeText03)); //p15Name
                s.Append(string.Format("<td data-type='number' contenteditable='true' data-field='p85FreeNumber02'>{0}</td>", BO.BAS.TestDouleAsDbKey(c.p85FreeNumber02))); //p15OperParam
                s.Append(string.Format("<td contenteditable='true' data-field='p85FreeText04'>{0}</td>", c.p85FreeText04)); //p15MaterialCode
                s.Append(string.Format("<td contenteditable='true' data-field='p85FreeText05'>{0}</td>", c.p85FreeText05)); //p15MaterialName
                s.Append(string.Format("<td data-type='number' contenteditable='true' data-field='p85FreeNumber03'>{0}</td>", BO.BAS.TestDouleAsDbKey(c.p85FreeNumber03))); //p15UnitsCount
                s.Append(string.Format("<td data-type='number' contenteditable='true' data-field='p85FreeNumber04'>{0}</td>", BO.BAS.TestDouleAsDbKey(c.p85FreeNumber04))); //p15DurationPreOper
                s.Append(string.Format("<td data-type='number' contenteditable='true' data-field='p85FreeNumber05'>{0}</td>", BO.BAS.TestDouleAsDbKey(c.p85FreeNumber05))); //p15DurationOper
                s.Append(string.Format("<td data-type='number' contenteditable='true' data-field='p85FreeNumber06'>{0}</td>", BO.BAS.TestDouleAsDbKey(c.p85FreeNumber06))); //p15DurationPostOper

                s.Append(string.Format("<td><button type='button' class='btn btn-danger btn-sm' title='Odstranit řádek' onclick='delete_row({0})'>&times;</button></td>", c.pid));
                s.Append("<td>");
                if (xx > 0)
                {
                    s.Append(string.Format("<button type='button' class='btn btn-primary btn-sm' title='Posunout nahoru' onclick='move_row({0},0)'>🡅</button>", c.pid));
                }
                s.Append("</td><td>");
                if (xx < lis.Count() - 1)
                {
                    s.Append(string.Format("<button type='button' class='btn btn-primary btn-sm' title='Posunout dolů' onclick='move_row({0},1)'>🡇</button>", c.pid));
                };
                s.Append("</td>");



                s.Append("</tr>");
                xx += 1;
            }


            return s.ToString();
        }
    }
}