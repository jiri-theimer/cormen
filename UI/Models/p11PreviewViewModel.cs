﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p11PreviewViewModel:BaseViewModel
    {
        public BO.p11ClientProduct Rec { get; set; }
        public BO.p10MasterProduct RecP10 { get; set; }

    }
}
