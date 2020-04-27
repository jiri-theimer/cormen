﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Dapper;

namespace BL.DL
{
    public class FinalSqlCommand
    {
        public string FinalSql { get; set; }
        public string FinalTotalsRowSql { get; set; }
        public string SqlWhere { get; set; }
        public DynamicParameters Parameters { get; set; }
        public List<DL.Param4DT> Parameters4DT { get; set; }
    }
}
