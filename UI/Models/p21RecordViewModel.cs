﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p21RecordViewModel : BaseViewModel
    {
        public BO.p21License Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }

        
        
    }
}
