using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _360PlusPlugin.Models
{
    public class CustomerSpire
    {
        //  public int id { get; set; }
        public string customerNo
        {
            get; set;
        }
        public string name
        {
            get; set;
        }
        public string code
        {
            get; set;
        }
        public bool hold
        {
            get; set;
        }
        public bool? applyFinanceCharges
        {
            get; set;
        }
        public int? creditType
        {
            get; set;
        }
        public decimal? creditLimit
        {
            get; set;
        }
        public string currency
        {
            get; set;
        }
        public string reference
        {
            get; set;
        }

        public AddressSpire address;
    }
}
