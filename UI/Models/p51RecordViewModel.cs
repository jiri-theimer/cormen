using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p51RecordViewModel:BaseViewModel
    {
        public BO.p51Order Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }
    }
}
