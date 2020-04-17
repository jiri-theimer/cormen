using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p14MasterOper:BaseBO
    {
        [Key]
        public int p14ID { get; set; }
        public int p13ID { get; set; }

        public string p14Name { get; set; }

        public int p14RowNum { get; set; }

        public string p14MaterialCode { get; set; }
        public string p14MaterialName { get; set; }
        public string p14OperCode { get; set; }
        public string p14OperNum { get; set; }    
        public int p14OperParam { get; set; }
        public double p14UnitsCount { get; set; }
        public double p14DurationPreOper { get; set; }
        public double p14DurationPostOper { get; set; }
        public double p14DurationOper { get; set; }

    }
}
