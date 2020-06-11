using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p41PreviewViewModel:BaseViewModel
    {
        public BO.p41Task Rec { get; set; }

        
        public BO.p52OrderItem RecP52 { get; set; }
        public BO.p51Order RecP51 { get; set; }

        public BO.p41Task RecSuccessor { get; set; }
        public BO.p41Task RecPredecessor { get; set; }
    }
}
