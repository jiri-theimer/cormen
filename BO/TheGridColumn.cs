using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class TheGridColumn
    {
        private string _CssClass;
        private string _Entity;
        private string _Field;
        private string _Prefix;
        public bool IsTimestamp;
        
        public string FieldType { get; set; }   //string, bool, int, num, date, datetime
        public string Header { get; set; }
        public string SqlSyntax { get; set; }
        public int DefaultColumnFlag { get; set; }  //1 - default grid sloupec i combo sloupec, 2 - pouze default grid sloupec
        
        public bool IsSortable { get; set; } = true;
        public bool IsFilterable { get; set; } = true;
        public bool IsShowTotals { get; set; }
        public int FixedWidth { get; set; }
        public string EntityAlias { get; set; }

        public string RelName { get; set; } //název relace ve from klauzuly - naplní se v getSelectedPallete
        public string RelSql { get; set; }  //sql relace from klauzule - naplní se v getSelectedPallete

        public string Entity
        {
            get
            {
                return _Entity;
            }
            set
            {
                _Entity = value;

                _Prefix = _Entity.Substring(0, 3);
            }
        }
        public string Field
        {
            get
            {
                return _Field;
            }
            set
            {
                _Field = value;
               
            }
        }
       

        public string CssClass
        {
            get
            {
                if (_CssClass == null)
                {
                    if (FieldType == "num" || FieldType == "num0" || FieldType=="num3") _CssClass = "tdn";
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

        public string NormalizedTypeName
        {
            get
            {
                if (FieldType == "num0" || FieldType == "num" || FieldType=="num3") return "num";
                if (FieldType == "date" || FieldType == "datetime") return "date";
                return this.FieldType;
            }
            
        }
        
        public string Prefix
        {
            get
            {
                return _Prefix;
            }
        }

        public string getFinalSqlSyntax_SELECT(string strContextTablePrefix)
        {            
            
            if (this.SqlSyntax == null)
            {
                if (this.RelName != null)
                {
                    return this.RelName+"."+this.Field + " AS " + this.Field;   //v RelName je uložený název relace GRIDU
                }
                if (_Prefix == strContextTablePrefix)
                {
                    return "a." + this.Field+" AS "+this.Field;    //pole z primární tabulky strPrimaryTablePrefix
                }
                else
                {
                    return _Prefix + "." + this.Field+" AS "+this.Field;
                }
            }
            else
            {
                if (this.RelName != null)
                {
                    if (this.SqlSyntax.IndexOf("a.") > -1)
                    {
                        return this.SqlSyntax.Replace("a.", this.RelName + ".") + " AS " + this.Field;
                    }
                    return this.SqlSyntax + " AS " + this.Field;
                }

                if (_Prefix == strContextTablePrefix)
                {
                    return this.SqlSyntax + " AS " + this.Field;
                }
                else
                {
                    if (this.SqlSyntax.IndexOf("a.") > -1)
                    {
                        return this.SqlSyntax.Replace("a.", _Prefix + ".") + " AS " + this.Field;
                    }
                    return this.SqlSyntax + " AS " + this.Field;
                }
             

            }
            
        }
        public string getFinalSqlSyntax_WHERE(string strContextTablePrefix)
        {
            if (this.SqlSyntax == null)
            {
                if (_Prefix == strContextTablePrefix || _Prefix == "")
                {
                    return "a." + this.Field;    //pole z primární tabulky strPrimaryTablePrefix
                }
                else
                {
                    return _Prefix + "." + this.Field;
                }
            }
            else
            {
                if (this.IsTimestamp)
                {
                    if (_Prefix == strContextTablePrefix)
                    {
                        return this.SqlSyntax;
                    }
                    else
                    {
                        return this.SqlSyntax.Replace("a.", _Prefix + ".");
                    }
                }
                else
                {
                    return this.SqlSyntax;
                }
                
            }

        }
        public string getFinalSqlSyntax_ORDERBY(string strContextTablePrefix)
        {
            return getFinalSqlSyntax_WHERE(strContextTablePrefix);  //SQL pro ORDERBY je stejná jako WHERE
        }


        public string getFinalSqlSyntax_SUM(string strContextTablePrefix)
        {
            if (this.IsShowTotals == false) return "NULL as " + Field;
            if (this.SqlSyntax == null)
            {
                if (_Prefix == strContextTablePrefix)
                {
                    return "SUM(a." + this.Field + ") AS " + this.Field;    //pole z primární tabulky strPrimaryTablePrefix
                }
                else
                {
                    return "SUM("+_Prefix + "." + this.Field + ") AS " + this.Field;
                }
            }
            else
            {
                return "SUM("+this.SqlSyntax + ") AS " + this.Field;
            }

        }

        public string getSymbol()
        {
            switch (this.FieldType)
            {
                case "num":
                    return "0.0";
                case "num0":
                    return "000";
                case "date":
                    return "&#128197;";
                case "datetime":
                    return "&#128197;";
                case "bool":
                    return "&#10004;";
                default:
                    return "ab";
            }
        }

       
    }
}
