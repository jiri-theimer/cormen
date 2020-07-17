using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum p19TypeFlagEnum
    {
        Zbozi=1,
        Polotovar=2,
        Vyrobek=3,
        Surovina=4,
        Obal=5,
        Etiketa=6
    }
    public class p19Material: BaseBO
    {
        [Key]
        public int p19ID { get; set; }
        
        public int p20ID { get; set; }
        public int p28ID { get; set; }
        public int p10ID_Master { get; set; }

        public p19TypeFlagEnum p19TypeFlag { get; set; }
        public int j02ID_Owner { get; set; }
        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string p19Name { get; set; }
        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p19Code { get; set; }
        
        public string p19Memo { get; set; }

        public string p19Supplier { get; set; }
        public string p19Intrastat { get; set; }
        public string p19NameAlias { get; set; }
        public string p19ITSINC { get; set; }
        public string p19ITSCAS { get; set; }
        public string p19ITSEINECS { get; set; }

        public string p19Lang1 { get; set; }
        public string p19Lang2 { get; set; }
        public string p19Lang3 { get; set; }
        public string p19Lang4 { get; set; }


        
        public string p28Name { get; set; }
        public string p20Code { get; set; }

        public double p19StockActual { get; set; }
        public double p19StockReserve { get; set; }
        public DateTime? p19StockDate { get; set; }
    }
}
