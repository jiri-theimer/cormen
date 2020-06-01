using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p15RecordViewModel:BaseViewModel
    {
        public BO.p15ClientOper Rec { get; set; }
        public BO.p12ClientTpv RecP12 { get; set; }
        public MyToolbarViewModel Toolbar { get; set; }
    }
}
