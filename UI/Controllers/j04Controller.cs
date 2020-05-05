using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace UI.Controllers
{
    public class j04Controller : BaseController
    {
        ///APLIKAČNÍ ROLE
        public IActionResult Record(int pid, bool isclone)
        {
            var v = new Models.j04RecordViewModel();
            if (pid > 0)
            {
                v.Rec = Factory.j04UserRoleBL.Load(pid);
                if (v.Rec == null)
                {
                    return RecNotFound(v);
                }

            }
            else
            {
                v.Rec = new BO.j04UserRole();
                v.Rec.entity = "j04";
               
            }
            v.SelectedPermissions = new List<int>();
            foreach (var item in v.PermCatalogue)
            {
                int x = (int)item.Value;
                int y = v.Rec.j04PermissionValue & Convert.ToInt32(item.Value);
                if (x == y)
                {
                    v.SelectedPermissions.Add(x);
                }
            }
             

            v.Toolbar = new MyToolbarViewModel(v.Rec);
            if (isclone) { v.Toolbar.MakeClone(); }

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(Models.j04RecordViewModel v)
        {
            if (ModelState.IsValid)
            {
                BO.j04UserRole c = new BO.j04UserRole();
                if (v.Rec.pid > 0) c = Factory.j04UserRoleBL.Load(v.Rec.pid);

                
                c.j04Name = v.Rec.j04Name;
                c.j04PermissionValue = v.SelectedPermissions.Sum();
                c.j04IsClientRole = v.Rec.j04IsClientRole;
                c.ValidUntil = v.Toolbar.GetValidUntil(c);
                c.ValidFrom = v.Toolbar.GetValidFrom(c);

                v.Rec.pid = Factory.j04UserRoleBL.Save(c);
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