﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
   public class p15ClientOper:BaseBO
    {
        [Key]
        public int p15ID { get; set; }
        public int p12ID { get; set; }
        public int p19ID { get; set; }
        public int p18ID { get; set; }

        
        public int p15RowNum { get; set; }              
        public int p15OperNum { get; set; }
        public double p15OperParam { get; set; }
        public double p15UnitsCount { get; set; }
        public double p15DurationPreOper { get; set; }
        public double p15DurationPostOper { get; set; }
        public double p15DurationOper { get; set; }
        public double p15DurOperUnits { get; set; }
        public double p15DurOperMinutes { get; set; }

        public string Material { get; set; }
        public string OperCode { get; set; }
        public string OperCodePlusName { get; set; }
    }
}
