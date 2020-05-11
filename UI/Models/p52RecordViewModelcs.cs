using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p52RecordViewModelcs:BaseViewModel
    {
        public BO.p52OrderItem Rec { get; set; }

        public BO.p51Order Rec_Header { get; set; }
        public MyToolbarViewModel Toolbar { get; set; }
    }
}
