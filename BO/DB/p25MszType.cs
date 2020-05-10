using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p25MszType:BaseBO
    {
        [Key]
        public int p25ID { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string p25Name { get; set; }    
        public string p25Code { get; set; }

        public string p25Memo { get; set; }
    }
}
