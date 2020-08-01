using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class b03RecordViewModel:BaseViewModel
    {
        public BO.b03StatusGroup Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }


        public List<BO.b02Status> lisB02 { get; set; }

        public List<int> SelectedB02IDs { get; set; }
    }
}
