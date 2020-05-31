using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class o51Tag:BaseBO
    {
        [Key]
        public int o51ID { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string o51Name { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit druh entity!")]
        public string o51Entity { get; set; }
        public string o51Code { get; set; }
    }
}
