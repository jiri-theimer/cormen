using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p19Material: BaseBO
    {
        [Key]
        public int p19ID { get; set; }
        public int o12ID { get; set; }
        public int p20ID { get; set; }
        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string p19Name { get; set; }
        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p19Code { get; set; }
        public string p19Memo { get; set; }

        public int p19DefaultOperParam { get; set; }

        public string o12Name { get; set; }
    }
}
