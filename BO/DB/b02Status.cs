using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class b02Status: BaseBO
    {
        [Key]
        public int b02ID { get; set; }
        
        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string b02Name { get; set; }
        public int b02Ordinary { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit druh entity!")]
        public string b02Entity { get; set; }
        public string b02Code { get; set; }
        public string b02Memo { get; set; }
    }
}
