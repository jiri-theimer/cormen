using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class o12RecordViewModel:BaseViewModel
    {
        public BO.o12Category Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }
    }
}
