using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p27RecordViewModel:BaseViewModel
    {
        public BO.p27MszUnit Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }
    }
}
