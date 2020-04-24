﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j02Person : BaseBO
    {
        [Key]
        public int j02ID { get; set; }
        public int p28ID { get; set; }
        public bool j02IsUser { get; set; }
        public bool j02IsMustChangePassword { get; set; }
        public int j04ID { get; set; }
        
        public string j04Name { get; set; }
        
        public string p28Name { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit e-mail adresa!")]
        public string j02Email { get; set; }

        
        public string j02Login { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit jméno!")]
        public string j02FirstName { get; set; }   
        [Required(ErrorMessage ="Chybí vyplnit příjmení!")]
        public string j02LastName { get; set; }
        public string j02TitleBeforeName { get; set; }
        public string j02TitleAfterName { get; set; }

        public string j02Tel1 { get; set; }
        public string j02Tel2 { get; set; }

        public string j02PasswordHash { get; set; }
        public int j02AccessFailedCount { get; set; }

    }
}
