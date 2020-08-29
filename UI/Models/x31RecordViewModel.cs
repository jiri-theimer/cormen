using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class x31RecordViewModel: BaseViewModel
    {
        public BO.x31Report Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        public string UploadGuid { get; set; }
    }
}
