using System;
using System.Collections.Generic;

namespace _360PlusPlugin.Models
{
	
    public class PricingSpire
    {
        public string CurrMargin { get; set; }
        public string AvgMarginPct { get; set; }
        public string AvgMargin { get; set; }
        public string CurrMarginPct { get; set; }

        public List<string> SellPrice { get; set; }
    }

}
