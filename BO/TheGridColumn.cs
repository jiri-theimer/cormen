﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class TheGridColumn
    {
        private string _CssClass;

        public string Entity { get; set; }
        public string Field { get; set; }
        public string FieldType { get; set; }   //string, bool, int, num, date, datetime
        public string Header { get; set; }
        public string SqlSyntax { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSortable { get; set; } = true;
        public bool IsFilterable { get; set; } = true;
        public bool IsShowTotals { get; set; }
        public int FixedWidth { get; set; }
        
        public string CssClass
        {
            get
            {
                if (_CssClass == null)
                {
                    if (FieldType == "num0" || FieldType == "num") _CssClass = "tdn";
                    if (FieldType == "bool") _CssClass = "tdb";
                }                
                return _CssClass;
            }
            set
            {
                _CssClass = value;
            }
        }
        public string UniqueName
        {
            get
            {
                return Entity + "__" + Field;
            }
        }
        public string FinalSqlSyntax
        {
            get
            {
                if (SqlSyntax == null)
                {
                    return "a." + Field;
                }
                else
                {
                    return SqlSyntax + " AS " + Field;
                }
            }
        }
        public string FinalSqlSyntaxOrderBy
        {
            get
            {
                if (SqlSyntax == null)
                {
                    return "a." + Field;
                }
                else
                {
                    return SqlSyntax;
                }
            }
        }
        public string FinalSqlSyntaxSum
        {
            get
            {
                if (IsShowTotals == false) return "NULL as "+ Field;
                if (SqlSyntax == null)
                {
                    return "SUM(a." + Field+") as "+Field;
                }
                else
                {
                    return "SUM("+SqlSyntax + ") AS " + Field;
                }
            }
        }

        public string ColumnWidthPixels
        {
            get
            {
                if (this.FixedWidth == 0)
                {
                    return "auto";
                }
                else
                {
                    return this.FixedWidth.ToString() + "px";
                }
            }
        }

    }
}