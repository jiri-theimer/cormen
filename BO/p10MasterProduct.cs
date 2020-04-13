using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p10MasterProduct:BaseBO
    {
        [Key]
        public int p10ID { get; set; }
        public int p13ID { get; set; }
        public int o12ID { get; set; }
        public int b02ID { get; set; }
        [Required(ErrorMessage = "Chybí vyplnit název!")]
        [MaxLength(100, ErrorMessage = "Maximum 100 znaků")]
        public string p10Name { get; set; }
        [MaxLength(1000,ErrorMessage ="Maximum 1000 znaků")]
        public string p10Memo { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p10Code { get; set; }

        private string _p13Name;
        public string p13Name { get { return _p13Name; } }

        private string _b02Name;
        public string b02Name { get { return _b02Name; } }
        private string _o12Name;
        public string o12Name { get { return _o12Name; } }
    }
}
