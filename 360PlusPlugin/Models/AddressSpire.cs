using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace _360PlusPlugin.Models
{
    [DataContract]
    public class AddressSpire
    {
        [DataMember]
        public string type
        {
            get; set;
        }
        [DataMember]
        public string shipId
        {
            get; set;
        }
        [DataMember]
        public string linkTable
        {
            get; set;
        }
        [DataMember]
        public string linkNo
        {
            get; set;
        }
        [DataMember]
        public string name
        {
            get; set;
        }
        [DataMember]
        public string streetAddress
        {
            get; set;
        }
        [DataMember]
        public string line1
        {
            get; set;
        }
        [DataMember]
        public string line2
        {
            get; set;
        }
        [DataMember]
        public string line3
        {
            get; set;
        }
        [DataMember]
        public string city
        {
            get; set;
        }
        [DataMember]
        public string postalCode
        {
            get; set;
        }
        [DataMember]
        public string provState
        {
            get; set;
        }
        [DataMember]
        public string country
        {
            get; set;
        }
        [DataMember]
        public PhoneSpire phone;
        [DataMember]
        public PhoneSpire fax;
        [DataMember]
        public string email
        {
            get; set;
        }
        [DataMember]
        public string website
        {
            get; set;
        }
        [DataMember]
        public string defaultWarehouse
        {
            get; set;
        }
        public string shipCode
        {
            get; set;
        }
        public string shipDescription
        {
            get; set;
        }

        public TerritorySpire territory;

        public string sellLevel
        {
            get; set;
        }

    }
}
