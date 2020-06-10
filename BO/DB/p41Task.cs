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

        public int p52ID { get; set; }
        public int p27ID { get; set; }      
        public int j02ID_Owner { get; set; }
        public int b02ID { get; set; }
        public int p41MasterID { get; set; }        //hlavní zakázka
        public int p41SuccessorID { get; set; }     //následník

        //[Required(ErrorMessage = "Chybí vyplnit název!")]
        [MaxLength(100, ErrorMessage = "Maximum 100 znaků")]
        public string p41Name { get; set; }
        [MaxLength(1000, ErrorMessage = "Maximum 1000 znaků")]
        public string p41Memo { get; set; }

        public bool p41IsDraft { get; set; }

        //[Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p41Code { get; set; }

        public string p41StockCode { get; set; }

        public DateTime p41PlanStart { get; set; }
        public double p41Duration { get; set; }
        public double p41DurationPoPre;
        public double p41DurationPoPost;
        public DateTime p41PlanEnd {
            get
            {
                return this.p41PlanStart.AddMinutes(this.p41Duration);
            }
        }
        public double DurationSeconds
        {
            get
            {
                return (this.p41Duration * 60);
            }
        }
        public double DurationHours
        {
            get
            {
                return (this.p41Duration/60);
            }
        }
        public DateTime? p41RealStart { get; set; }
        public DateTime? p41RealEnd { get; set; }
        public double p41PlanUnitsCount { get; set;}
        public int p41ActualRowNum { get; set; }
        public double p41RealUnitsCount { get; set; }

        public string p27Name { get; set; } //get+set: kvůli mycombo
        public string b02Name { get; set; } //get+set: kvůli mycombo
        public string p52Name { get; set; } //get+set: kvůli mycombo
        public string p52Code { get; set; } //get+set: kvůli mycombo

        public string RecordOwner;
        public int p11ID;

        public bool IsTempDeleted { get; set; }
        public string TagHtml;

        public DateTime PlanStartClear  //čas bez úvodních PRE operací
        {
            get
            {
                return this.p41PlanStart.AddMinutes(this.p41DurationPoPre);
            }
        }
        
        public string CssStyleDisplay
        {
            get
            {
                if (this.IsTempDeleted == true)
                {
                    return "style='display:none;'";
                }
                else
                {
                    return "style='display:block'";
                }
            }
        }
    }
}
