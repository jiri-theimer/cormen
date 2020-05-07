using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p41Task:BaseBO
    {
        [Key]
        public int p41ID { get; set; }

        public int p11ID { get; set; }
        public int p26ID { get; set; }
        public int p28ID { get; set; }
        public int j02ID_Owner { get; set; }

        public int b02ID { get; set; }
        [Required(ErrorMessage = "Chybí vyplnit název!")]
        [MaxLength(100, ErrorMessage = "Maximum 100 znaků")]
        public string p41Name { get; set; }
        [MaxLength(1000, ErrorMessage = "Maximum 1000 znaků")]
        public string p41Memo { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p41Code { get; set; }

        public string p41StockCode { get; set; }

        public DateTime? p41PlanStart { get; set; }
        public DateTime? p41PlanEnd { get; set; }
        public DateTime? p41RealStart { get; set; }
        public DateTime? p41RealEnd { get; set; }
        public double p41PlanUnitsCount { get; set;}
        public int p41ActualRowNum { get; set; }
        public double p41RealUnitsCount { get; set; }

        public string p11Name { get; set; } //get+set: kvůli mycombo
        public string b02Name { get; set; } //get+set: kvůli mycombo
        public string p28Name { get; set; } //get+set: kvůli mycombo
        public string p26Name { get; set; } //get+set: kvůli mycombo
        public string RecordOwner;
    }
}
