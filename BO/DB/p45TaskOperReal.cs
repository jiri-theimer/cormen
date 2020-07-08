using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p45TaskOperReal:BaseBO
    {
        [Key]
        public int p45ID { get; set; }
        public int p41ID { get; set; }

        public int p19ID { get; set; }
        public int p18ID { get; set; }

        public DateTime p45Start { get; set; }
        public DateTime p45End { get; set; }
        public int p45EndFlag { get; set; }


        public int p45RowNum { get; set; }
        public int p45OperNum { get; set; }
        public double p45OperParam { get; set; }
        public double p45MaterialUnitsCount { get; set; }
        public double p45TotalDurationOperMin { get; set; }

        public string p45OperStatus { get; set; }
        public string p45Operator { get; set; }


        public string p45OperCode { get; set; }         //textově posílá stroj, pro jistotu ukládáme pro přípoad neshod
        public string p45Name { get; set; }             //textově posílá stroj, pro jistotu ukládáme pro přípoad neshod
        public string p45MaterialCode { get; set; }     //textově posílá stroj, pro jistotu ukládáme pro přípoad neshod
        public string p45MaterialBatch { get; set; }   //textově posílá stroj, pro jistotu ukládáme pro přípoad neshod
        public string p45MaterialName { get; set; }     //textově posílá stroj, pro jistotu ukládáme pro přípoad neshod




    }
}
