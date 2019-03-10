using ApiTest;
using System.Collections.Generic;


namespace PlusApi.Models

{
   

    public class CustomerCrm
    {
      //  public int id { get; set; }
        public string customerNo { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public bool hold { get; set; }
        public bool? applyFinanceCharges { get; set; }
        public int? creditType { get; set; }
        public decimal? creditLimit { get; set; }
        public string currency { get; set; }
        public string reference { get; set; }

        public AddressCRM address;
    }
    public class AddressCRM
    {
        public string type { get; set; }
        public string linkTable { get; set; }
        public string linkNo { get; set; }
        public string name { get; set; }
        public string streetAddress { get; set; }

        public string line1 { get; set; }
        public string line2 { get; set; }
        public string line3 { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string provState { get; set; }
        public string country { get; set; }

        public PhoneCRM phone;
        public FaxCRM fax;
        public string email { get; set; }
        public string website { get; set; }
        public string defaultWarehouse { get; set; }


        public string shipCode { get; set; }
        public string shipDescription { get; set; }

        public TerritoryCRM territory;

        public string sellLevel { get; set; }
     
    }

    public class TerritoryCRM
    {
        public string code { get; set; }
        public string description { get; set; }

    }

    public class PhoneCRM
    {
        public string number { get; set; }
        public int? format { get; set; }

    }

    public class FaxCRM
    {
        public string number { get; set; }
        public int? format { get; set; }

    }

        public class CustomerClient : BaseObjectClient<CustomerCrm>
    {
        public CustomerClient(ApiClient client) : base(client) { }

        public override string Resource
        {
            get
            {
                return "customers/";
            }
        }
    }

    public class CustomerClientAddress : BaseObjectClient<AddressCRM>
    {
        public CustomerClientAddress(ApiClient client) : base(client) { }

        public override string Resource
        {
            get
            {
                return "customers/";
            }
        }
    }

}
