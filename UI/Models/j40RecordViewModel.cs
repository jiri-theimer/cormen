using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class j40RecordViewModel:BaseViewModel
    {
        public BO.j40MailAccount Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }
    }
}
