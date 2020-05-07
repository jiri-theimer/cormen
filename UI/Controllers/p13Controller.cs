
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class p13Controller : BaseController
    {
        

        public IActionResult Index(int pid)
        {
            var v = new Models.p13PreviewViewModel();
           

            v.Rec = Factory.p13MasterTpvBL.Load(pid);     
           
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
            var v = new Models.p13RecordViewModel();
           
            if (pid > 0)
            {
                v.Rec = Factory.p13MasterTpvBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.p13MasterTpv();
                v.Rec.entity = "p13";
            }

            v.lisTemp = new List<BO.p85Tempbox>();

            v.Toolbar = new MyToolbarViewModel(v.Rec);            
            if (isclone) {
                v.Toolbar.MakeClone();
                v.Guid = BO.BAS.GetGuid();
                PrepareTempTable(pid,v.Guid,true);
            }
            var mq = new BO.myQuery("p14MasterOper");
            mq.p13id = v.Rec.pid;
            v.lisP14 = Factory.p14MasterOperBL.GetList(mq).ToList();
            for(var i = 0; i < v.lisP14.Count(); i++)
            {
                v.lisP14[i].TempRecGuid = BO.BAS.GetGuid();
                v.lisP14[i].TempRecDisplay = "table-row";
            }
            
            return View(v);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.p13RecordViewModel v,string rec_oper,string rec_guid)
        {
            if (rec_oper != null)
            {
                if (rec_oper == "add")
                {
                    v.lisP14.Add(new BO.p14MasterOper() {TempRecDisplay="table-row", TempRecGuid = BO.BAS.GetGuid() });
                }
                if (rec_oper == "postback")
                {
                    //pouze postback
                    v.lisP14 = v.lisP14.OrderBy(p => p.p14RowNum).ToList();

                }

                v.Toolbar = new MyToolbarViewModel(v.Rec);

                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.p13MasterTpv c = new BO.p13MasterTpv();
                if (v.Rec.pid > 0) c = Factory.p13MasterTpvBL.Load(v.Rec.pid);

                c.p13Code = v.Rec.p13Code;
                c.p13Name = v.Rec.p13Name;
                c.p13Memo = v.Rec.p13Memo;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                if (string.IsNullOrEmpty(v.Guid)) { v.Guid = BO.BAS.GetGuid(); };

                v.Rec.pid = Factory.p13MasterTpvBL.Save(c, v.lisP14);
                //v.Rec.pid = Factory.p13MasterTpvBL.Save(c,v.Guid);
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


        private void PrepareTempTable(int intP13ID,string guid,bool isclone)
        {
            if (intP13ID == 0) return;
            var mq = new BO.myQuery("p14");
            mq.p13id = intP13ID;
            mq.explicit_orderby = "a.p14RowNum";
            var lis = Factory.p14MasterOperBL.GetList(mq);
            int x = 1000;
            foreach (var c in lis)
            {
                var rec = new BO.p85Tempbox() { p85GUID = guid, p85Prefix = "p14" };
                if (isclone == false)
                {
                    rec.p85RecordPid = c.pid;
                    rec.p85OtherKey1 = c.p13ID;
                }
                
                rec.p85FreeNumber01 = c.p14RowNum;
                rec.p85FreeText01 = c.p14OperNum;
                rec.p85FreeText02 = c.p14OperCode;
                rec.p85FreeText03 = c.p14Name;
                rec.p85FreeNumber02 = c.p14OperParam;

                rec.p85FreeText04 = c.p14MaterialCode;
                rec.p85FreeText05 = c.p14MaterialName;
                                                
                rec.p85FreeNumber03 = c.p14UnitsCount;
                rec.p85FreeNumber04 = c.p14DurationPreOper;
                rec.p85FreeNumber05 = c.p14DurationOper;
                rec.p85FreeNumber06 = c.p14DurationPostOper;

                rec.p85OtherKey5 = x;
                Factory.p85TempboxBL.Save(rec);
                x += 1000;
            }
            
        }

        public BO.p85Tempbox InsertTempRow(string guid)
        {
            var lis = Factory.p85TempboxBL.GetList(guid,false,"p14").OrderBy(p => p.p85OtherKey5);
            var intPoradi = 1000;
            if (lis.Count() > 0)
            {
                intPoradi = 1000+lis.Last(p => p.pid > 0).p85OtherKey5;
            }
            var rec = new BO.p85Tempbox() { p85GUID = guid, p85Prefix = "p14", p85OtherKey5 = intPoradi };
            var x=Factory.p85TempboxBL.Save(rec);
            return Factory.p85TempboxBL.Load(x);
        }
        public BO.p85Tempbox DeleteTempRow(string guid, int p85id)
        {
            return Factory.p85TempboxBL.VirtualDelete(p85id);
            

        }
        public BO.p85Tempbox MoveTempRow(string guid,int p85id,string direction)
        {            
            var lis = Factory.p85TempboxBL.GetList(guid,false,"p14").OrderBy(p => p.p85OtherKey5);
            
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
            lis = Factory.p85TempboxBL.GetList(guid,false,"p14").OrderBy(p => p.p85OtherKey5);
            int x = 1000;
            
            foreach(var cc in lis)
            {                
                cc.p85OtherKey5 = x;
                Factory.p85TempboxBL.Save(cc);
                x += 1000;                
            }



            return c;


        }
        public string getHtmlTempBody(string guid, int p13id)
        {
            var lis = Factory.p85TempboxBL.GetList(guid,true,"p14");
            if (lis.Count() == 0)
            {
                PrepareTempTable(p13id, guid,false);
                lis = Factory.p85TempboxBL.GetList(guid,false,"p14");
            }
            lis = lis.Where(p=>p.p85IsDeleted==false).OrderBy(p => p.p85OtherKey5);
            int x = 1;
            foreach(var c in lis)
            {
                c.p85FreeNumber01 = x;
                x += 1;
            }
           
            var s = new System.Text.StringBuilder();            
            int xx = 0;
            foreach (var c in lis)
            {
                s.Append(string.Format("<tr data-p14id='{0}' data-p85id='{1}'>",c.p85RecordPid,c.pid));

                s.Append(string.Format("<td>{0}</td>", c.p85FreeNumber01)); //p14RowNum
                s.Append(string.Format("<td contenteditable='true' data-field='p85FreeText01'>{0}</td>", c.p85FreeText01)); //p14OperNum
                s.Append(string.Format("<td contenteditable='true' data-field='p85FreeText02'>{0}</td>", c.p85FreeText02)); //p14OperCode
                s.Append(string.Format("<td contenteditable='true' data-field='p85FreeText03'>{0}</td>", c.p85FreeText03)); //p14Name
                s.Append(string.Format("<td data-type='number' contenteditable='true' data-field='p85FreeNumber02'>{0}</td>", BO.BAS.TestDouleAsDbKey(c.p85FreeNumber02))); //p14OperParam
                s.Append(string.Format("<td contenteditable='true' data-field='p85FreeText04'>{0}</td>", c.p85FreeText04)); //p14MaterialCode
                s.Append(string.Format("<td contenteditable='true' data-field='p85FreeText05'>{0}</td>", c.p85FreeText05)); //p14MaterialName
                s.Append(string.Format("<td data-type='number' contenteditable='true' data-field='p85FreeNumber03'>{0}</td>", BO.BAS.TestDouleAsDbKey(c.p85FreeNumber03))); //p14UnitsCount
                s.Append(string.Format("<td data-type='number' contenteditable='true' data-field='p85FreeNumber04'>{0}</td>", BO.BAS.TestDouleAsDbKey(c.p85FreeNumber04))); //p14DurationPreOper
                s.Append(string.Format("<td data-type='number' contenteditable='true' data-field='p85FreeNumber05'>{0}</td>", BO.BAS.TestDouleAsDbKey(c.p85FreeNumber05))); //p14DurationOper
                s.Append(string.Format("<td data-type='number' contenteditable='true' data-field='p85FreeNumber06'>{0}</td>", BO.BAS.TestDouleAsDbKey(c.p85FreeNumber06))); //p14DurationPostOper

                s.Append(string.Format("<td><button type='button' class='btn btn-danger btn-sm' title='Odstranit řádek' onclick='delete_row({0})'>&times;</button></td>", c.pid));
                s.Append("<td>");
                if (xx > 0)
                {
                    s.Append(string.Format("<button type='button' class='btn btn-primary btn-sm' title='Posunout nahoru' onclick='move_row({0},0)'>🡅</button>", c.pid));
                }
                s.Append("</td><td>");
                if (xx < lis.Count()-1)
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