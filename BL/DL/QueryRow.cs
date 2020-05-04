using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Dapper;

namespace BL.DL
{
    public class QueryRow
    {
        public string StringWhere { get; set; }
        public string ParName { get; set; }
        public object ParValue { get; set; }

        public string AndOrZleva { get; set; } = "AND";
    }

   
}
