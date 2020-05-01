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
    [HtmlTargetElement("myautocomplete")]
    public class myAutoCompleteTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("placeholder")]
        public string PlaceHolder { get; set; }
        [HtmlAttributeName("o15flag")]
        public string o15flag { get; set; }

        private System.Text.StringBuilder _sb;
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            output.TagMode = TagMode.StartTagAndEndTag;


            _sb = new System.Text.StringBuilder();
            var strControlID = this.For.Name.Replace(".", "_");

            
            sb(string.Format("<div class='input-group' id='{0}_Dropdown'>",strControlID));
            sb("<div class='input-group-prepend'>");
            //sb(string.Format("<div id='{0}_Dropdown' class='dropdown input-group'>", strControlID));
            sb(string.Format("<button class='btn btn-light dropdown-toggle' type='button' id='{0}_cmdCombo' tabindex='-1' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false' style='text-align:left;'>",strControlID));
            sb("</button>");
            sb(string.Format("<div class='dropdown-menu' aria-labelledby='{0}_cmdCombo' >", strControlID));
                sb(string.Format("<input type='text' id='{0}_search' class='form-control' placeholder='Najít...' autocomplete='off' />", strControlID));
                sb(string.Format("<div id='{0}_divtab' style='height:200px;overflow:auto;'></div>", strControlID));
            sb("</div>");
            sb("</div>");

            sb(string.Format("<input id='{0}' class='form-control' placeholder='{1}' value='{2}' name='{3}' />", strControlID, this.PlaceHolder,this.For.Model,this.For.Name));
            sb("</div>");

           
            sb("");
            sb("");


            sb("<script type='text/javascript'>");
            _sb.Append(string.Format("var c{0}=", strControlID));
            _sb.Append("{");
            _sb.Append(string.Format("controlid: '{0}',posturl: '/Common/GetAutoCompleteHtmlItems',o15flag:'{1}'", strControlID,this.o15flag));
            _sb.Append("};");

            sb("");
            sb(string.Format("myautocomplete_init(c{0});", strControlID));

            sb("");
            sb("</script>");

            output.Content.AppendHtml(_sb.ToString());






        }



        private void sb(string s)
        {
            _sb.AppendLine(s);

        }
    }
}
