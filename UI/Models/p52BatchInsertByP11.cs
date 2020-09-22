using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p52BatchInsertByP11 : BaseViewModel
    {
        public string p11ids { get; set; }
        public int SelectedP51ID{ get; set; }
        public string p51Name { get; set; }
        public List <BO.p52OrderItem> lisP52 { get; set; }

        public BO.p51Order RecP51 { get; set; }

        public DateTime? p52DateNeeded { get; set; }
        
    }
}
