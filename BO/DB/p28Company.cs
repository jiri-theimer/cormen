using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p28Company:BaseBO
    {
        [Key]
        public int p28ID { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string p28Name { get; set; }
        public string p28ShortName { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p28Code { get; set; }
        
        public string p28RegID { get; set; }        
        public string p28VatID { get; set; }
        public string p28Street1 { get; set; }
        public string p28City1 { get; set; }
        public string p28PostCode1 { get; set; }
        public string p28Country1 { get; set; }
        public string p28Street2 { get; set; }
        public string p28City2 { get; set; }
        public string p28PostCode2 { get; set; }
        public string p28Country2 { get; set; }
    }
}
