using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p13Tpv:BaseBO
    {
        [Key]
        public int p13ID { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        [MaxLength(100, ErrorMessage = "Maximum 100 znaků")]
        public string p13Name { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód TPV!")]        
        public string p13Code { get; set; }
        [MaxLength(1000, ErrorMessage = "Maximum 1000 znaků")]
        public string p13Memo { get; set; }
    }
}
