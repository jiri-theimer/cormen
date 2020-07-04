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
            var tg = Factory.o51TagBL.GetTagging("j02", pid);           
            v.Rec.TagHtml = tg.TagHtml;
            return View(v);

        }
        public IActionResult Record(int pid, bool isclone)
        {
            if (!this.TestIfUserEditor(true, true))
            {
                return this.StopPageCreateEdit(true);
            }
            var v = new Models.j02RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.j02PersonBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }
                if (!this.TestIfRecordEditable(v.Rec.j02ID_Owner))
                {
                    return this.StopPageEdit(true);
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
                var tg = Factory.o51TagBL.GetTagging("j02", pid);
                v.TagPids = tg.TagPids;
                v.TagNames = tg.TagNames;
                v.TagHtml = tg.TagHtml;

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
                c.j02JobTitle = v.Rec.j02JobTitle;
                c.j02ID_Owner = v.Rec.j02ID_Owner;

                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                if (ValidateBeforeSave(c, v))
                {
                    v.Rec.pid = Factory.j02PersonBL.Save(c);
                    if (v.Rec.pid > 0)
                    {
                        Factory.o51TagBL.SaveTagging("j02", v.Rec.pid, v.TagPids);

                        c = Factory.j02PersonBL.Load(v.Rec.pid);
                        if (v.IsUserProfile == false)
                        {
                            v.SetJavascript_CallOnLoad(v.Rec.pid);
                            return View(v);
                        }
                    }
                   
                    if (c.pid > 0 && v.IsUserProfile==true)
                    {
                        BO.j03User cU = new BO.j03User();

                        cU.j02ID = c.pid;
                        if (c.j03ID > 0)
                        {
                            cU = Factory.j03UserBL.Load(c.j03ID);
                        }
                        cU.j04ID = v.UserProfile.j04ID;
                        cU.j03Login = v.UserProfile.j03Login;
                        cU.j03IsMustChangePassword = v.UserProfile.j03IsMustChangePassword;
                        cU.ValidUntil = c.ValidUntil;
                        if (c.j03ID==0) cU.j03EnvironmentFlag = 2;  //klientské rozhraní

                        if (!string.IsNullOrEmpty(v.ResetPassword))
                        {                            
                            var lu = new BO.LoggingUser();
                            cU.j03PasswordHash = lu.Pwd2Hash(v.ResetPassword, cU);                                                       
                        }
                        int intJ03ID = Factory.j03UserBL.Save(cU);
                        if (intJ03ID> 0)
                        {
                            if (cU.j03ID == 0)  //nahodit první heslo pro nového uživatele
                            {                                
                                cU = Factory.j03UserBL.Load(intJ03ID);
                                var lu = new BO.LoggingUser();
                                cU.j03PasswordHash = lu.Pwd2Hash(v.ResetPassword, cU);
                                Factory.j03UserBL.Save(cU);
                            }
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
                if (c.p28ID == 0  || Factory.p21LicenseBL.HasClientValidLicense(c.p28ID) ==false)
                {
                    Factory.CurrentUser.AddMessage("Osoba s uživatelským účtem musí mít vazbu na subjekt (firmu) s platnou licencí užívat tento software.");
                    return false;
                }
                if (Factory.j03UserBL.GetList(new BO.myQuery("j03User")).Where(p=>p.pid !=c.j03ID && p.j03Login.ToUpper()==v.UserProfile.j03Login.ToUpper()).Count()>0)
                {
                    Factory.CurrentUser.AddMessage("Uživatel s tímto loginem již existuje.");
                    return false;
                }
                
            }
            



            return true;
        }

        public IActionResult WhoIsOnline(int go2pid)
        {
            var v = new TheGridInstanceViewModel() { prefix = "j02", go2pid = go2pid, contextmenuflag = 1 };
            v.entity = BL.TheEntities.ByPrefix("j02").TableName;
           
            
            return View(v);

        }
    }
}