using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace UI.Views.Shared.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("mynumber")]
    public class myNumberTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }


        private const string ForDecimalDigits = "decimal-digits";
        [HtmlAttributeName(ForDecimalDigits)]
        public int DecimalDigits { get; set; } = 2;

        private string _StringValue { get; set; } //tvar hodnoty čísla pro jeho uložení na hostitelské view
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            var sb = new System.Text.StringBuilder();
            
            
            string strStep = "0.01";
            string strPlaceHolder = "0,00";
            switch (this.DecimalDigits)
            {
                case 0:
                    strStep = "";
                    strPlaceHolder = "000";
                    break;
                case 1:
                    strStep = "0.1";
                    strPlaceHolder = "0,0";
                    break;
                case 2:
                    strStep = "0.01";
                    strPlaceHolder = "0,00";
                    break;
                case 3:
                    strStep = "0.001";
                    strPlaceHolder = "0,000";
                    break;
                default:
                    strStep = "any";
                    strPlaceHolder = "0,00";                    
                    break;
            }
            if ( this.For.Model != null)
            {
                _StringValue = this.For.Model.ToString().Replace(".",",");
                //StringFormatValue= String.Format("{0:##.##}", this.For.Model);
            }
            else
            {
                _StringValue = "0";
                
            }

            sb.Append(string.Format("<input type='text' for-id='{0}' class='form-control' step='{1}' placeholder='{2}' onfocus='mynumber_focus(this)' onblur='mynumber_blur(this,{3})' value='{4}'/>", this.For.Name.Replace(".", "_"),strStep,strPlaceHolder,DecimalDigits, _StringValue));
            sb.Append(string.Format("<input type='hidden' value ='{0}' id ='{1}' name ='{2}'/>", _StringValue, this.For.Name.Replace(".", "_"), this.For.Name));

            output.Content.AppendHtml(sb.ToString());
        }
    }
}
