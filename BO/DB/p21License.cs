﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum p21PermENUM
    {
        Default=1,
        Independent2Master=2
        
    }
    public class p21License: BO.BaseBO
    {
        [Key]
        public int p21ID { get; set; }
        public int p28ID { get; set; }
        
        public int b02ID { get; set; }
        public int o12ID { get; set; }
        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string p21Name { get; set; }
        [MaxLength(1000, ErrorMessage = "Maximum 1000 znaků")]
        public string p21Memo { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p21Code { get; set; }
        public double p21Price { get; set; }

        public p21PermENUM p21PermissionFlag { get; set; }

        public string p28Name { get; set; } //get+set: kvůli mycombo
        public string b02Name { get; set; } //get+set: kvůli mycombo
        public string o12Name { get; set; } //get+set: kvůli mycombo

        public string PermFlagAlias
        {
            get
            {                
                if (this.p21PermissionFlag == BO.p21PermENUM.Independent2Master) return "Cyber";
                return "Standard";
            }
        }
    }
}
