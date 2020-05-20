using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class j40Controller : BaseController
    {
        public IActionResult SendMail()
        {

            var v = new Models.SendMailViewModel();
            v.Rec = new BO.x40MailQueue();
           
            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMail(Models.SendMailViewModel v)
        {
            if (ModelState.IsValid)
            {

                BO.Result r = Factory.MailBL.SendMessage(v.Rec.j40ID, v.Rec.x40To, "", v.Rec.x40Subject, v.Rec.x40Body, false);

                if (r.Flag==BO.ResultEnum.Success)
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }
                else
                {
                    Factory.CurrentUser.AddMessage(r.Message);
                }

            }

            return View(v);
        }


        public IActionResult Record(int pid, bool isclone)
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
        public IActionResult Record(Models.j40RecordViewModel v)
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