using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL;
using BO;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg.OpenPgp;
using UI.Models;

namespace UI.Controllers
{
    public class o51Controller : BaseController
    {
        ///štítky
        ///
        public IActionResult MultiSelect(string entity,string o51ids)
        {            
            var v = new TagsMultiSelect();
            v.Entity = entity;
            string prefix = v.Entity.Substring(0, 3);

            var mq = new BO.myQuery("o53TagGroup");
            var lisGroups = Factory.o53TagGroupBL.GetList(mq).Where(p =>p.o53IsMultiSelect==false && ( p.o53Entities == null || p.o53Entities.Contains(prefix))).ToList();
            v.SingleCombos = new List<SingleSelectCombo>();
            foreach (var group in lisGroups)
            {
                v.SingleCombos.Add(new SingleSelectCombo() { o53ID = group.pid, o53Name = group.o53Name });
            }
            
            mq = new BO.myQuery("o51Tag");
            mq.IsRecordValid = true;
            IEnumerable<BO.o51Tag> lis = Factory.o51TagBL.GetList(mq);
            v.ApplicableTags_Multi = lis.Where(p => p.o53IsMultiSelect==true && ( p.o53Entities == null || p.o53Entities.Contains(prefix)));
            v.ApplicableTags_Single = lis.Where(p => p.o53IsMultiSelect==false && (p.o53Entities == null || p.o53Entities.Contains(prefix)));

            

            v.CheckedO51IDs = BO.BAS.ConvertString2ListInt(o51ids);
            v.SelectedO51IDs = BO.BAS.ConvertString2ListInt(o51ids);

            //if (String.IsNullOrEmpty(o51ids) == false)
            //{                
            //    mq.pids = BO.BAS.ConvertString2ListInt(o51ids);
            //    v.SelectedTags = Factory.o51TagBL.GetList(mq);
            //}
            
            

            return View(v);
        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, false))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.o51RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.o51TagBL.Load(pid);
                
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                
            }
            else
            {
                v.Rec = new BO.o51Tag();
                v.Rec.o51Code = Factory.CBL.EstimateRecordCode("o51");
                v.Rec.entity = "o51";

            }
            RefreshState(ref v);
            if (isclone) { v.Toolbar.MakeClone(); v.Rec.o51Code = Factory.CBL.EstimateRecordCode("o51"); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.o51RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.o51Tag c = new BO.o51Tag();
                if (v.Rec.pid > 0) c = Factory.o51TagBL.Load(v.Rec.pid);
                c.o53ID = v.Rec.o53ID;
                c.o51Code = v.Rec.o51Code;
                c.o51Name = v.Rec.o51Name;
                c.o51Ordinary = v.Rec.o51Ordinary;
                c.o51IsColor = v.Rec.o51IsColor;
                c.o51BackColor = v.Rec.o51BackColor;
                c.o51ForeColor = v.Rec.o51ForeColor;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.o51TagBL.Save(c);
                if (v.Rec.pid > 0)
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }

            }

            RefreshState(ref v);
            this.Notify_RecNotSaved();
            return View(v);
        }


        public IActionResult Batch(int j72id, string pids)
        {
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.TagsBatch();
            RefreshState_Batch(ref v, j72id, pids);

            return View(v);
        }
        [HttpPost]
        public IActionResult Batch(Models.TagsBatch v,string oper)
        {            
            if (ModelState.IsValid)
            {
                List<int> o51ids = v.SelectedO51IDs.Where(p => p > 0).ToList();
                List<int> pids = BO.BAS.ConvertString2ListInt(v.Record_Pids);

                if (o51ids.Count > 0 || oper=="clear")
                {
                    string strO51IDs = string.Join(",", o51ids);
                    foreach (int pid in pids)
                    {
                        switch (oper)
                        {
                            case "replace":
                                Factory.o51TagBL.SaveTagging(v.Record_Entity, pid, strO51IDs);
                                break;
                            case "clear":
                                Factory.o51TagBL.SaveTagging(v.Record_Entity, pid, "");
                                break;
                            case "append":
                                var c = Factory.o51TagBL.GetTagging(v.Record_Entity, pid);
                                if (c.TagPids == null)
                                {
                                    c.TagPids = strO51IDs;
                                }
                                else
                                {
                                    c.TagPids += ","+strO51IDs;
                                }
                                Factory.o51TagBL.SaveTagging(v.Record_Entity, pid, c.TagPids);

                                break;
                            case "remove":
                                var d = Factory.o51TagBL.GetTagging(v.Record_Entity, pid);
                                if (d.TagPids != null)
                                {
                                    foreach(int o51id in o51ids)
                                    {
                                        d.TagPids = BO.BAS.RemoveValueFromDelimitedString(d.TagPids, o51id.ToString());
                                    }
                                    Factory.o51TagBL.SaveTagging(v.Record_Entity, pid, d.TagPids);
                                }
                                break;
                        }
                        
                    }
                    v.SetJavascript_CallOnLoad(v.j72ID);
                    return View(v);
                }
                else
                {
                    Factory.CurrentUser.AddMessage("Musíte zaškrtnout minimálně jeden štítek.");
                }
                
            }
            
            RefreshState_Batch(ref v, v.j72ID, v.Record_Pids);
           
            return View(v);
        }
        
        private void RefreshState_Batch(ref TagsBatch v,int j72id,string pids)
        {
            BO.j72TheGridState c = Factory.gridBL.LoadTheGridState(j72id);            
            v.j72ID = j72id;
            v.Record_Entity = c.j72Entity;
            v.Record_Pids = pids;
            string prefix = v.Record_Entity.Substring(0, 3);
            var mq = new BO.myQuery("o51Tag");
            mq.IsRecordValid = true;
            v.ApplicableTags = Factory.o51TagBL.GetList(mq).Where(p => p.o53Entities == null || p.o53Entities.Contains(prefix));     //.OrderBy(p => p.o53Ordinary).ThenBy(p=>p.o51Ordinary).ThenBy(p=>p.o51Name);
        }

        private void RefreshState(ref o51RecordViewModel v)
        {
            
            v.Toolbar = new MyToolbarViewModel(v.Rec);
        }

        
    }

    
}