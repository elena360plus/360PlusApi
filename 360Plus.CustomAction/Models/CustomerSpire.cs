
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
        [DataMember]
        public int? id
        {
            get; set;
        }
        [DataMember]
        public string code
        {
            get; set;
        }

        [DataMember]
        public string customerNo
        {
            get; set;
        }

       
    }
}
