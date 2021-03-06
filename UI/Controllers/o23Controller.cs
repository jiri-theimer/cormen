﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using UI.Models;


namespace UI.Controllers
{
    
    public class o23Controller : BaseController
        {
        private BL.RunningApp _app;
        public o23Controller(BL.RunningApp app)
        {
            _app = app;
        }
        public IActionResult Index(int pid)
        {            
            var v = new Models.o23PreviewViewModel();
            v.Rec = Factory.o23DocBL.Load(pid);
            if (v.Rec == null) return RecNotFound(v);
            var tg = Factory.o51TagBL.GetTagging("o23", pid);
            v.Rec.TagHtml = tg.TagHtml;
            v.lisO27 = Factory.o23DocBL.GetListO27(pid);
            return View(v);
            
        }
        public IActionResult Record(int pid, bool isclone,string recprefix,int recpid)
        {
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.o23RecordViewModel();
            v.Guid = BO.BAS.GetGuid();
            if (pid > 0)
            {
                v.Rec = Factory.o23DocBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                if (!this.TestIfRecordEditable(v.Rec.j02ID_Owner))
                {
                    return this.StopPageEdit(true);
                }
                var tg = Factory.o51TagBL.GetTagging("o23", pid);
                v.TagPids = tg.TagPids;
                v.TagNames = tg.TagNames;
                v.TagHtml = tg.TagHtml;
            }
            else
            {
                v.Rec = new BO.o23Doc();
                v.Rec.entity = "o23";
                if (string.IsNullOrEmpty(recprefix))
                {
                    v.Rec.o23Entity = "p28Company";
                }
                else
                {
                    v.Rec.o23Entity = BL.TheEntities.ByPrefix(recprefix).TableName;
                    if (recpid > 0)
                    {
                        v.Rec.o23RecordPid = recpid;
                        v.Rec.RecordPidAlias = Factory.CBL.GetRecordAlias(v.Rec.o23Entity, recpid);
                    }
                }
                
                v.Rec.o23Code = Factory.CBL.EstimateRecordCode("o23");
            }
          
            
            
            RefreshState(v);

            if (isclone) {
                v.Toolbar.MakeClone();
                v.Rec.o23Code = Factory.CBL.EstimateRecordCode("o23");
            }

            if (pid>0 && !isclone) v.lisO27 = Factory.o23DocBL.GetListO27(pid);

            return View(v);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.o23RecordViewModel v,bool change_entity_only)
        {
            if (change_entity_only)
            {
                
                v.Rec.o23RecordPid = 0;
                v.Rec.RecordPidAlias = "";
               
                RefreshState(v);

                return View(v);
            }
            if (ModelState.IsValid)
            {
                BO.o23Doc c = new BO.o23Doc();
                if (v.Rec.pid > 0) c = Factory.o23DocBL.Load(v.Rec.pid);

                c.o23Code = v.Rec.o23Code;
                c.o23Name = v.Rec.o23Name;
                c.o23Entity = v.Rec.o23Entity;
                c.o23RecordPid = v.Rec.o23RecordPid;
                
                c.o23Memo = v.Rec.o23Memo;
                c.o23Date = v.Rec.o23Date;
                c.b02ID = v.Rec.b02ID;
                
                c.j02ID_Owner = v.Rec.j02ID_Owner;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                string strTempDir = _app.TempFolder;
                string strUploadDir = _app.UploadFolder;

                List<BO.o27Attachment> lisO27 = BO.BASFILE.CopyTempFiles2Upload(_app.TempFolder, v.Guid, _app.UploadFolder);               

                v.Rec.pid = Factory.o23DocBL.Save(c,lisO27,BO.BAS.ConvertString2ListInt(v.o27IDs4Delete));
                if (v.Rec.pid > 0)
                {
                    Factory.o51TagBL.SaveTagging("o23", v.Rec.pid, v.TagPids);
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }

            }
           
            
            
            
            RefreshState(v);
            this.Notify_RecNotSaved();

            
            
            return View(v);
        }

        private void RefreshState(o23RecordViewModel v)
        {
            if (v.Rec.pid > 0) v.lisO27 = Factory.o23DocBL.GetListO27(v.Rec.pid);

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            
            
        }

        public IActionResult SingleUpload(string guid)
        {
            if (string.IsNullOrEmpty(guid) == true)
            {
                return this.StopPage(true,"guid missing");
            }
            var v = new FileUploadSingleViewModel() { Guid = guid };
            var lisO27 = BO.BASFILE.GetUploadedFiles(Factory.App.TempFolder, v.Guid);
            if (lisO27.Count() > 0)
            {
                v.TempFileName = lisO27.First().o27ArchiveFileName;
                v.OrigFileName = lisO27.First().o27Name;
            }
           
            return View(v);
        }
        [HttpPost]
        public async Task<IActionResult> SingleUpload(FileUploadSingleViewModel v, List<IFormFile> files)
        {

            var tempDir = Factory.App.TempFolder + "\\";

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    v.OrigFileName = formFile.FileName;
                    v.TempFileName = v.Guid + "_" + formFile.FileName;
                    var strTempFullPath = tempDir + v.TempFileName;


                    System.IO.File.WriteAllText(tempDir + v.Guid + ".infox", formFile.ContentType + "|" + formFile.Length.ToString() + "|" + formFile.FileName + "|" + v.Guid + "_" + formFile.FileName + "|" + v.Guid + "|0|||0");


                    using (var stream = new FileStream(strTempFullPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            
            return View(v);

        }


        public IActionResult DoUpload(string guid,string prefix)
        {
            ViewBag.prefix = prefix;
            ViewBag.guid = guid;
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> DoUpload(List<IFormFile> files,string guid,string prefix)
        {
            ViewBag.guid = guid;
            ViewBag.prefix = prefix;

            long size = files.Sum(f => f.Length);

            var tempfiles = new List<string>();
            var tempDir = _app.TempFolder + "\\";
            
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // full path to file in temp location
                    //var strTempFullPath = Path.GetTempFileName();
                    
                    var strTempFullPath = tempDir + guid+"_"+formFile.FileName;
                    tempfiles.Add(strTempFullPath);
                    System.IO.File.AppendAllText(tempDir + guid + "_"+ formFile.FileName+ ".infox", formFile.ContentType+"|"+ formFile.Length.ToString()+"|"+formFile.FileName+"|"+ guid + "_" + formFile.FileName);
                    

                    using (var stream = new FileStream(strTempFullPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            return View();
            //return Ok(new { count = files.Count, size, tempfiles });
        }

        [HttpGet]
        public ActionResult FileDownloadInline(string guid)
        {
            var c = Factory.o23DocBL.LoadO27ByGuid(guid);
            string strUploadDir = _app.UploadFolder;

            string fullPath = strUploadDir + "\\" + c.o27ArchiveFolder + "\\" + c.o27ArchiveFileName;
            if (System.IO.File.Exists(fullPath))
            {
                Response.Headers["Content-Disposition"] = string.Format("inline; filename={0}", c.o27Name);
                var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(fullPath), c.o27ContentType);

                return fileContentResult;
            }
            else
            {
                return FileDownloadNotFound(c);
            }

        }

        public ActionResult FileDownloadReport(string filename)
        {
            string fullPath = Factory.App.ReportFolder + "\\" + filename;
            if (System.IO.File.Exists(fullPath))
            {
                return File(System.IO.File.ReadAllBytes(fullPath), "application/octet-stream", filename);
            }
            else
            {
                return FileDownloadNotFound(new BO.o27Attachment());
            }
        }


        [HttpGet]
        public ActionResult FileDownload(string guid)
        {
            var c = Factory.o23DocBL.LoadO27ByGuid(guid);
            string strUploadDir = _app.UploadFolder;
            Response.Headers["Content-Type"] = c.o27ContentType;
            Response.Headers["Content-Length"] = c.o27FileSize.ToString();

            string fullPath = strUploadDir + "\\" + c.o27ArchiveFolder + "\\" + c.o27ArchiveFileName;

            if (System.IO.File.Exists(fullPath))
            {
                return File(System.IO.File.ReadAllBytes(fullPath), c.o27ContentType, c.o27Name);
            }
            else
            {
                return FileDownloadNotFound(c);
            }
        }

       
        public ActionResult FileDownloadNotFound(BO.o27Attachment c)
        {            
            var fullPath = _app.TempFolder + "\\notfound.txt";
            System.IO.File.WriteAllText(fullPath, string.Format("Soubor [{0}] na serveru [??????\\{1}] neexistuje!", c.o27Name, c.o27ArchiveFolder));
            Response.Headers["Content-Disposition"] = string.Format("inline; filename={0}", "notfound.txt");
            var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(fullPath), "text/plain");
            return fileContentResult;

        }

        public FileResult FileDownloadTempFile(string tempfilename,string guid=null,string contenttype=null)
        {
            if (System.IO.File.Exists(_app.TempFolder + "\\" + tempfilename)==false)
            {
                return null;
            }

            string destfilename = tempfilename;
            if (guid != null)
            {
                destfilename = tempfilename.Replace(guid + "_", "");
            }

            if (contenttype == null)
            {                
                byte[] fileBytes = System.IO.File.ReadAllBytes(_app.TempFolder + "\\" + tempfilename);
                Response.Headers["Content-Type"] = "application/octet-stream";
                Response.Headers["Content-Length"] = fileBytes.Length.ToString();
                return File(fileBytes, "application/octet-stream", destfilename);
            }
            else
            {
                Response.Headers["Content-Disposition"] = string.Format("inline; filename={0}", destfilename);
                var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(_app.TempFolder + "\\" + tempfilename), contenttype);
                Response.Headers["Content-Length"] = fileContentResult.FileContents.Length.ToString();
                return fileContentResult;
            }

        }


    }
}