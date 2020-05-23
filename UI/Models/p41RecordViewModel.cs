using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p41RecordViewModel:BaseViewModel
    {
        public BO.p41Task Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }
    }
}
