using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p26RecordViewModel : BaseViewModel
    {
        public BO.p26Msz Rec { get; set; }

        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }
        public MyToolbarViewModel Toolbar { get; set; }


      
      

    }
}
