using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{    
    public class j02Controller:BaseController
    {

        public IActionResult Index(int pid)
        {
            var v = new Models.j02PreviewViewModel();
            v.Rec = Factory.j02PersonBL.Load(pid);                       
            if (v.Rec == null) return RecNotFound(v);
            if (v.Rec.j03ID > 0)
            {
                v.UserProfile = Factory.j03UserBL.Load(v.Rec.j03ID);
            }
            return View(v);

        }
        public IActionResult Record(int pid, bool isclone)
        {            
            var v = new Models.j02RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.j02PersonBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                if (v.Rec.j03ID > 0)
                {
                    v.UserProfile = Factory.j03UserBL.Load(v.Rec.j03ID);
                    v.IsUserProfile = true;
                }
                else
                {
                    v.UserProfile = new BO.j03User();
                }
                
                
            }
            else
            {
                v.Rec = new BO.j02Person();
                v.Rec.entity = "j02";
                v.UserProfile = new BO.j03User();
            }
            
            RefreshState(v);
            if (isclone) { v.Toolbar.MakeClone(); }
            

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.j02RecordViewModel v)
        {           
            if (ModelState.IsValid)
            {
                BO.j02Person c = new BO.j02Person();
                BO.j03User cU = new BO.j03User();

                if (v.Rec.pid > 0)
                {
                    c = Factory.j02PersonBL.Load(v.Rec.pid);                                       
                }

                c.p28ID = v.Rec.p28ID;
                c.j02TitleBeforeName = v.Rec.j02TitleBeforeName;
                c.j02TitleAfterName = v.Rec.j02TitleAfterName;
                c.j02FirstName = v.Rec.j02FirstName;
                c.j02LastName = v.Rec.j02LastName;                
                c.j02Email = v.Rec.j02Email;
                c.j02Tel1 = v.Rec.j02Tel1;
                c.j02Tel2 = v.Rec.j02Tel2;                

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                if (ValidateBeforeSave(c, v))
                {
                    v.Rec.pid = Factory.j02PersonBL.Save(c);
                    if (v.Rec.pid > 0)
                    {
                        c = Factory.j02PersonBL.Load(v.Rec.pid);
                    }
                   
                    if (c.pid > 0 && v.IsUserProfile==true)
                    {
                        cU = new BO.j03User();
                        cU.j02ID = c.pid;
                        if (c.j03ID > 0)
                        {
                            cU = Factory.j03UserBL.Load(v.Rec.pid);
                        }
                        cU.j04ID = v.UserProfile.j04ID;
                        cU.j03Login = v.UserProfile.j03Login;
                        cU.j03IsMustChangePassword = v.UserProfile.j03IsMustChangePassword;
                        cU.ValidUntil = c.ValidUntil;

                        if (!string.IsNullOrEmpty(v.ResetPassword))
                        {                            
                            var lu = new BO.LoggingUser();
                            cU.j03PasswordHash = lu.Pwd2Hash(v.ResetPassword, cU);                                                       
                        }
                        if (Factory.j03UserBL.Save(cU) > 0)
                        {
                            v.SetJavascript_CallOnLoad(v.Rec.pid);
                            return View(v);
                        }
                        
                    }
                    
                }                
               
            }
          
            RefreshState(v);
            this.Notify_RecNotSaved();
            return View(v);


        }

        private void RefreshState(j02RecordViewModel v)
        {
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            



        }

        private bool ValidateBeforeSave(BO.j02Person c,j02RecordViewModel v)
        {
            if (v.IsUserProfile)
            {
                if (!string.IsNullOrEmpty(v.ResetPassword))
                {
                    var lu = new BO.LoggingUser();
                    var res = lu.ValidatePassword(v.ResetPassword);
                    if (res.Flag == BO.ResultEnum.Failed)
                    {
                        Factory.CurrentUser.AddMessage(res.Message);
                        return false;
                    }
                    
                }

                if (string.IsNullOrEmpty(v.UserProfile.j03Login) || v.UserProfile.j04ID==0)
                {
                    Factory.CurrentUser.AddMessage("Uživatel musí mít vyplněný uživatelský účet.");return false;
                }
                if ((c.j03ID == 0 && string.IsNullOrEmpty(v.ResetPassword)))
                {
                    Factory.CurrentUser.AddMessage("Pro nového uživatele musíte definovat výchozí heslo.");return false;
                    
                }
                if (c.p28ID == 0  || Factory.p28CompanyBL.Load(c.p28ID).p28TypeFlag == 0)
                {
                    Factory.CurrentUser.AddMessage("Osoba s uživatelským účtem musí mít vazbu na firmu/držitele licence. Vyplňte firmu, která je držitelem licence.");
                    return false;
                }
                
            }
            
            
            
            return true;
        }
    }
}