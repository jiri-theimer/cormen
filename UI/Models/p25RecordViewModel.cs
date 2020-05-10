using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p25RecordViewModel:BaseViewModel
    {
        public BO.p25MszType Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

    }
}
