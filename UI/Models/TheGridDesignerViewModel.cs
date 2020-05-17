﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridDesignerViewModel:BaseViewModel
    {
        public BO.j72TheGridState Rec { get; set; }
        public IEnumerable<BO.TheGridColumn> ApplicableCollumns;
        public List<BO.TheGridColumn> SelectedColumns;


        public List<BO.EntityRelation> Relations;
        public IEnumerable<BO.TheGridColumn> AllColumns;

    }
}
