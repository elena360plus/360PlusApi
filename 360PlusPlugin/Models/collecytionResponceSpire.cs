using System;
using System.Collections.Generic;

namespace _360PlusPlugin.Models
{

    public class collecytionResponceSpire<T> where T : baseSpire
    {
        public int count
        {
            get; set;
        }
        public int start
        {
            get; set;
        }
        public int limit
        {
            get; set;
        }

        public List<T> records
        {
            get; set;
        }
    }
}
