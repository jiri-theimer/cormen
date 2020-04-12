﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class p26Msz:BaseBO
    {
        [Key]
        public int p26ID { get; set; }
        public int p28ID { get; set; }
        public int b02ID { get; set; }
        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string p26Name { get; set; }
        public string p26Memo { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit kód!")]
        public string p26Code { get; set; }

        private string _p28Name;
        public string p28Name { get { return _p28Name; } }

        private string _b02Name;
        public string b02Name { get { return _b02Name; } }


    }
}
