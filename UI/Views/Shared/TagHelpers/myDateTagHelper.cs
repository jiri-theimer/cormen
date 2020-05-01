using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UI.Views.Shared.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("mydate")]
    public class myDateTagHelper : TagHelper
    {        
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        private string _StringValue { get; set; } //tvar hodnoty čísla pro jeho uložení na hostitelské view


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;

            if (this.For.Model != null)
            {                
                _StringValue=Convert.ToDateTime(this.For.Model).ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                _StringValue = "";
            }

            
            var sb = new System.Text.StringBuilder();
            var strControlID = this.For.Name.Replace(".", "_");
            sb.AppendLine("<div class='input-group' style='width:100%;'>");
            sb.Append(string.Format("<input type='text' id='{0}' for-id='{1}' class='form-control' placeholder='dd.mm.yyyy' autocomplete='off' value='{2}' onchange='datepicker_change(this)'/>", strControlID+"helper", strControlID, _StringValue));
                       
            sb.Append("<div class='input-group-append'>");
            sb.Append(string.Format("<button id='{0}cmd' type='button' class='btn btn-outline-secondary px-1 py-0' tabindex='-1' style='font-size:14px;' onclick='datepicker_button_click({1})'>&#128197;</button>", strControlID,"\""+strControlID+"helper"+"\""));
            sb.Append("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine(string.Format("<input type='hidden' value ='{0}' id='{1}' name='{2}'/>", _StringValue, strControlID, this.For.Name));

            sb.AppendLine("");


            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("$(document).ready(function () {");

            sb.AppendLine(string.Format("datepicker_init('{0}');", strControlID+"helper"));
            sb.AppendLine("");

            sb.AppendLine("});");
            sb.AppendLine("</script>");


            output.Content.AppendHtml(sb.ToString());
        }
    }
}
