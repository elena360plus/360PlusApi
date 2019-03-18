using System;
using System.Collections.Generic;

namespace _360PlusPlugin.Models
{

    public class InventoryCRM
    {
        public int Count { get; set; }
        public int Start { get; set; }
        public int Limit { get; set; }
		
        public List<RecordCRM> Records { get; set; }
    }

}
