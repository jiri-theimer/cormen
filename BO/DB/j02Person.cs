using System;
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
        public int j02ID_Owner { get; set; }

        

        [Required(ErrorMessage = "Chybí vyplnit e-mail adresa!")]
        public string j02Email { get; set; }        
       
        [Required(ErrorMessage = "Chybí vyplnit jméno!")]
        public string j02FirstName { get; set; }   
        [Required(ErrorMessage ="Chybí vyplnit příjmení!")]
        public string j02LastName { get; set; }
        public string j02TitleBeforeName { get; set; }
        public string j02TitleAfterName { get; set; }

        public string j02Tel1 { get; set; }
        public string j02Tel2 { get; set; }


        ///readonly účely:        
        public string p28Name;
        public int j03ID;
        public string j03Login;
        public string j04Name;
        public string RecordOwner;

    }
}
