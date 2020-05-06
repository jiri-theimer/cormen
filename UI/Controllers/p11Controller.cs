using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    public class p11Controller : BaseController
    {
        public IActionResult Index(int pid)
        {

            var v = new Models.p11PreviewViewModel();
            v.Rec = Factory.p11ClientProductBL.Load(pid);
            if (v.Rec == null) return RecNotFound(v);
            return View(v);

        }
    }
}