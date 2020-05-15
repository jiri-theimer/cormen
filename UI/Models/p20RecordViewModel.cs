using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p20RecordViewModel:BaseViewModel
    {
        public BO.p20Unit Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }
    }
}
