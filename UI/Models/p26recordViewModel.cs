using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p26RecordViewModel : BaseViewModel
    {
        public BO.p26Msz Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }


        public TheComboViewModel ComboB02ID { get; set; }

        public TheComboViewModel ComboP28ID { get; set; }

        
        public UI.Views.Shared.TagHelpers.PokusTagHelpder Pokus1 {get;set;}

    }
}
