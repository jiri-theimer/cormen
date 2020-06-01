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

        private System.Text.StringBuilder _sb;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            _sb = new System.Text.StringBuilder();
            string strSelectedValues = this.For.Model as string;

            _sb.AppendLine("<div class='input-group'>");

            _sb.AppendLine("<div class='input-group-prepend'>");
            _sb.AppendLine(string.Format("<input type='hidden' id='{0}' name='{0}' value='{1}' />", this.For.Name, strSelectedValues));
            _sb.AppendLine(string.Format("<input type='text' class='form-control bg-light' id='TagNames' name='TagNames' value='{0}' />", this.SelectedTagNames));
            _sb.AppendLine("</div>");

            _sb.AppendLine("<button type='button' class='btn btn-light'>...</button>");

            _sb.AppendLine("</div>");

            output.Content.AppendHtml(_sb.ToString());
        }
    }
}
