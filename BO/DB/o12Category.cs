using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class o12Category:BaseBO
    {
        [Key]
        public int o12ID { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string o12Name { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit druh entity!")]
        public string o12Entity { get; set; }
        public string o12Code { get; set; }

 
       
    }
}
