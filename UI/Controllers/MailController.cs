using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class MailController : BaseController
    {
        public IActionResult SendMail()
        {

            var v = new Models.SendMailViewModel();
            v.Rec = new BO.x40MailQueue();
            v.Rec.j40ID = BO.BAS.InInt(Factory.CBL.LoadUserParam("SendMail_j40ID"));
            v.Rec.j40Name = Factory.CBL.LoadUserParam("SendMail_j40Name");            
            v.Rec.x40MessageGuid= BO.BAS.GetGuid();
            v.UploadGuid = BO.BAS.GetGuid();

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMail(Models.SendMailViewModel v)
        {
            if (ModelState.IsValid)
            {
                foreach(BO.o27Attachment c in BO.BASFILE.GetUploadedFiles(Factory.App.TempFolder, v.UploadGuid))
                {
                    Factory.MailBL.AddAttachment(c.FullPath,c.o27Name,c.o27ContentType);
                }
                
                BO.Result r = Factory.MailBL.SendMessage(v.Rec);
                if (v.Rec.j40ID > 0)
                {
                    Factory.CBL.SetUserParam("SendMail_j40ID", v.Rec.j40ID.ToString());
                    Factory.CBL.SetUserParam("SendMail_j40Name", v.Rec.j40Name);
                }
                
                if (r.Flag==BO.ResultEnum.Success)  //případná chybová hláška je již naplněná v BL vrstvě
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }
                

            }

            return View(v);
        }


        public IActionResult Record_j04(int pid, bool isclone)
        {
           
            var v = new Models.j40RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.MailBL.LoadJ40(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.j40MailAccount();
                v.Rec.entity = "j40";

            }
            


            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record_j04(Models.j40RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.j40MailAccount c = new BO.j40MailAccount();
                if (v.Rec.pid > 0) c = Factory.MailBL.LoadJ40(v.Rec.pid);
                c.j02ID_Owner = v.Rec.j02ID_Owner;
                c.j40UsageFlag = v.Rec.j40UsageFlag;
                c.j40SmtpHost = v.Rec.j40SmtpHost;
                c.j40SmtpPort = v.Rec.j40SmtpPort;
                c.j40SmtpName = v.Rec.j40SmtpName;
                c.j40SmtpEmail = v.Rec.j40SmtpEmail;
                c.j40SmtpUsePersonalReply = v.Rec.j40SmtpUsePersonalReply;
                c.j40SmtpLogin = v.Rec.j40SmtpLogin;
                if (String.IsNullOrEmpty(v.Rec.j40SmtpPassword) == false)
                {
                    c.j40SmtpPassword = v.Rec.j40SmtpPassword;
                }
                
                c.j40SmtpUseDefaultCredentials = v.Rec.j40SmtpUseDefaultCredentials;
                c.j40SmtpEnableSsl = v.Rec.j40SmtpEnableSsl;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.MailBL.SaveJ40(c);
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
    }
}