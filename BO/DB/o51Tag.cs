﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class o51Tag:BaseBO
    {
        [Key]
        public int o51ID { get; set; }
        public int o53ID { get; set; }
        public int j02ID_Owner { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string o51Name { get; set; }

        
        public string o51Entities { get; set; }
        public string o51Code { get; set; }

        public string o53Name { get; set; }
    }
}
