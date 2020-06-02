using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p12PreviewViewModel:BaseViewModel
    {
        public BO.p12ClientTpv Rec { get; set; }
        
        public BO.p13MasterTpv RecP13 { get; set; }
        public BO.p21License RecP21 { get; set; }
    }
}
