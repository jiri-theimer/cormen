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

      
        public IViewComponentResult
            Invoke(Models.TheComboViewModel input)
        {
            if (string.IsNullOrEmpty(input.ControlID))
            {
                input.ControlID = input.Entity + "cbx1";
            }
            if (input.SelectedValue == "0") { input.SelectedValue = ""; };

            var s = input.getPrefix();
            if (s == "p28" || s=="j02" || s=="p21" || s=="o23" || s=="p10" || s=="p13" || s=="p26")
            {
                return View("Multi", input);
            }
            else
            {
                return View("Single", input);
            }
            
            
        }


    }
}
