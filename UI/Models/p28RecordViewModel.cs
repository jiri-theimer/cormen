using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace UI.Models
{
    public class p28RecordViewModel : BaseViewModel
    {
        public BO.p28Company Rec { get; set; }

        public BO.j02Person FirstPerson { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }

        public bool IsFirstPerson { get; set; }

    }
}
