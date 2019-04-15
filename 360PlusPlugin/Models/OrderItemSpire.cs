using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace _360PlusPlugin.Models
{
    [DataContract]
    public class OrderItemSpire
    {
        [DataMember]
        public int? sequence
           {
                get; set;
            }
        [DataMember]
        public ItemsSpire inventory
          {
                get; set;
            }

        [DataMember]
        public string partNo
        {
            get; set;
        }

        [DataMember]
        public string orderQty
        {
            get; set;
        }


    }
}
