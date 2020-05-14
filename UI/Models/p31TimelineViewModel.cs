using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p31TimelineViewModel:BaseViewModel

    {
        public BO.p31CapacityFond Rec { get; set; }

        public DateTime CurrentDate { get; set; }
        public BO.p31CapacityFond RecP31 { get; set; }

        
    }
}
