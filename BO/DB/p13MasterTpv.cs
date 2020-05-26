using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p13MasterTpv:BaseBO
    {
        [Key]
        public int p13ID { get; set; }
        public int p25ID { get; set; }

       
        [MaxLength(100, ErrorMessage = "Maximum 100 znaků")]
        public string p13Name { get; set; }

                
        public string p13Code { get; set; }
        [MaxLength(1000, ErrorMessage = "Maximum 1000 znaků")]
        public string p13Memo { get; set; }

        public double p13TotalDuration { get; set; }

        public string p25Name { get; set; } //kvůli combo
    }
}
