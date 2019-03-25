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
using _360PlusPlugin.Utility;

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
            CustomerSpire modelCustomerFromCRM = GetModelAccountFromCRM(newAccount);
          

            try
            {

                resultPostCustomer = POST(modelCustomerFromCRM, String.Empty);

            }

            catch (Exception e)
            {

                return "Error";

                
            }


            return resultPostCustomer;
        }

        public string SpirePostMethod_Address(Guid _AddressId, IOrganizationService _service)
        {
            service = _service;
            string Error = String.Empty;
            string resultPostCustomer = String.Empty;

            var newAddress = (from ad in ctx.CustomerAddressSet where ad.CustomerAddressId == _AddressId select ad).FirstOrDefault();
           
            var AddressForAccount = (from ac in ctx.AccountSet where ac.AccountId == newAddress.ParentId.Id select ac).FirstOrDefault();
            if (newAddress == null) return "Error: New Address is Empty";
            var SpireAccountId = AddressForAccount.tsp_SpireID;

            AddressSpire modelAddressFromCRM = GetModelAddressFromCRM(AddressForAccount, newAddress, false);

            try
            {

                resultPostCustomer = POST(modelAddressFromCRM, SpireAccountId);

            }

            catch (Exception e)
            {

                return "Error";


            }


            return resultPostCustomer;
        }


        public  _360PlusPlugin.Models.CustomerSpire  GetModelAccountFromCRM( xrm.Account ac)
        {
            #region Get Currency

            string CurrencyISOCode = String.Empty;
            if (ac.TransactionCurrencyId != null && ac.TransactionCurrencyId.Id != Guid.Empty)
            {
                 CurrencyISOCode = (from cc in ctx.TransactionCurrencySet where cc.TransactionCurrencyId == ac.TransactionCurrencyId.Id select cc.ISOCurrencyCode).FirstOrDefault();
             }

            #endregion

            AddressSpire modelAddressFromCRM = GetModelAddressFromCRM( ac, null, true);

            #region Create Spire Customer Model
           return new _360PlusPlugin.Models.CustomerSpire()
            {

                name = ac.Name,
                code = ac.AccountNumber,
                customerNo = ac.AccountNumber,
                hold = ac.CreditOnHold != null ? (bool)ac.CreditOnHold : false,
                reference = ac.tsp_Reference,
                applyFinanceCharges = ac.tsp_ApplyFinanceCharges != null ? (bool)ac.tsp_ApplyFinanceCharges : false,
                creditType = (ac.tsp_CreditType != null) ? ac.tsp_CreditType.Value : 0,
                creditLimit = (ac.CreditLimit != null) ? decimal.Parse(ac.CreditLimit.Value.ToString()) : 0,
                currency = CurrencyISOCode,
                address = modelAddressFromCRM    
            };

           

                        #endregion
        }


        public  _360PlusPlugin.Models.AddressSpire GetModelAddressFromCRM( xrm.Account ac, xrm.CustomerAddress ad, bool fromAccount)
        {
            #region Constructors
            
            string _type = String.Empty;
            string _shipId = String.Empty;
            string _linkTable = String.Empty;
            string _linkNo = String.Empty;
            string _name = String.Empty;
            string _streetAddress = String.Empty;
            string _line1 = String.Empty;

            string _line2 = String.Empty;
            string _line3 = String.Empty;
            string _city = String.Empty;
            string _postalCode = String.Empty;
            string _provState = String.Empty;
            string _country = String.Empty;

            #endregion



            #region Get Country, Name and No

            _linkNo = ac.AccountNumber;
            _name = ac.Name;

            if (fromAccount && ac.tsp_Country != null && ac.tsp_Country.Id != Guid.Empty)
            {
                _country = (from cc in ctx.tsp_countrySet where cc.tsp_countryId == ac.tsp_Country.Id select cc.tsp_Code).FirstOrDefault();
                _type = "B";
               
            }
            if (!fromAccount)
            {
               
                _country = ad.tsp_Coutry != null ? ad.Country: null;
                _type = ad.AddressTypeCode != null && ad.AddressTypeCode.Value == 1 ? "B": "S";
                _shipId = ad.AddressTypeCode != null ? ad.FormattedValues["addresstypecode"].ToString() : "Ship To";
            }
            #endregion

            _linkTable = "CUST";
            _streetAddress = fromAccount? ac.Address1_Line1: ad.Line1;
            _line1 = fromAccount ? ac.Address1_Line1 : ad.Line1;
            _line2 = fromAccount ? ac.Address1_Line2 : ad.Line2;
            _line3 = fromAccount ? ac.Address1_Line3 : ad.Line3;
            _city = fromAccount ? ac.Address1_City: ad.City;
            _postalCode = fromAccount ? ac.Address1_PostalCode: ad.PostalCode;
            _provState = fromAccount ? ac.Address1_StateOrProvince: ad.StateOrProvince;

            #region Create Spire Address Model
            return new _360PlusPlugin.Models.AddressSpire()
            {
                type = _type,
                linkTable = "CUST",
                linkNo = _linkNo,
                shipId = _shipId,
                name = _name,
                streetAddress = _streetAddress,
                line1 = _line1,
                line2 = _line2,
                line3 = _line3,
                city = _city,
                postalCode = _postalCode,
                provState = _provState,
                country = _country,
                phone = new PhoneSpire()
                {
                    format = 1,
                    number = null
                },

                fax = new PhoneSpire()
                {
                    format = 1,
                    number = null
                }

            };

            #endregion
        }


        public string POST(object messages, string SpireAccountID)
        {
            //string urlInventory = "https://localhost:10880/api/v1/companies/inspire2_10/sales/orders/";
            string urlCustomer = String.Empty;
            string Sur = String.Empty;

            if (String.IsNullOrEmpty(SpireAccountID))
            {
                urlCustomer = "https://localhost:10880/api/v1/companies/inspire2_10/customers/";
                Sur = "account";
            }
            else
            {
                urlCustomer = "https://localhost:10880/api/v1/companies/inspire2_10/customers/" + SpireAccountID + "/addresses/";
                Sur = "address";
            }

            
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
                    string jsonMsg =  SerializeDataInJSON(messages,Sur);
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


        public string SerializeDataInJSON (object Customer, string TypeSer)
        {
            string Serializedresult = String.Empty;
            using (MemoryStream SerializememoryStream = new MemoryStream())
            {
                //initialize DataContractJsonSerializer object and pass Customer class type to it
                DataContractJsonSerializer serializer = null;
                if(TypeSer == "account")
                serializer  = new DataContractJsonSerializer(typeof(CustomerSpire));
                else if(TypeSer == "address")
                    serializer = new DataContractJsonSerializer(typeof(AddressSpire));
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
