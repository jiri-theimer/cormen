﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace UI.Models
{
    public class p28RecordViewModel : BaseViewModel
    {
        public BO.p28Company Rec { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        
       

    }
}
