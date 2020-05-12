﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p52OrderItem:BaseBO
    {
        public int p52ID { get; set; }        
        public int p51ID { get; set; }
       
        public int p11ID { get; set; }
        public int j02ID_Owner { get; set; }
        public string p52Code { get; set; }

        
        public double p52UnitsCount { get; set; }
        public DateTime? p52PlanStart { get; set; }
        public DateTime? p52PlanEnd { get; set; }

        public string p11Name { get; set; }
        public string p11Code { get; set; }
        public string p51Code;
    }
}
