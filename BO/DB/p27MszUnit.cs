using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p27MszUnit: BaseBO
    {
        [Key]
        public int p27ID { get; set; }
        public int p26ID { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string p27Name { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p27Code { get; set; }

        public double p27Capacity { get; set; }

        public string p26Name { get; set; }//kvůli combo
        public int p25ID;    //dotaženo ze stroje
        public int p31ID;   //dotaženo ze stroje
        public string StrediskoPlusStroj;
    }
}
