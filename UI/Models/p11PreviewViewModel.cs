﻿using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p11PreviewViewModel:BaseViewModel
    {
        public BO.p11ClientProduct Rec { get; set; }
        
        public BO.p10MasterProduct RecP10 { get; set; }
        public BO.p21License RecP21 { get; set; }

        public double SimulateUnitsCount { get; set; }
        public double SimulationResult { get; set; }
        public int p27ID_Simulation { get; set; }
        public string p27Name_Simulation { get; set; }
        

    }
}
