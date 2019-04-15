using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace _360PlusPlugin.Models
{
    [DataContract]
   public class ShipToForOrderSpire
    {
        [DataMember]
        public int id
        {
            get; set;
        }
        [DataMember]
        public AddressSpire FullAdress
        {
            get; set;
        }


    }
}
