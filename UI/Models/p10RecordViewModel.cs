using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p10RecordViewModel: BaseViewModel
    {
        public BO.p10MasterProduct Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        public TheComboViewModel ComboP13ID { get; set; }
        public TheComboViewModel ComboB02ID { get; set; }
        public TheComboViewModel ComboO12ID { get; set; }
    }
}
