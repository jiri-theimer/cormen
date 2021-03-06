﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p51Order:BaseBO
    {
        [Key]
        public int p51ID { get; set; }
        public int p28ID { get; set; }
        //public int p26ID { get; set; }
        public int b02ID { get; set; }
        public int j02ID_Owner { get; set; }
        
        [MaxLength(100, ErrorMessage = "Maximum 100 znaků")]
        public string p51Name { get; set; }
        public bool p51IsDraft { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]        
        public string p51Code { get; set; }
        public string p51CodeByClient { get; set; }
        public DateTime p51Date { get; set; }
        public DateTime? p51DateDelivery { get; set; }
        public DateTime? p51DateDeliveryConfirmed { get; set; }

        [MaxLength(1000, ErrorMessage = "Maximum 1000 znaků")]
        public string p51Memo { get; set; }

       
        public string RecordOwner { get; set; } //get+set: kvůli mycombo
        public string p28Name { get; set; } //get+set: kvůli mycombo
        public string p28Code;
        
        public string b02Name { get; set; } //get+set: kvůli mycombo

        public double p51Order_Kg;
        public double p51Task_Kg;

        public string TagHtml;
    }
}
