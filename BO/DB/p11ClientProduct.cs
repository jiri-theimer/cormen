﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p11ClientProduct:BaseBO
    {
        [Key]
        public int p11ID { get; set; }

        public int p12ID { get; set; }
        public int p21ID { get; set; }
       
        public int b02ID { get; set; }
        
        [Required(ErrorMessage = "Chybí vyplnit název!")]
        [MaxLength(100, ErrorMessage = "Maximum 100 znaků")]
        public string p11Name { get; set; }
        [MaxLength(1000, ErrorMessage = "Maximum 1000 znaků")]
        public string p11Memo { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p11Code { get; set; }


        public string p12Name;
        public string b02Name;        
        public string p21Name;
        public string p10Name;
        
    }
}