using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p44RecordViewModel:BaseViewModel
    {
        public BO.p44TaskOperPlan Rec { get; set; }
        public BO.p41Task RecP41 { get; set; }
        public BO.p18OperCode RecP18 { get; set; }

        public BO.p27MszUnit RecP27 { get; set; }

        public int p18flag { get; set; }
        public MyToolbarViewModel Toolbar { get; set; }
    }
}
