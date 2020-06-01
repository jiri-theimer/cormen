using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p14RecordViewModel:BaseViewModel
    {
        public BO.p14MasterOper Rec { get; set; }
        public BO.p13MasterTpv RecP13 { get; set; }
        public MyToolbarViewModel Toolbar { get; set; }
    }
}
