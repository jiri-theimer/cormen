using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p41RecordViewModel:BaseViewModel
    {
        public BO.p41Task Rec { get; set; }
        public BO.p10MasterProduct RecP10 { get; set; }
       
        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }
        public BO.p52OrderItem RecP52 { get; set; }
        public BO.p51Order RecP51 { get; set; }
        public MyToolbarViewModel Toolbar { get; set; }
    }
}
