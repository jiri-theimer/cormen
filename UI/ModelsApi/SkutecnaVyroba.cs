using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.ModelsApi
{
    public class SkutecnaVyroba
    {
        public string p41Code { get; set; }
        
        public string p45MaterialCode { get; set; }
        public string p45MaterialBattch { get; set; }
        
        public int p45RowNum { get; set; }
        public string p45OperCode { get; set; }

        public int p45OperNum { get; set; }

        public double p45OperParam { get; set; }
        public string p45OperStatus { get; set; }


        public DateTime p45Start { get; set; }
        public DateTime p45End { get; set; }
        public double p45TotalDurationOperMin { get; set; }
        
        public int p45EndFlag { get; set; }
        public double p45MaterialUnitsCount { get; set; }

        public string p45Operator { get; set; }

    }
}
