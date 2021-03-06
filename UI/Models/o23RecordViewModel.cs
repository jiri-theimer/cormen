﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class o23RecordViewModel:BaseViewModel
    {
        public BO.o23Doc Rec { get; set; }
        public string Guid { get; set; }
        public string o27IDs4Delete { get; set; }

        public string TagPids { get; set; }
        public string TagNames { get; set; }
        public string TagHtml { get; set; }

        public MyToolbarViewModel Toolbar { get; set; }

        public IEnumerable<BO.o27Attachment> lisO27;

    }
}
