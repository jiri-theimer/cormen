﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class PeriodViewModel
    {
        public DateTime? d1 { get; set; }
        public DateTime? d2 { get; set; }

        public int PeriodValue { get; set; }
        //public List<BO.ThePeriod> lisPeriods { get; set; }

        public bool IsShowButtonRefresh { get; set; }

        //public string SelectedB02IDs { get; set; }
        //public string SelectedB02Names { get; set; }
    }
}
