using System;
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
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new Models.o23RecordViewModel();
            v.Guid = BO.BAS.GetGuid();
            if (pid > 0)
            {
                v.Rec = Factory.o23DocBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.o23Doc();
                v.Rec.entity = "o23";
            }
            v.ComboB02ID = new MyComboViewModel("b02", v.Rec.b02ID.ToString(), v.Rec.b02Name, "cbxB02");
            v.ComboB02ID.Param1 = "o23";
            v.ComboO12ID = new MyComboViewModel("o12", v.Rec.o12ID.ToString(), v.Rec.o12Name, "cbxO12");
            v.ComboO12ID.Param1 = "o23";
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); }

            if (pid>0) v.lisO27 = Factory.o23DocBL.GetListO27(pid);

            return View(v);
        }

        

        


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.o23RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.o23Doc c = new BO.o23Doc();
                if (v.Rec.pid > 0) c = Factory.o23DocBL.Load(v.Rec.pid);

                c.o23Code = v.Rec.o23Code;
                c.o23Name = v.Rec.o23Name;
                c.o23Entity = v.Rec.o23Entity;
                c.o23Memo = v.Rec.o23Memo;
                c.o23Date = v.Rec.o23Date;
                c.b02ID = BO.BAS.InInt(v.ComboB02ID.SelectedValue);
                c.o12ID = BO.BAS.InInt(v.ComboO12ID.SelectedValue);                

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                string strTempDir = "c:\\temp\\CORE_UPLOAD\\TEMP";
                string strUploadDir = "c:\\temp\\CORE_UPLOAD";
                var lisO27 = new List<BO.o27Attachment>();
                foreach (string file in System.IO.Directory.EnumerateFiles(strTempDir, v.Guid + "_*.infox", System.IO.SearchOption.AllDirectories))
                {
                    var info = System.IO.File.ReadAllText(file).Split("|");
                    var strGUID = BO.BAS.GetGuid();
                    var cO27 = new BO.o27Attachment() { o27ContentType = info[0], o27FileSize = BO.BAS.InInt(info[1]), o27Name = info[2], o27GUID = strGUID };
                    cO27.o27ArchiveFileName = strGUID + "_" + cO27.o27Name;
                    cO27.o27ArchiveFolder = DateTime.Now.Year.ToString()+"\\"+DateTime.Now.Month.ToString();
                    if (!System.IO.Directory.Exists(strUploadDir+ "\\"+ cO27.o27ArchiveFolder))
                    {
                        System.IO.Directory.CreateDirectory(strUploadDir + "\\" + cO27.o27ArchiveFolder);
                    }
                    System.IO.File.Copy(strTempDir + "\\" + v.Guid + "_" + cO27.o27Name, strUploadDir + "\\" + cO27.o27ArchiveFolder + "\\"+cO27.o27ArchiveFileName, true);
                    lisO27.Add(cO27);
                }

                v.Rec.pid = Factory.o23DocBL.Save(c,lisO27);
                if (v.Rec.pid > 0)
                {
                    
                    return RedirectToActionPermanent("Index", "TheGrid", new { pid = v.Rec.pid, entity = "o23" });
                }
                else
                {
                    v.Notify(Factory.CurrentUser.ErrorMessage);
                }
            }
            if (v.Rec.pid>0) v.lisO27 = Factory.o23DocBL.GetListO27(v.Rec.pid);

            v.ComboB02ID = new MyComboViewModel("b02", v.ComboB02ID.SelectedValue, v.ComboB02ID.SelectedText, "cbxB02");
            v.ComboB02ID.Param1 = "o23";
            v.ComboO12ID = new MyComboViewModel("o12", v.ComboB02ID.SelectedValue, v.ComboB02ID.SelectedText, "cbxO12");
            v.ComboO12ID.Param1 = "o23";
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.Notify("Záznam zatím nebyl uložen.", "warning");
            return View(v);
        }


        public IActionResult DoUpload(string guid)
        {
            ViewBag.guid = guid;
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> DoUpload(List<IFormFile> files,string guid)
        {
            ViewBag.guid = guid;
            long size = files.Sum(f => f.Length);

            var tempfiles = new List<string>();
            var tempDir = @"c:\temp\CORE_UPLOAD\TEMP\";
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
            string strUploadDir = "c:\\temp\\CORE_UPLOAD";

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
        [HttpGet]
        public ActionResult FileDownload(string guid)
        {
            var c = Factory.o23DocBL.LoadO27ByGuid(guid);
            string strUploadDir = "c:\\temp\\CORE_UPLOAD";
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
            string strUploadDir = "c:\\temp";
            var fullPath = strUploadDir + "\\notfound.txt";
            System.IO.File.WriteAllText(fullPath, string.Format("Soubor [{0}] na serveru [??????\\{1}] neexistuje!", c.o27Name, c.o27ArchiveFolder));
            Response.Headers["Content-Disposition"] = string.Format("inline; filename={0}", "notfound.txt");
            var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(fullPath), "text/plain");
            return fileContentResult;

        }

        public ActionResult FileDownloadTempFile(string tempfilename,string guid)
        {
            
            string strTempDir = "c:\\temp\\CORE_UPLOAD\\TEMP";

            string fullPath = strTempDir + "\\" + tempfilename;

            
            return File(System.IO.File.ReadAllBytes(fullPath), "application/octet-stream", tempfilename.Replace(guid + "_",""));
        }


    }
}