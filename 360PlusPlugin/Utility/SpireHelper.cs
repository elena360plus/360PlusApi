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
using Newtonsoft.Json.Linq;

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

        public string SpirePostMethod_Account( Guid _AccountId, IOrganizationService _service, string ContextMessageName)
        {
            service = _service;
            string Error = String.Empty;
            string resultPostCustomer = String.Empty;
            string _SpireURL = String.Empty;
            string _SpireCustomerID = String.Empty;
            string _SpireCustomerNO = String.Empty;
            string _SpireBillingAddressID = String.Empty;
            string _query = String.Empty;
            bool _AccountExists;

            var newAccount = (from ac in ctx.AccountSet where ac.AccountId == _AccountId select ac).FirstOrDefault();
            if (newAccount == null) return "Error: New Account is Empty";

         

            #region create Account Model
            CustomerSpire modelCustomerFromCRM = GetModelAccountFromCRM(newAccount);
            #endregion

          
            _SpireCustomerNO = newAccount.AccountNumber;
            
        
            #region Check If Customer Exists in Spire
            _query =@"customers/?filter= {" + "\"customerNo"+"\":" + "\""+ _SpireCustomerNO+ "\"}";

            _SpireURL = GetSpireURL() + _query;
            _SpireCustomerID = CheckObjectExistsInSpire(_SpireURL);
            #endregion


            try
            {
                if (String.IsNullOrEmpty(_SpireCustomerID) || (_SpireCustomerID.Contains("Error") && _SpireCustomerID.Contains("404")))
                {
                    _SpireURL = GetSpireURL() + "customers/";
                    resultPostCustomer = POST(modelCustomerFromCRM, "POST", _SpireURL, "account");

                    #region Retrieve Address SpireID
                    if (!String.IsNullOrEmpty(resultPostCustomer) && !resultPostCustomer.Contains("Error"))
                    {
                        _query = @"customers/" + resultPostCustomer + "/addresses/?filter= {" + "\"name" + "\":" + "\"" + newAccount.Name + "\"}";
                        _SpireURL = GetSpireURL() + _query;
                        _SpireBillingAddressID = CheckObjectExistsInSpire(_SpireURL);
                        //   https://localhost:10880/api/v1/companies/inspire2_10/customers/2094/addresses/?filter={"name" : "VeryGood488"}
                    }
                    #endregion
                }
                else
                {
                    _SpireURL = GetSpireURL() + "customers/" + _SpireCustomerID;
                    resultPostCustomer = POST(modelCustomerFromCRM, "PUT", _SpireURL, "account");
                    if (String.IsNullOrEmpty(resultPostCustomer) && _SpireCustomerID != newAccount.tsp_SpireID)
                        resultPostCustomer = _SpireCustomerID;
                }

                if(!String.IsNullOrEmpty(resultPostCustomer) &&  !resultPostCustomer.Contains("Error"))
                    {

                    resultPostCustomer = resultPostCustomer + (!String.IsNullOrEmpty(_SpireBillingAddressID) && !_SpireBillingAddressID.Contains("Error") ? ":" +_SpireBillingAddressID: "");
                   }
            }

            catch (Exception e)
            {

               return ("Server Error, please try again later.");

            }
            

            return resultPostCustomer;
        }

        public string SpirePostMethod_Address(Guid _AddressId, IOrganizationService _service, string MessageName)
        {
            service = _service;
            string Error = String.Empty;
            string resultPostCustomer = String.Empty;
            string resultPostAddress = String.Empty;
            string _SpireURL = String.Empty;
            string _SpireCustomerID = String.Empty;
            string _SpireCustomerNO = String.Empty;

            string _SpireAddressID = String.Empty;
            string _query = String.Empty;
         

            var newAddress = (from ad in ctx.CustomerAddressSet where ad.CustomerAddressId == _AddressId select ad).FirstOrDefault();
            var AddressForAccount = (from ac in ctx.AccountSet where ac.AccountId == newAddress.ParentId.Id select ac).FirstOrDefault();

            if (newAddress == null) return "Error: New Address is Empty";

         //   _SpireURL = GetSpireURL();
            _SpireCustomerNO = AddressForAccount.AccountNumber;

            #region If CRM Account doesn't have SpireID 

            if (AddressForAccount.tsp_SpireID == null)
            {
                #region Check If Customer Exists in Spire

                _query = @"customers/?filter= {" + "\"customerNo" + "\":" + "\"" + _SpireCustomerNO + "\"}";
                _SpireURL = GetSpireURL() + _query;
                _SpireCustomerID = CheckObjectExistsInSpire(_SpireURL);

                if (!String.IsNullOrEmpty(_SpireCustomerID) && _SpireCustomerID.Contains("Error")) return "Server Error. Please try again later";

                #endregion

                try
                {
                    #region Create Account in Spire if doesn't exist

                    if (String.IsNullOrEmpty(_SpireCustomerID))
                    {
                        #region create Account Model
                        CustomerSpire modelCustomerFromCRM = GetModelAccountFromCRM(AddressForAccount);
                        #endregion

                        _SpireURL = GetSpireURL() + "customers/";
                        _SpireCustomerID = POST(modelCustomerFromCRM, "POST", _SpireURL, "account");

                    }
                    #endregion

                    #region Update CRM Account Spire ID If Empty
                    if (String.IsNullOrEmpty(_SpireCustomerID))
                    {
                        xrm.Account AccountForUpdate = (from ac in ctx.AccountSet
                                                        where ac.AccountId == AddressForAccount.AccountId
                                                        select new xrm.Account
                                                        {
                                                            AccountId = ac.AccountId,
                                                            tsp_SpireID = ac.tsp_SpireID
                                                        }).FirstOrDefault<xrm.Account>();


                        if (!ctx.IsAttached(AccountForUpdate)) ctx.Attach(AccountForUpdate);
                        ctx.UpdateObject(AccountForUpdate);
                        ctx.SaveChanges();
                    }
                    #endregion

                }

                catch (Exception e)
                {
                    return ("Server Error, please try again later.");
                }
            }
            else _SpireCustomerID = AddressForAccount.tsp_SpireID;
                #endregion

            AddressSpire modelAddressFromCRM = GetModelAddressFromCRM(AddressForAccount, newAddress, false);
            
                #region Check If Address Exists in Spire
                _query = @"customers/"+_SpireCustomerID+ "/addresses/?filter= {" + "\"shipId" + "\":" + "\"" + newAddress.Name.ToUpper() + "\"}";
                _SpireURL = GetSpireURL() + _query;
                _SpireAddressID = CheckObjectExistsInSpire(_SpireURL);
            //    if (!String.IsNullOrEmpty(_SpireCustomerID) && _SpireCustomerID.Contains("Error")) return "Server Error. Please try again later";
                #endregion
                
                try
                {
                    if (String.IsNullOrEmpty(_SpireAddressID) || (_SpireAddressID.Contains("Error") && _SpireAddressID.Contains("404")))
                    {
                        _SpireURL = GetSpireURL() + "customers/"+_SpireCustomerID+ "/addresses/";
                    resultPostAddress = POST(modelAddressFromCRM, "POST", _SpireURL, "address");
                    }
                    else
                    {
                    _SpireURL = GetSpireURL() + "customers/" + _SpireCustomerID + "/addresses/" + _SpireAddressID;
                    resultPostAddress = POST(modelAddressFromCRM, "PUT", _SpireURL, "address");
                        if (String.IsNullOrEmpty(resultPostAddress) && _SpireAddressID != newAddress.tsp_AddressID)
                        resultPostAddress = _SpireAddressID;
                    }
                 

            }

            catch (Exception e)
            {

                return ("Server Error, please try again later.");
            }

            return resultPostAddress;
        }

        public  CustomerSpire  GetModelAccountFromCRM( xrm.Account ac)
        {
            #region Get Currency

            string CurrencyISOCode = "";
            if (ac.TransactionCurrencyId != null && ac.TransactionCurrencyId.Id != Guid.Empty)
            {
                 CurrencyISOCode = (from cc in ctx.TransactionCurrencySet where cc.TransactionCurrencyId == ac.TransactionCurrencyId.Id select cc.ISOCurrencyCode).FirstOrDefault();
             }

            #endregion

            AddressSpire modelAddressFromCRM = GetModelAddressFromCRM( ac, null, true);

            _360PlusPlugin.Models.paymentTermsSpire modelPaymentTermsFromCRM = new _360PlusPlugin.Models.paymentTermsSpire();

            if (ac.PaymentTermsCode != null) modelPaymentTermsFromCRM.id = ac.PaymentTermsCode.Value;
            else modelPaymentTermsFromCRM = null;


            #region Create Spire Customer Model
            return new _360PlusPlugin.Models.CustomerSpire()
            {

                name = ac.Name,
                code = ac.AccountNumber,
                customerNo = ac.AccountNumber,
                hold = ac.CreditOnHold != null ? (bool)ac.CreditOnHold : false,
                reference = ac.tsp_Reference != null? ac.tsp_Reference: "",
                applyFinanceCharges = ac.tsp_ApplyFinanceCharges != null ? (bool)ac.tsp_ApplyFinanceCharges : false,
                creditType = (ac.tsp_CreditType != null) ? ac.tsp_CreditType.Value : 0,
                creditLimit = (ac.CreditLimit != null) ? decimal.Parse(ac.CreditLimit.Value.ToString()) : 0,
                currency = CurrencyISOCode,
                address = modelAddressFromCRM,
                paymentTerms = modelPaymentTermsFromCRM

           };

           

                        #endregion
        }

        public AddressSpire GetModelAddressFromCRM( xrm.Account ac, xrm.CustomerAddress ad, bool fromAccount)
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
            string _phone = String.Empty;
            string _fax = String.Empty;

            #endregion

            #region Get Data from Account Or Address

            if (fromAccount && ac.tsp_Country != null && ac.tsp_Country.Id != Guid.Empty)
            {
                _linkNo = ac.AccountNumber;
                _country = (from cc in ctx.tsp_countrySet where cc.tsp_countryId == ac.tsp_Country.Id select cc.tsp_Code).FirstOrDefault();
                _type = "B";
                _name = ac.Name;
                _linkTable = "CUST";
                _phone = ac.Telephone1 != null ? ac.Telephone1.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : "";
                _fax = ac.Fax != null ? ac.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : "";

            }
            if (!fromAccount)
            {
                _name = ad.Name;
                _country = ad.Country != null ? ad.Country: null;
                _type = ad.AddressTypeCode != null && ad.AddressTypeCode.Value == 1 ? "B": "S";
                _shipId = ad.Name;// ad.AddressTypeCode != null ? ad.FormattedValues["addresstypecode"].ToString() : "Ship To";
                _linkNo = "CUST" + ac.AccountNumber.ToString();// + "             " + _name;
                _linkTable = "SHIP";
                _phone = ad.Telephone1!=null? ad.Telephone1.Replace("(","").Replace(")","").Replace("-","").Replace(" ",""): "";
                _fax = ad.Fax != null ? ad.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : "";

            }

            #endregion

            _streetAddress = fromAccount? (ac.Address1_Line1!= null ? ac.Address1_Line1: "") : (ad.Line1 != null? ad.Line1: "");
            _line1 = fromAccount ? (ac.Address1_Line1 != null ? ac.Address1_Line1 : "") : (ad.Line1 != null ? ad.Line1 : "");  
            _line2 = fromAccount ? (ac.Address1_Line2 != null ? ac.Address1_Line2 : "") : (ad.Line2 != null ? ad.Line2 : ""); 
            _line3 = fromAccount ? (ac.Address1_Line3 != null ? ac.Address1_Line3 : "") : (ad.Line3 != null ? ad.Line3 : ""); 
            _city = fromAccount ? (ac.Address1_City != null ? ac.Address1_City : "") : (ad.City != null ? ad.City : ""); 
            _postalCode = fromAccount ? (ac.Address1_PostalCode != null ? ac.Address1_PostalCode : "") : (ad.PostalCode != null ? ad.PostalCode : ""); 
            _provState = fromAccount ? (ac.Address1_StateOrProvince != null ? ac.Address1_StateOrProvince : "") : (ad.StateOrProvince != null ? ad.StateOrProvince : "");

            #region Create Spire Address Model
            _360PlusPlugin.Models.AddressSpire AddressSpireModel = new _360PlusPlugin.Models.AddressSpire()
            {
                type = _type,
                linkTable = "SHIP",
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
                    number = _phone
                },

                fax = new PhoneSpire()
                {
                    format = 1,
                    number = _fax
                },
                territory = new TerritorySpire()
                {
                    code = null,
                    description = null
                }

            };
          
           ContactSpire[] Contacts = new ContactSpire[3];

            if (!fromAccount)
            {
                Contacts[0] = GetModelContactFromCRM(ad.PrimaryContactName ?? "", ad.tsp_MainContactPhone ?? "", ad.tsp_MainContactFax ?? "", ad.tsp_MainContactEmail ?? "");
                Contacts[1] = GetModelContactFromCRM(ad.tsp_SalesContactName ?? "", ad.tsp_SalesContactPhone ?? "", ad.tsp_SalesContactFax ?? "", ad.tsp_SalesContactEmail ?? "");
                Contacts[2] = GetModelContactFromCRM(ad.tsp_AccountingContactName ?? "", ad.tsp_AccountingContactPhone ?? "", ad.tsp_AccountingContactFax ?? "", ad.tsp_AccountingContactEmail ?? "");
            }
            else
            {
                Contacts[0] = Contacts[1] = Contacts[2]= GetModelContactFromCRM("", "", "",  "");

            }
            AddressSpireModel.contacts = Contacts;
            return AddressSpireModel;

            #endregion
        }

        public ContactSpire GetModelContactFromCRM(string _Name, string _Phone, string _Fax, string _Email)
        {

            return new ContactSpire()
            {

                name = !String.IsNullOrEmpty(_Name) ? _Name : "",
                phone = new PhoneSpire()
                {
                    format = 1,
                    number = !String.IsNullOrEmpty(_Phone) ? _Phone : ""
                },

                fax = new PhoneSpire()
                {
                    format = 1,
                    number = !String.IsNullOrEmpty(_Fax) ? _Fax : ""
                },
                email = !String.IsNullOrEmpty(_Email) ? _Email : ""

            };

        }

        public string CheckObjectExistsInSpire(string _SpireURL)
        {
            bool ObjectExistsInSpire;
            if (String.IsNullOrEmpty(_SpireURL)) return "Error: Spire requested URL is empty";
            
                String username = "SPIRE";
                String password = "12345";
                string result = String.Empty;
                try
                {
                    // Accept self-signed certificate
                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, certificate, chain, sslPolicyErrors) => true;
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_SpireURL);

                    NetworkCredential networkCredential = new NetworkCredential(username, password);
                    Uri u = new Uri(_SpireURL);
                    CredentialCache credentialCache = new CredentialCache();
                    credentialCache.Add(u, "Basic", networkCredential);

                    httpWebRequest.PreAuthenticate = true;
                    httpWebRequest.Credentials = credentialCache;
                    httpWebRequest.Method = "GET";

                    using (HttpWebResponse webResponse = httpWebRequest.GetResponse() as HttpWebResponse)
                    {
                        if (webResponse.StatusCode == HttpStatusCode.OK)
                        {
                            Stream responseStream = webResponse.GetResponseStream();
                            StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);
                            string jsonText = myStreamReader.ReadToEnd();
                           JObject jsonResponse = JObject.Parse(jsonText);
                      
                        string _Count = JObject.Parse(jsonText)["count"].ToString();
                        if(_Count != "0") return jsonResponse["records"].Children().FirstOrDefault()["id"].ToString();
                        return result;
                         }
                   }
                    return "Error: Web Response failed";
                }
                catch (WebException e)
                {

                HttpWebResponse res = (HttpWebResponse)e.Response;
                // Console.WriteLine("Http status code: " + res.StatusCode);
                if (res.StatusCode.ToString().Contains("NotFound"))
                    return "Error: 404";
                else return "Server Error, please try again later.";
            }
         
         
            }
 
        public string GetSpireURL()
        {
            //string urlInventory = "https://localhost:10880/api/v1/companies/inspire2_10/sales/orders/";
            string urlCustomer = String.Empty;
            string Sur = String.Empty;


            urlCustomer = "https://localhost:10880/api/v1/companies/inspire2_10/";

            //if (String.IsNullOrEmpty(SpireAccountID))
            //{
            //    urlCustomer = "https://localhost:10880/api/v1/companies/inspire2_10/customers/";
            //}
            //else
            //{
            //    urlCustomer = "https://localhost:10880/api/v1/companies/inspire2_10/customers/" + SpireAccountID + "/addresses/";
            //}


            //  https://localhost:10880/api/v1/companies/inspire2_10/customers/?filter={"customerNo":"plugin1"}
            return urlCustomer;
        }

        public string POST(object messages, string contextMessage, string urlCustomer, string EntityName)
        {

            string Sur = String.Empty;
            #region Not In Use
            //string urlInventory = "https://localhost:10880/api/v1/companies/inspire2_10/sales/orders/";
            // string urlCustomer = String.Empty;


            //  urlCustomer = GetSpireURL(contextMessage, SpireAccountID, _query);

            //if (String.IsNullOrEmpty(SpireAccountID))
            //{
            //    urlCustomer = "https://localhost:10880/api/v1/companies/inspire2_10/customers/";
            //    Sur = "account";
            //}
            //else
            //{
            //    urlCustomer = "https://localhost:10880/api/v1/companies/inspire2_10/customers/" + SpireAccountID + "/addresses/";
            //    Sur = "address";
            //}

            #endregion
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
                    httpWebRequest.Method = contextMessage;

                    ServicePointManager.DefaultConnectionLimit = 50;
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                    string jsonMsg =  SerializeDataInJSON(messages, EntityName);
                  //  jsonMsg = jsonMsg.Replace("null", @"""""");
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
            catch (WebException e)
            {

                HttpWebResponse res = (HttpWebResponse)e.Response;
                // Console.WriteLine("Http status code: " + res.StatusCode);
                if (res.StatusCode.ToString() == "423")
                    return "Error: The Update failed because the record is open and lockes in Spire.Please close this record in Spire and try again.";
                else return "Server Error, please try again later.";
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
