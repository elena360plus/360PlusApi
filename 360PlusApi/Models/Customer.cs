using _360PlusPlugin.Models;
using ApiTest;
using System.Collections.Generic;


namespace PlusApi.Models

{
   
    public class CustomerClient : BaseObjectClient<CustomerSpire>
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

    public class CustomerClientAddress : BaseObjectClient<AddressSpire>
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
