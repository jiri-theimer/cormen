using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class o23Doc:BaseBO
    {
        [Key]
        public int o23ID { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit svázaný záznam!")]
        public int o23RecordPid { get; set; }
        [Required(ErrorMessage = "Chybí vyplnit druh entity!")]
        public string o23Entity { get; set; }
        public int o12ID { get; set; }
        public int b02ID { get; set; }
        public int j02ID_Owner { get; set; }

        [Required(ErrorMessage = "Chybí vyplnit název!")]
        [MaxLength(100, ErrorMessage = "Maximum 100 znaků")]
        public string o23Name { get; set; }
        [MaxLength(1000, ErrorMessage = "Maximum 1000 znaků")]
        public string o23Memo { get; set; }
        
        public string o23Code { get; set; }
        public DateTime? o23Date { get; set; }

        public string o12Name { get; set; }
        
        public string b02Name { get; set; }

        public string RecordPidAlias { get; set; }

        public string EntityAlias { get
            {
                return BAS.getEntityAlias(this.o23Entity);
            } }

        public string RecordOwner;
    }
}
