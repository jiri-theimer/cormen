using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j04UserRole:BaseBO
    {
        [Key]
        public int j04ID { get; set; }

        [Required(ErrorMessage ="Název role je povinné pole")]
        public string j04Name { get; set; }
        public int j04PermissionValue { get; set; }
    }
}
