using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p41AppendPoViewModel:BaseViewModel
    {

        
        public int p41ID { get; set; }
        

        public BO.p41Task RecP41 { get; set; }
        public int p18flag { get; set; }

        public List<BO.AppendPostPreP44Oper> lisDestOper { get; set; }
    }
}
