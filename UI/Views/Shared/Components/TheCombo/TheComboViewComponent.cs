using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UI.Views.Shared.Components.TheCombo
{
    public class TheComboViewComponent:ViewComponent
    {
        
        BO.RunningUser _user;
        public TheComboViewComponent(BO.RunningUser ru)
        {
            _user = ru;
        }

        //public IViewComponentResult
        //    Invoke(string TheComboEntity,string TheComboValue, string TheComboText)
        //{
        //    var c = new Models.MyComboViewModel(TheComboEntity, TheComboValue, TheComboText, "cbx"+ TheComboEntity);

        //    return View("Default", c);
        //}

        public IViewComponentResult
            Invoke(Models.MyComboViewModel TheComboInput)
        {
            //var c = new Models.MyComboViewModel(TheComboEntity, TheComboValue, TheComboText, "cbx" + TheComboEntity);
            
            return View("Default", TheComboInput);
        }


    }
}
