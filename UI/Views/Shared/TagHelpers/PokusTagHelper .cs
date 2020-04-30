using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

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


        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }
        public ModelExpression radio { get; set; }

        
       
        public PokusTagHelpder(BO.RunningUser ru)
        {
            _ru = ru;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            //output.TagName = "Pokus";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.SuppressOutput();


            var sb = new System.Text.StringBuilder();
            sb.Append("<input type='text' for-id='"+ this.For.Name.Replace(".", "_")+"' class='form-control' onchange='pokus(this)' placeholder='For???' value='" + this.For.Model + "'/>");
            sb.Append("<input type='text' value ='"+this.For.Model+ "' id ='" + this.For.Name.Replace(".","_") + "' name ='" + this.For.Name + "'/>");



            sb.Append("<hr>Vyplňte radio:");
            sb.Append("<input type='text' for-id='" + this.radio.Name.Replace(".", "_") + "' onchange='pokus(this)' class='form-control' value='" + this.radio.Model + "'/>");
            sb.Append("<input type='hidden' value ='" + this.radio.Model + "' id ='" + this.radio.Name.Replace(".", "_") + "' name ='" + this.radio.Name + "'/>");

            sb.AppendFormat("<span>jméno: {0}</span> <br/>", this.Name.Model);
            sb.AppendFormat("<span>kód: {0}</span>", this.Designation.Model);
            sb.Append("<br>Zpracoval: " + _ru.j03Login);
            sb.Append("<hr>");
            
                
            output.Content.AppendHtml(sb.ToString());
           // output.PreContent.SetHtmlContent(sb.ToString());

        }


        
    }
}
