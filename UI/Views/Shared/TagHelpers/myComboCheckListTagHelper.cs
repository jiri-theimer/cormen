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
    [HtmlTargetElement("mycombochecklist")]
    public class myComboCheckListTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("placeholder")]
        public string PlaceHolder { get; set; }
        

        private System.Text.StringBuilder _sb;
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;


            _sb = new System.Text.StringBuilder();
            string strSelectedValues = this.For.Model as string;


            var strControlID = this.For.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_");


            sb(string.Format("<div id='divDropdownContainer{0}' class='dropdown input-group' style='border:solid 1px #C8C8C8;border-radius:3px;width:100%;'>", strControlID));


            
            sb(string.Format("<input id='value_alias_{0}' type='text' class='form-control bg-light' readonly='readonly' placeholder='{1}'/>", strControlID,this.PlaceHolder));

            //sb("<div class='input-group-append'>");
            sb(string.Format("<button type='button' id='cmdCombo{0}' class='btn btn-secondary dropdown-toggle' style='border-radius:0px;' data-toggle='dropdown' aria-haspopup='true' aria-expanded='false'></button>", strControlID));

            sb(string.Format("<div id='divDropdown{0}' class='dropdown-menu' aria-labelledby='cmdCombo{0}' style='width:100%;' tabindex='-1'>", strControlID));
            sb(string.Format("<div id='divData{0}' style='height:220px;overflow:auto;z-index:500;width:100%;min-width:200px;'>", strControlID));
            sb("</div>");
            //sb("</div>");

            sb("</div>");
            
            sb("");            
            sb("");


            sb("</div>");


            sb("<script type='text/javascript'>");
            sb("");
            
            sb("$('#divDropdown"+strControlID+"').on('click.bs.dropdown', function (e) {");
            sb("e.stopPropagation();");
            sb("});");

            sb("$('#divDropdownContainer" + strControlID + "').on('show.bs.dropdown', function (e) {");
            sb("$('#divDropdownSelectedP27IDs').css('margin-left', 25 + $('#divDropdownSelectedP27IDs').width() * -1);");
            sb("});");
            sb("");
            sb("$('#value_alias_"+strControlID+"').click(function () { $('#cmdCombo"+strControlID+"').dropdown('toggle') });");

            sb("");
            sb("</script>");


            sb(string.Format("<input type='hidden' id='{0}' name='{0}' value=\"{1}\" />", this.For.Name, strSelectedValues));




            output.Content.AppendHtml(_sb.ToString());
        }

        private void sb(string s)
        {
            _sb.AppendLine(s);

        }
    }
}
