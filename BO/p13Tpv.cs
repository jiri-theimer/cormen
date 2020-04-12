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
        public string p13Name { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód TPV!")]        
        public string p13Code { get; set; }

        public string p13Memo { get; set; }
    }
}
