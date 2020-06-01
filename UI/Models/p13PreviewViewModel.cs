using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p13PreviewViewModel : BaseViewModel
    {
        public BO.p13MasterTpv Rec { get; set; }
        public IEnumerable<BO.p14MasterOper> lisP14 { get; set; }
    }
}
