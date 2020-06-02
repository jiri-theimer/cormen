using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p51PreviewViewModel:BaseViewModel
    {
        public BO.p51Order Rec { get; set; }
        
        public IEnumerable<BO.p52OrderItem> OrderItems { get; set; }
    }
}
