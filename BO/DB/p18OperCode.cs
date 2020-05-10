using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p18OperCode:BaseBO
    {
        [Key]
        public int p18ID { get; set; }
        public int p25ID { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string p18Name { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]        
        public string p18Code { get; set; }

        public string CodePlusName { get; set; }

        public string p25Name { get; set; } //kvůli combo
    }
}
