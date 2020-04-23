using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UI.Views.Shared.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project

    [HtmlTargetElement("pokus")]
    public class PokusTagHelpder : TagHelper
    {
        //[HtmlAttributeName("for-name")]
        public ModelExpression Name { get; set; }
        //[HtmlAttributeName("for-designation")]
        public ModelExpression Designation { get; set; }
        private readonly BO.RunningUser _ru;

        public PokusTagHelpder(BO.RunningUser ru)
        {
            _ru = ru;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            //output.TagName = "Pokus";
            output.TagMode = TagMode.StartTagAndEndTag;

            var sb = new System.Text.StringBuilder();
            sb.Append("<hr>");
            sb.Append("<input type='text' class='form-control' value='" + this.Name.Model + "'/>");
            sb.AppendFormat("<span>jméno: {0}</span> <br/>", this.Name.Model);
            sb.AppendFormat("<span>kód: {0}</span>", this.Designation.Model);
            sb.Append("<br>Zpracoval: " + _ru.j02Login);
            sb.Append("<hr>");
            output.Content.AppendHtml(sb.ToString());
           // output.PreContent.SetHtmlContent(sb.ToString());

        }

        
    }
}
