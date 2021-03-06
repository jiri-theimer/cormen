﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p31CapacityFond:BaseBO
    {
        [Key]
        public int p31ID { get; set; }
        public int j02ID_Owner { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string p31Name { get; set; }

        public string RecordOwner { get; set; }

        public int p31DayHour1 { get; set; } = 5;
        public int p31DayHour2 { get; set; } = 20;

    }
}
