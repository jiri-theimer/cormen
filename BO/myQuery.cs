﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class myQuery
    {
        public string Entity;
        public List<int> pids;


        public bool? IsRecordValid;

        public int j04id;
        public int b02id;
        public int p28id;
        public string query_by_entity_prefix;

        public string SearchString;
       
        public myQuery(string strEntity)
        {
            this.Entity = strEntity;
        }
    }
}
