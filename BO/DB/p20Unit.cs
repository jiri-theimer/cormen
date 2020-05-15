using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p20Unit:BaseBO
    {
        [Key]
        public int p20ID { get; set; }
       
        [Required(ErrorMessage = "Chybí vyplnit název!")]
        [MaxLength(50, ErrorMessage = "Maximum 50 znaků")]
        public string p20Name { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p20Code { get; set; }
    }
}
