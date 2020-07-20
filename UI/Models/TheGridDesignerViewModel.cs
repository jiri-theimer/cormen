﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridDesignerViewModel:BaseViewModel
    {
        public BO.j72TheGridState Rec { get; set; }

        public List<BO.TheGridColumn> SelectedColumns;


        public List<BO.EntityRelation> Relations;
        public List<BO.TheGridColumn> AllColumns;

        public List<BO.j73TheGridQuery> lisJ73 { get; set; }

        public List<BO.TheQueryField> lisQueryFields { get; set; }
        public List<BO.ThePeriod> lisPeriods { get; set; }


    }
}
