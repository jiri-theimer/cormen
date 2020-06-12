using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum b02StartFlagENUM
    {
        _None=0,
       DefaultStatus=1
    }
    public enum b02MoveFlagENUM
    {
        _None = 0,
        User = 1,
        System=2,
        
    }
    public class b02Status: BaseBO
    {
        [Key]
        public int b02ID { get; set; }
        
        [Required(ErrorMessage = "Chybí vyplnit název!")]
        public string b02Name { get; set; }
        public int b02Ordinary { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit druh entity!")]
        public string b02Entity { get; set; }
        public string b02Code { get; set; }
        public string b02Memo { get; set; }

        public b02StartFlagENUM b02StartFlag { get; set; }
        public b02MoveFlagENUM b02MoveFlag { get; set; }
        public string b02MoveBySql { get; set; }
    }
}
