﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    //public enum p18FlagEnum
    //{
    //    TO=1,
    //    PRE=2,
    //    POST=3
    //}
    
    
    public class p18OperCode:BaseBO
    {
        [Key]
        public int p18ID { get; set; }
                
        [Range(1, int.MaxValue,ErrorMessage = "Chybí vyplnit typ zařízení")]
        public int p25ID { get; set; }        
        public int p19ID { get; set; }  //předvyplňovat materiál v TPV

        public int p18Flag { get; set; }    //1,2,3,4

        public bool p18IsManualAmount { get; set; } //vyžaduje se ruční změna množství

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string p18Name { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]        
        public string p18Code { get; set; }

        public string p18Memo { get; set; }

        public double p18UnitsCount { get; set; }   //předvyplňovat v TPV
        public double p18DurationPreOper { get; set; }  //předvyplňovat v TPV
        public double p18DurationOper { get; set; } //předvyplňovat v TPV, podíl: Počet minut / Počet jednotek
        public double p18DurationPostOper { get; set; } //předvyplňovat v TPV

        public string p18Lang1 { get; set; }
        public string p18Lang2 { get; set; }
        public string p18Lang3 { get; set; }
        public string p18Lang4 { get; set; }

        public string CodePlusName { get; set; }

        public string p25Name { get; set; } //kvůli combo
        public string p19Name { get; set; } //kvůli combo
        public string p19Code { get; set; }

        public double p18OperParam { get; set; }

        public bool p18IsRepeatable { get; set; }

        public double p18DurOperUnits { get; set; } //Počet jednotek
        public double p18DurOperMinutes { get; set; }   //Počet minut
    }
}
