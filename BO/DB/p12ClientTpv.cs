using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p12ClientTpv:BaseBO
    {
        [Key]
        public int p12ID { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        [MaxLength(100, ErrorMessage = "Maximum 100 znaků")]
        public string p12Name { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód TPV!")]
        public string p12Code { get; set; }
        [MaxLength(1000, ErrorMessage = "Maximum 1000 znaků")]
        public string p12Memo { get; set; }
        
    }
}
