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
        public int b02ID { get; set; }

        public int p12ID { get; set; }
        public int p21ID { get; set; }
               
        public int p10ID_Master { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        [MaxLength(100, ErrorMessage = "Maximum 100 znaků")]
        public string p11Name { get; set; }
        [MaxLength(1000, ErrorMessage = "Maximum 1000 znaků")]
        public string p11Memo { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p11Code { get; set; }

        public double p11RecalcUnit2Kg { get; set; }

        public double p11UnitPrice { get; set; }
        public int p20ID { get; set; }
        public int p20ID_Pro { get; set; }

        public ProductTypeEnum p11TypeFlag { get; set; }
        public string p11PackagingCode { get; set; }

        public string p12Name { get; set; } //get+set: kvůli mycombo
        public string p12Code;
        public string b02Name { get; set; }      //combo
        public string p21Name { get; set; }//get+set: kvůli mycombo
        public string p21Code;
        public string p28Name;
        public string p10Name { get; set; }//get+set: kvůli mycombo
        public string p10Code;
        public string p20Code { get; set; } //get+set: kvůli mycombo
        public string p20Name;
        public string TagHtml;

        public string p20NamePro;
        public string p20CodePro { get; set; } //combo

        public double p11Davka { get; set; }
        public double p11DavkaMin { get; set; }
        public double p11DavkaMax { get; set; }
        public double p11SalesPerMonth { get; set; }
        public double p11UnitsPerPalette { get; set; }
    }
}
