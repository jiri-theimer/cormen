using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class p41CreateViewModel
    {

        public int p52ID { get; set; }
        public string p52Code { get; set; }


        public List<BO.p41Task> lisTasks { get; set; }
        
    }
}
