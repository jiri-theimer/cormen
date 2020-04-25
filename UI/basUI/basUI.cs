using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;

namespace UI
{
    public class basUI
    {
        public static string xxxDataTableToJSONWithJSONNet(System.Data.DataTable dt)
        {
            
            return JsonConvert.SerializeObject(dt, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include});




        }

       
       
    }

    
}
