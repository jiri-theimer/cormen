﻿using System;
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
        [MaxLength(1000, ErrorMessage = "Maximum 1000 znaků")]
        public string p21Memo { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p21Code { get; set; }
        public double p21Price { get; set; }
        
        public string p28Name { get; set; }
        
       
        public string b02Name { get; set; }
    }
}
