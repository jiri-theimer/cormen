using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace UI.Views.Components.Pokus1
{
    public class PokusControlViewComponent : ViewComponent
    {
        private BL.RunningApp _app;
        public PokusControlViewComponent(BL.RunningApp app)
        {
            _app = app;
        }

        public IViewComponentResult
            Invoke(BO.RunningUser PokusControlType, int PokusControlCislo)
        {
            
            PokusControlType.j02Login = "Přes DI přišel upload folder: "+_app.UploadFolder;
            return View("Index", PokusControlType);
        }
    }
}
