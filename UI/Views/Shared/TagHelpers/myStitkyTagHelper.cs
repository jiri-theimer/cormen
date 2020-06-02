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
    [HtmlTargetElement("mystitky")]
    public class myStitkyTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("entity")]
        public string Entity { get; set; }

        [HtmlAttributeName("tagnames")]
        public string SelectedTagNames { get; set; }

        [HtmlAttributeName("taghtml")]
        public string SelectedTagHtml { get; set; }

        private System.Text.StringBuilder _sb;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            _sb = new System.Text.StringBuilder();
            string strSelectedValues = this.For.Model as string;

            _sb.AppendLine("<div class='input-group' style='width:100%;'>");

            
            _sb.AppendLine(string.Format("<input type='hidden' id='TagPids' name='TagPids' value='{0}' />", strSelectedValues));
            _sb.AppendLine(string.Format("<input type='text' readonly class='form-control bg-light' tabindex='-1' id='TagNames' name='TagNames' value='{0}' />", this.SelectedTagNames));

            _sb.AppendLine("<div class='input-group-append'>");
            _sb.AppendLine(string.Format("<button type='button' class='btn btn-secondary' onclick='mystitky_multiselect(event,\"{0}\")'>...</button>", this.Entity));
            _sb.AppendLine("</div>");

            _sb.AppendLine(string.Format("<input type='hidden' id='TagHtml' name='TagHtml' value=\"{0}\" />", this.SelectedTagHtml));            //.Replace("<","||").Replace(">","##")

            _sb.AppendLine("</div>");
            _sb.Append("<div id='divTagHtml' class='input-group'>");
            if (string.IsNullOrEmpty(this.SelectedTagHtml) == false)
            {                
                _sb.Append(this.SelectedTagHtml);       //.Replace("||","<").Replace("##",">")         
            }
            _sb.Append("</div>");

            output.Content.AppendHtml(_sb.ToString());
        }
    }
}
