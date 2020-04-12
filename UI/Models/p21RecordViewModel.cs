using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p21RecordViewModel
    {
        public BO.p21License Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        public MyComboViewModel ComboP28ID { get; set; }
        public MyComboViewModel ComboB02ID { get; set; }
    }
}
