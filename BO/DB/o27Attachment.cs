using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class o27Attachment:BaseBO
    {
        [Key]
        public int o27ID { get; set; }    
        public int o23ID { get; set; }
        public string o27Name { get; set; }
        public string o27ArchiveFileName { get; set; }
        public string o27ArchiveFolder { get; set; }
        public int o27FileSize { get; set; }
        public string o27ContentType { get; set; }
        public string o27GUID { get; set; }

    }
}
