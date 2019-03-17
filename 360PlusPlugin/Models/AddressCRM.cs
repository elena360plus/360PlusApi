using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _360PlusPlugin.Models
{
    public class AddressCRM
    {
        public string type
        {
            get; set;
        }
        public string linkTable
        {
            get; set;
        }
        public string linkNo
        {
            get; set;
        }
        public string name
        {
            get; set;
        }
        public string streetAddress
        {
            get; set;
        }

        public string line1
        {
            get; set;
        }
        public string line2
        {
            get; set;
        }
        public string line3
        {
            get; set;
        }
        public string city
        {
            get; set;
        }
        public string postalCode
        {
            get; set;
        }
        public string provState
        {
            get; set;
        }
        public string country
        {
            get; set;
        }

        public PhoneCRM phone;

        public PhoneCRM fax;
        public string email
        {
            get; set;
        }
        public string website
        {
            get; set;
        }
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

        public TerritoryCRM territory;

        public string sellLevel
        {
            get; set;
        }

    }
}
