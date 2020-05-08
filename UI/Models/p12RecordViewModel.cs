using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p12RecordViewModel:BaseViewModel
    {
        public BO.p12ClientTpv Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        public List<BO.p15ClientOper> lisP15 { get; set; }
    }
}
