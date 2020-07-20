﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using System.Collections;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Data.SqlClient;

namespace UI.Views.Shared.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("mydropdown")]
    public class myDropdownTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("valuefield")]
        public string ValueField { get; set; }  //může být i string
        [HtmlAttributeName("textfield")]
        public string TextField { get; set; }

        [HtmlAttributeName("isfirstemptyrow")]
        public bool IsFirstEmptyRow { get; set; }

        [HtmlAttributeName("event_after_changevalue")]
        public string Event_After_ChangeValue { get; set; }

        [HtmlAttributeName("groupfield")]
        public string GroupField { get; set; }

        [HtmlAttributeName("datavalue")]
        public string DataValue { get; set; }

        [HtmlAttributeName("datasource")]
        public ModelExpression DataSource { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IEnumerable lisDatasource = this.DataSource.Model as IEnumerable;
            if (lisDatasource == null)
            {
                return;
            }

            string strSelectedValue = "";
            string strLastGroup = "";
            

            if (this.For.Model != null)
            {
                strSelectedValue=Convert.ToString(this.For.Model);
            }
            var sb = new System.Text.StringBuilder();

            var strControlID = this.For.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_");
            
            sb.AppendLine(string.Format("<select class='form-control' id='{0}' name='{1}'",strControlID, this.For.Name));
            if (this.Event_After_ChangeValue != null)
            {
                sb.Append(string.Format(" onchange='{0}(this)'", this.Event_After_ChangeValue));
            }
            if (this.DataValue != null)
            {
                sb.Append(string.Format(" data-value='{0}'",this.DataValue));
            }
            sb.Append(">");
            if (this.IsFirstEmptyRow)
            {
                sb.AppendLine("<option></option>");
            }
            foreach (var item in lisDatasource)
            {
                string strGroup = "";
                if (this.GroupField != null)
                {
                    if (DataSource.Metadata.ElementMetadata.Properties[this.GroupField].PropertyGetter(item) == null)
                    {
                        strGroup = "";
                    }
                    else
                    {
                        strGroup = DataSource.Metadata.ElementMetadata.Properties[this.GroupField].PropertyGetter(item).ToString();
                    }

                    if (strGroup != strLastGroup)
                    {
                        sb.AppendLine(string.Format("<option disabled style='background-color:silver;'>{0}</option>",strGroup));

                    }
                }

                string strText = DataSource.Metadata.ElementMetadata.Properties[this.TextField].PropertyGetter(item).ToString();
                string strValue = Convert.ToString(DataSource.Metadata.ElementMetadata.Properties[this.ValueField].PropertyGetter(item));
                if (strSelectedValue == strValue)
                {
                    sb.Append(string.Format("<option value='{0}' selected>{1}</option>", strValue,strText));
                }
                else
                {
                    sb.Append(string.Format("<option value='{0}'>{1}</option>", strValue, strText));
                }

                strLastGroup = strGroup;                
            }

            sb.AppendLine("</select>");
            output.Content.AppendHtml(sb.ToString());

        }
    }
}
