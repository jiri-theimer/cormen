using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p13RecordViewModel : BaseViewModel
    {
        public BO.p13MasterTpv Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        
      
        public List<BO.p14MasterOper> lisP14 { get; set; }
    }
}
