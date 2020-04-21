﻿using System;
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
                
            }
            else
            {
                v.Rec = new BO.j02Person();
                v.Rec.entity = "j02";
            }
            
            v.ComboJ04ID = new MyComboViewModel("j04", v.Rec.j04ID.ToString(), v.Rec.j04Name,"cbx1");
            v.ComboP28ID = new MyComboViewModel("p28", v.Rec.p28ID.ToString(), v.Rec.p28Name, "cbx2");
            v.TitleBeforeName = new MyAutoCompleteViewModel(1, v.Rec.j02TitleBeforeName,"Titul","pop1");
            v.TitleAfterName = new MyAutoCompleteViewModel(2, v.Rec.j02TitleAfterName,"","pop2");
            v.Toolbar = new MyToolbarViewModel(v.Rec);
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
                if (v.Rec.pid > 0) c = Factory.j02PersonBL.Load(v.Rec.pid);

                c.j02IsUser = v.Rec.j02IsUser;
                if (c.j02IsUser)
                {
                    c.j04ID = BO.BAS.InInt(v.ComboJ04ID.SelectedValue);
                    c.j02Login = v.Rec.j02Login;
                }
                else
                {
                    c.j04ID = 0;
                    c.j02Login = null;
                    c.j02PasswordHash = null;
                    v.ResetPassword = "";
                }                
                c.p28ID = BO.BAS.InInt(v.ComboP28ID.SelectedValue);
                c.j02TitleBeforeName = v.TitleBeforeName.SelectedText;
                c.j02TitleAfterName = v.TitleAfterName.SelectedText;
                c.j02FirstName = v.Rec.j02FirstName;
                c.j02LastName = v.Rec.j02LastName;                
                c.j02Email = v.Rec.j02Email;
                c.j02Tel1 = v.Rec.j02Tel1;
                c.j02Tel2 = v.Rec.j02Tel2;
                c.j02IsMustChangePassword = v.Rec.j02IsMustChangePassword;


                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                if (ValidateBeforeSave(c, v))
                {
                    v.Rec.pid = Factory.j02PersonBL.Save(c);
                    if (v.Rec.pid > 0)
                    {
                        if (!string.IsNullOrEmpty(v.ResetPassword))
                        {
                            c = Factory.j02PersonBL.Load(v.Rec.pid);
                            var lu = new BO.LoggingUser();
                            c.j02PasswordHash = lu.Pwd2Hash(v.ResetPassword, c);
                           
                            Factory.j02PersonBL.Save(c);
                        }

                        return RedirectToAction("Index", "TheGrid", new { pid = v.Rec.pid, entity = "j02" });
                    }
                    
                }                
               
            }
            v.Toolbar = new MyToolbarViewModel(v.Rec);
            v.ComboJ04ID = new MyComboViewModel("j04", v.ComboJ04ID.SelectedValue, v.ComboJ04ID.SelectedText, "cbx1");
            v.ComboP28ID = new MyComboViewModel("p28", v.ComboP28ID.SelectedValue, v.ComboP28ID.SelectedText, "cbx2");
            v.TitleBeforeName = new MyAutoCompleteViewModel(1, v.TitleBeforeName.SelectedText, "Titul", "pop1");
            v.TitleAfterName = new MyAutoCompleteViewModel(2, v.TitleAfterName.SelectedText, "", "pop2");
            this.Notify_RecNotSaved();
            return View(v);


        }

        private bool ValidateBeforeSave(BO.j02Person c,j02RecordViewModel v)
        {
            if (c.j02IsUser)
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

                if (string.IsNullOrEmpty(c.j02Login) || c.j04ID==0)
                {
                    Factory.CurrentUser.AddMessage("Uživatel musí mít vyplněný uživatelský účet.");return false;
                }
                if ((c.pid == 0 && string.IsNullOrEmpty(v.ResetPassword)) || (c.pid>0 && string.IsNullOrEmpty(c.j02PasswordHash) && string.IsNullOrEmpty(v.ResetPassword)))
                {
                    Factory.CurrentUser.AddMessage("Pro nového uživatele musíte definovat výchozí heslo.");return false;
                    
                }
            }
            
            
            
            return true;
        }
    }
}