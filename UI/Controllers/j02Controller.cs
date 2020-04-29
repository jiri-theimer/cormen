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
            
            
            v.TitleBeforeName = new MyAutoCompleteViewModel(1, v.Rec.j02TitleBeforeName,"Titul","pop1");
            v.TitleAfterName = new MyAutoCompleteViewModel(2, v.Rec.j02TitleAfterName,"","pop2");
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
                 
                c.p28ID = v.ComboP28ID.SelectedValue;
                c.j02TitleBeforeName = v.TitleBeforeName.SelectedText;
                c.j02TitleAfterName = v.TitleAfterName.SelectedText;
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
                        cU.j04ID = v.ComboJ04ID.SelectedValue;
                        cU.j03Login = v.UserProfile.j03Login;
                        cU.j03IsMustChangePassword = v.UserProfile.j03IsMustChangePassword;
                        cU.ValidUntil = c.ValidUntil;

                        if (!string.IsNullOrEmpty(v.ResetPassword))
                        {                            
                            var lu = new BO.LoggingUser();
                            cU.j03PasswordHash = lu.Pwd2Hash(v.ResetPassword, cU);                                                       
                        }
                        Factory.j03UserBL.Save(cU);

                        return RedirectToAction("Index", "TheGrid", new { pid = c.pid, entity = "j02" });
                    }
                    
                }                
               
            }
            
            v.TitleBeforeName = new MyAutoCompleteViewModel(1, v.TitleBeforeName.SelectedText, "Titul", "pop1");
            v.TitleAfterName = new MyAutoCompleteViewModel(2, v.TitleAfterName.SelectedText, "", "pop2");

            RefreshState(v);
            this.Notify_RecNotSaved();
            return View(v);


        }

        private void RefreshState(j02RecordViewModel v)
        {
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (Request.Method == "GET")    //myCombo má vstupní parametry modelu v hidden polích a proto se v POST vše dostane na server
            {
                v.ComboJ04ID = new MyComboViewModel() { Entity = "j04UserRole", SelectedText = v.Rec.j04Name, SelectedValue = v.UserProfile.j04ID };
                v.ComboP28ID = new MyComboViewModel() { Entity = "p28Company", SelectedText = v.Rec.p28Name, SelectedValue = v.Rec.p28ID };                
            }



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

                if (string.IsNullOrEmpty(v.UserProfile.j03Login) || v.ComboJ04ID.SelectedValue==0)
                {
                    Factory.CurrentUser.AddMessage("Uživatel musí mít vyplněný uživatelský účet.");return false;
                }
                if ((c.j03ID == 0 && string.IsNullOrEmpty(v.ResetPassword)))
                {
                    Factory.CurrentUser.AddMessage("Pro nového uživatele musíte definovat výchozí heslo.");return false;
                    
                }
            }
            
            
            
            return true;
        }
    }
}