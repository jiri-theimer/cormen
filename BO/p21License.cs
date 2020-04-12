using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p21License: BO.BaseBO
    {
        [Key]
        public int p21ID { get; set; }
        public int p28ID { get; set; }
        public int b02ID { get; set; }
        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string p21Name { get; set; }
        public string p21Memo { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p21Code { get; set; }
        public Decimal p21Price { get; set; }

        private string _p28Name;
        public string p28Name { get { return _p28Name; } }

        private string _b02Name;
        public string b02Name { get { return _b02Name; } }
    }
}
