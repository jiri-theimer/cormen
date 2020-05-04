﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j72TheGridState : BaseBO
    {
        [Key]
        public int j72ID { get; set; }
        public int j03ID { get; set; }
        public int j70ID { get; set; }
        public string j72Entity { get; set; }
        public string j72MasterEntity { get; set; }
        public int j72MasterPID { get; set; }       //pouze pro průběžnou práci -> neukládá se do db, byť v sloupec existuje
        public int j72ContextMenuFlag { get; set; }   //pouze pro průběžnou práci

        [Required(ErrorMessage ="Grid musí obsahovat minimálně jeden sloupec.")]
        public string j72Columns { get; set; }
        public string j72SortDataField { get; set; }
        public string j72SortOrder { get; set; }
        public int j72PageSize { get; set; }
        public int j72CurrentPagerIndex { get; set; }
        public int j72CurrentRecordPid { get; set; }
        public bool j72IsNoWrap { get; set; }
        public string j72Filter { get; set; }
        public string j72ColumnsGridWidth { get; set; }
        public string j72ColumnsReportWidth { get; set; }
        public int j72SplitterFlag { get; set; }
        public int j72HeightPanel1 { get; set; }
        public int j72SelectableFlag { get; set; } = 1;

       
    }
}
