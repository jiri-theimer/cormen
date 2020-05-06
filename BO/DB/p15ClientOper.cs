using System;
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

        public string p15Name { get; set; }

        public int p15RowNum { get; set; }

        public string p15MaterialCode { get; set; }
        public string p15MaterialName { get; set; }
        public string p15OperCode { get; set; }
        public string p15OperNum { get; set; }
        public int p15OperParam { get; set; }
        public double p15UnitsCount { get; set; }
        public double p15DurationPreOper { get; set; }
        public double p15DurationPostOper { get; set; }
        public double p15DurationOper { get; set; }
    }
}
