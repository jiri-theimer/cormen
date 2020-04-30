using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class MyDateViewModel
    {
        
        public string StringValue { get; set; }

        public DateTime? SelectedDate {
            get
            {
                if (string.IsNullOrEmpty(StringValue) == false)
                {
                    return DateTime.ParseExact(StringValue, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);

                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    StringValue = "";
                }
                else
                {
                    StringValue =Convert.ToDateTime(value).ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                }
            }
         }
    }
}
