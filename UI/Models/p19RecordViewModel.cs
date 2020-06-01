﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p19RecordViewModel:BaseViewModel
    {
        public BO.p19Material Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        public string TagIds { get; set; }
        public string TagNames { get; set; }
    }
}
