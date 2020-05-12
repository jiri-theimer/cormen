using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p25RecordViewModel:BaseViewModel
    {
        public BO.p25MszType Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        public int p25ID_CopyTemplate { get; set; }
        public string p25Name_CopyTemplate { get; set; }

        public IEnumerable<BO.p18OperCode> lisP18 { get; set; }
    }
}
