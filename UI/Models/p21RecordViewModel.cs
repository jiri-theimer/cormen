using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p21RecordViewModel : BaseViewModel
    {
        public BO.p21License Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        
        public TheComboViewModel ComboP28ID { get; set; }
        public TheComboViewModel ComboB02ID { get; set; }
       
        public TheComboViewModel ComboSelectP10ID { get; set; }
        public string p10IDs { get; set; }
        
    }
}
