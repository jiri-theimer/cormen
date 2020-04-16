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

        
        public MyComboViewModel ComboB02ID { get; set; }

        public MyComboViewModel ComboP28ID { get; set; }
    }
}
