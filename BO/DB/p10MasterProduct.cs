using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    //public enum SwLicenseEnum
    //{
    //    None=0,
    //    TypeA=1,
    //    TypeB=2,
    //    TypeC=3
    //}
    public enum ProductTypeEnum
    {
        Polotovar = 1,
        Vyrobek = 2,
        Plneni = 3
    }
    public class p10MasterProduct:BaseBO
    {
        [Key]
        public int p10ID { get; set; }


        public int p13ID { get; set; }
        
        public int b02ID { get; set; }
        public int p20ID { get; set; }
        [Required(ErrorMessage = "Chybí vyplnit název!")]
        [MaxLength(100, ErrorMessage = "Maximum 100 znaků")]
        public string p10Name { get; set; }
        [MaxLength(1000,ErrorMessage ="Maximum 1000 znaků")]
        public string p10Memo { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p10Code { get; set; }
        //public SwLicenseEnum p10SwLicenseFlag { get; set; }   //1/2/3

        public double p10RecalcUnit2Kg { get; set; }
        public string p13Name { get; set; } //get+set: kvůli mycombo
        public string p13Code;
        public string b02Name { get; set; } //get+set: kvůli mycombo
        
        public string p20Code { get; set; }
        public string p20Name;
        public string TagHtml;
        public int p25ID;

        public ProductTypeEnum p10TypeFlag { get; set; }
    }
}
