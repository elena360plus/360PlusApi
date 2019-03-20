using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace _360PlusPlugin.Models
{

    [DataContract]
        public class CustomerSpire
    {
        //  public int id { get; set; }
        [DataMember]
        public string customerNo
        {
            get; set;
        }
        [DataMember]
        public string name
        {
            get; set;
        }
        [DataMember]
        public string code
        {
            get; set;
        }
        [DataMember]
        public bool hold
        {
            get; set;
        }
        [DataMember]
        public bool? applyFinanceCharges
        {
            get; set;
        }
        [DataMember]
        public int? creditType
        {
            get; set;
        }
        [DataMember]
        public decimal? creditLimit
        {
            get; set;
        }
        [DataMember]
        public string currency
        {
            get; set;
         }
            [DataMember]
        public string reference
        {
            get; set;
        }
        [DataMember]
        public AddressSpire address;
    }
}
