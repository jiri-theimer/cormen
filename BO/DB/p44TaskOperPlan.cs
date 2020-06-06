using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p44TaskOperPlan:BaseBO
    {
        public int p44ID { get; set; }
        public int p41ID { get; set; }
        public int p19ID { get; set; }
        public int p18ID { get; set; }

        public DateTime p44Start { get; set; }
        public DateTime p44End { get; set; }


        public int p44RowNum { get; set; }
        public int p44OperNum { get; set; }
        public int p44OperParam { get; set; }
        public double p44MaterialUnitsCount { get; set; }
        public double p44TotalDurationOperMin { get; set; }
               
       
    }
}
