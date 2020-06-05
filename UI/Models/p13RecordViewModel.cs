using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p13RecordViewModel : BaseViewModel
    {
        public BO.p13MasterTpv Rec { get; set; }

        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }
        public bool IsCloneP14Records { get; set; }
        public int p13ID_CloneSource { get; set; }
 
    }
}
