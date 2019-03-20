using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using xrm = SpirePlusPlugin;
using System.Net;
using _360PlusPlugin.Models;
using System.Runtime.Serialization.Json;
using System.IO;
using Spire_BusinessEntities;

namespace  Spire_BusinessEntities
{
    public class SpireHelper : Spire_Xrm_Base
    {
        public Stream SerializememoryStream { get; private set; }


        IOrganizationService service;

        #region Constructors

        public SpireHelper(xrm.XrmServiceContext ctx, string PrimaryEntityName, Guid PrimaryEntityId)
        {
            base.Init(ctx, PrimaryEntityName, PrimaryEntityId);

        }

        public SpireHelper(xrm.XrmServiceContext ctx)
        {
            base.Init(ctx);
        }

        #endregion

      


        #region Spire Methods

        public string SpirePostMethod_Account( Guid _AccountId, IOrganizationService _service)
        {
            service = _service;
            string Error = String.Empty;
            string resultPostCustomer = String.Empty;

            var newAccount = (from ac in ctx.AccountSet where ac.AccountId == _AccountId select ac).FirstOrDefault();
            if (newAccount == null) return "Error: New Account is Empty";
            CustomerSpire modelCustomerFromCRM = GetModelAccountFromCRM(ctx, newAccount);

            try
            {

                resultPostCustomer = POST(modelCustomerFromCRM);

            }

            catch (Exception e)
            {

                return "Error";

                
            }


            return resultPostCustomer;
        }

        public static _360PlusPlugin.Models.CustomerSpire  GetModelAccountFromCRM(xrm.XrmServiceContext ctx, xrm.Account ac)
        {
            #region Get Currency

            string CurrencyISOCode = String.Empty;
            if (ac.TransactionCurrencyId != null && ac.TransactionCurrencyId.Id != Guid.Empty)
            {
                 CurrencyISOCode = (from cc in ctx.TransactionCurrencySet where cc.TransactionCurrencyId == ac.TransactionCurrencyId.Id select cc.ISOCurrencyCode).FirstOrDefault();
             }

            #endregion

            #region Create Spire Customer Model
            return new _360PlusPlugin.Models.CustomerSpire()
            {

                name = ac.Name,
                code = ac.AccountNumber,
                customerNo = ac.AccountNumber,
                hold = ac.CreditOnHold!=null? (bool)ac.CreditOnHold : false,
                reference = ac.tsp_Reference,
                applyFinanceCharges = ac.tsp_ApplyFinanceCharges != null ? (bool)ac.tsp_ApplyFinanceCharges: false,
                 creditType = (ac.tsp_CreditType!=null)? ac.tsp_CreditType.Value: 0,
                 creditLimit = (ac.CreditLimit != null) ? decimal.Parse(ac.CreditLimit.Value.ToString()): 0,
                 currency = CurrencyISOCode

            };

                        #endregion
        }
        
        public string POST(CustomerSpire messages)
        {
            //string urlInventory = "https://localhost:10880/api/v1/companies/inspire2_10/sales/orders/";
            string urlCustomer = "https://localhost:10880/api/v1/companies/inspire2_10/customers/";

            String username = "SPIRE";
            String password = "12345";
            string result = String.Empty;

            try
            {

                ServicePointManager.ServerCertificateValidationCallback +=
                      (sender, certificate, chain, sslPolicyErrors) => true;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlCustomer);


                    NetworkCredential networkCredential = new NetworkCredential(username, password);

                    Uri u = new Uri(urlCustomer);
                    CredentialCache credentialCache = new CredentialCache();
                    credentialCache.Add(u, "Basic", networkCredential);

                    httpWebRequest.PreAuthenticate = true;
                    httpWebRequest.Credentials = credentialCache;
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string jsonMsg = SerializeDataInJSON(messages);
                        streamWriter.Write(jsonMsg);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                using (HttpWebResponse webResponse = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (webResponse.StatusCode == HttpStatusCode.OK || webResponse.StatusCode == HttpStatusCode.Created)
                    {
                           if(webResponse.SupportsHeaders && webResponse.Headers.AllKeys.Contains("Location") && webResponse.Headers["Location"] != null)

                                {
                                       result = webResponse.Headers["Location"].Remove(0, webResponse.ResponseUri.ToString().Length);
                                }
               

                    }
                 }
                return result;
                
            }
            catch (Exception e)
            {

                return  "Server error!";
            }
        }


        public string SerializeDataInJSON (CustomerSpire Customer)
        {
            string Serializedresult = String.Empty;
            using (MemoryStream SerializememoryStream = new MemoryStream())
            {
                //initialize DataContractJsonSerializer object and pass Customer class type to it
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(CustomerSpire));
                //write newly created object(Customer) into memory stream
                serializer.WriteObject(SerializememoryStream, Customer);

                //use stream reader to read serialized data from memory stream
                SerializememoryStream.Position = 0;
                StreamReader sr = new StreamReader(SerializememoryStream);

                //get JSON data serialized in string format in string variable 
                Serializedresult = sr.ReadToEnd();
                
            }
            return Serializedresult;


        }

    

        #endregion


    }
}
