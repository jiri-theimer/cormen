using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p12ClientTpv:BaseBO
    {
        [Key]
        public int p12ID { get; set; }
        public int p13ID_Master { get; set; }
        public int p21ID { get; set; }
        public int p25ID { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        [MaxLength(100, ErrorMessage = "Maximum 100 znaků")]
        public string p12Name { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód receptury!")]
        public string p12Code { get; set; }
        [MaxLength(1000, ErrorMessage = "Maximum 1000 znaků")]
        public string p12Memo { get; set; }

        public string p13Name;
        public string p13Code;
        public string p25Name { get; set; }//get+set: kvůli mycombo
        

        public string p21Name { get; set; }//get+set: kvůli mycombo
        public string p21Code;
        public string p28Name;

    }
}
