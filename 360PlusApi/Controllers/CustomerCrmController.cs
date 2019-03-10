using PlusApi.Models;
using ApiTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Configuration;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;
//using System.Web.Script.Serialization;

namespace PlusApi.Controllers
{


    [EnableCors(origins: "*", headers: "*", methods: "GET, POST, PUT, DELETE, OPTIONS")]
    public class CustomerCRMController : ApiController
    {
       // private CustomerCrm customers = new CustomerCrm()
       //{
       //             customerNo = "111", 
       //        name = "Haleemah Redfern"
                    
       //};

        public static string SendRequest(string uri, CustomerCrm jsonContent)
        {
            string NewID = String.Empty;
            string Company = String.Empty;
            string UserName = String.Empty;
            string Password = String.Empty;
            string SpireBaseURL = String.Empty;
            try
            {
                    SpireBaseURL = ConfigurationManager.AppSettings["SpireBaseURL"];
                    Company = ConfigurationManager.AppSettings["Company"];
                    UserName = ConfigurationManager.AppSettings["UserName"];
                    Password = ConfigurationManager.AppSettings["Password"];

               if(String.IsNullOrEmpty(SpireBaseURL) || String.IsNullOrEmpty(Company)) return "The appSettings section is empty.Write first.";

                //var clientS = new ApiClient("inspire2_10", "SPIRE", "12345");
                var clientS = new ApiClient(Company, UserName, Password, SpireBaseURL);
                var customerClient = new CustomerClient(clientS);
                var customer = jsonContent;
                NewID = customerClient.Create(customer);
                

            }

            catch (ApiException e)
            {
                Console.Error.WriteLine(e.Message);
                return e.Message;
            }


            return NewID;
            //using (var webClient = new WebClient())
            //{
            //    webClient.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
            //    return  webClient.UploadString(new Uri(api), "POST", jsonContent);
            //}
        }

      
        [HttpPost]
        public HttpResponseMessage PostMethod([FromBody]JObject _Customer)
        {
            string resultCustomer = String.Empty;
            string CountryThreeLetters = String.Empty;

            // CustomerCrm customerCRM =  (CustomerCrm) _Customer.GetType().GetProperty("customer").GetValue(_Customer, null);

           AddressCRM addressCRM = _Customer.GetValue("addressAPI").ToObject<AddressCRM>();
            if (addressCRM.country != null && !String.IsNullOrEmpty(addressCRM.country.ToString()))
            {
                CountryThreeLetters = ConvertCountryNameToThreeLetterName(addressCRM.country.ToString());
                addressCRM.country = CountryThreeLetters;
            }

            PhoneCRM  PhoneCRM = _Customer.GetValue("addressAPI")["phoneAPI"].ToObject<PhoneCRM>();
            FaxCRM FaxCRM = _Customer.GetValue("addressAPI")["faxAPI"].ToObject<FaxCRM>();
            TerritoryCRM TerritoryCRM  = _Customer.GetValue("addressAPI")["territoryAPI"].ToObject<TerritoryCRM>();

            CustomerCrm customerCRM = _Customer.GetValue("customer").ToObject<CustomerCrm>();

            customerCRM.address = addressCRM;
            customerCRM.address.phone = PhoneCRM;
            customerCRM.address.fax = FaxCRM;
            customerCRM.address.territory = TerritoryCRM;
            try
            {
                string url = string.Empty;
                // string objJson = Customer; //
                // Customer   objJson    =   new JavaScriptSerializer().Serialize(Customer);
                resultCustomer = SendRequest(url, customerCRM);
                 

            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "Server error!");
            }

            return Request.CreateResponse(HttpStatusCode.Created, resultCustomer);


        }

        //[HttpGet]
        //public IEnumerable<Customer> getCustomer()
        //{
        //    return customers;
        //}


        //[HttpGet]
        //public IEnumerable<Employee> getEmployeeById(int id)
        //{
        //    var Emp = from emp in getEmployees()
        //              where emp.EmployeeId.Equals(id)
        //              select emp;

        //    return Emp.ToList<Employee>();
        //}



        //public HttpResponseMessage Post([FromBody] Employee value)
        //{
        //    //Employee pp = new Employee();
        //    //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
        //    //response.Headers.Location = new Uri(Request.RequestUri, String.Format("companies/{0}/", id));
        //    //return response;
        //}


        public static string ConvertCountryNameToThreeLetterName(string CountryEnglishName)
        {
            if (CountryEnglishName == null) return null;
          

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in cultures)
            {
                RegionInfo region = new RegionInfo(culture.LCID);
                if (region.EnglishName.ToUpper() == CountryEnglishName.ToUpper())
                {
                    return region.ThreeLetterISORegionName;
                }
            }

            return null;
        }



    }

  
}
