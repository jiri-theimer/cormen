using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;

namespace UI
{
    public class DATA
    {
        public static string DataTableToJSONWithJSONNet(System.Data.DataTable dt)
        {
            string s= JsonConvert.SerializeObject(dt, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include }).Replace(":null"," ");
            
            return s;


        }
    }
}
