using System;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using xrm = SpirePlusPlugin;
using System.Net;
using _360PlusPlugin.Models;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using _360PlusPlugin.Utility;
using System.Collections.Generic;

namespace Spire_BusinessEntities
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

        #region Prepare Data Integration to Spire 

        public string SpirePostMethod_Account( Guid _AccountId, IOrganizationService _service, string ContextMessageName, int _ContextDepth)
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
         
            _SpireCustomerNO = newAccount.AccountNumber;
            
            try
            {
                CustomerAllItemsSpire modelCustomerFromCRM = new CustomerAllItemsSpire();

                #region Check If Customer Exists in Spire

                 _query = @"customers/?filter= {" + "\"customerNo" + "\":" + "\"" + _SpireCustomerNO + "\"}";
                 _SpireURL = GetSpireURL() + _query;
                 _SpireCustomerID = CheckObjectExistsInSpire(_SpireURL,"id");

                #endregion

                #region If Account Exists in Spire already send PUT request
                if (!String.IsNullOrEmpty(_SpireCustomerID) && !_SpireCustomerID.Contains("Error"))
                {
                    #region create Account Model
                    var _AddressOOB = (from ac in ctx.CustomerAddressSet
                                       where ac.AddressNumber == 1
                                       where ac.ParentId.Id == _AccountId
                                       select ac).FirstOrDefault();
                    modelCustomerFromCRM = GetModelAccountFromCRM(newAccount, _AddressOOB);
                    #endregion

                    resultPostCustomer = _SpireCustomerID;
                    _SpireURL = GetSpireURL() + "customers/" + _SpireCustomerID;
                    resultPostCustomer = POST(modelCustomerFromCRM, "PUT", _SpireURL, "account");
                }
                #endregion
   
                #region Create Account in Spire if doesn't exist and Update CRM Spire Billing Address ID after creation
                else if (String.IsNullOrEmpty(_SpireCustomerID) || (_SpireCustomerID.Contains("Error") && _SpireCustomerID.Contains("404"))) // Not Found
                {
                    #region Create Account in Spire if doesn't exist 
                    modelCustomerFromCRM = GetModelAccountFromCRM(newAccount, null);
                    _SpireURL = GetSpireURL() + "customers/";
                    resultPostCustomer = POST(modelCustomerFromCRM, "POST", _SpireURL, "account");
                    #endregion

                    #region Retrieve Biiling Address SpireID
                    if (!String.IsNullOrEmpty(resultPostCustomer) && !resultPostCustomer.Contains("Error"))
                    {
                        _query = @"customers/" + resultPostCustomer + "/addresses/?filter= {" + "\"name" + "\":" + "\"" + newAccount.Name + "\"}";
                        _query = _query.Replace("&", "%26");
                        _SpireURL = GetSpireURL() + _query;
                        _SpireBillingAddressID = CheckObjectExistsInSpire(_SpireURL,"id");
                        //   https://localhost:10880/api/v1/companies/inspire2_10/customers/2094/addresses/?filter={"name" : "VeryGood488"}
                        #endregion
                    }
                }
                #endregion

                #region Create BillingAddress in CRM Custom Address Entity

                if(_ContextDepth ==1)  CreateUpdateAddressAndContactInCRM(modelCustomerFromCRM, null,null, _SpireBillingAddressID, "Create", true, _AccountId);

                #region Not In Use
                //xrm.tsp_accountaddresses newAddressCustomEntity = new xrm.tsp_accountaddresses()
                //{
                //    tsp_Account = new EntityReference(xrm.Account.EntityLogicalName, (Guid)newAccount.AccountId),
                //    tsp_name = "Billing Address",
                //    tsp_SpireID =   (!String.IsNullOrEmpty(resultPostCustomer) && !resultPostCustomer.Contains("Error"))? _SpireBillingAddressID: null,
                //    tsp_Street1 = newAccount.Address1_Line1,
                //    tsp_Street2 = newAccount.Address1_Line2,
                //    tsp_Street3 = newAccount.Address1_Line3,
                //    tsp_City = newAccount.Address1_City,
                //    tsp_CountryRegion = newAccount.Address1_Country,
                //    tsp_StateProvince = newAccount.Address1_StateOrProvince,
                //    tsp_ZipPostalCode = newAccount.Address1_PostalCode

                //};

                //ctx.AddObject(newAddressCustomEntity);
                //ctx.SaveChanges();
                #endregion
                #endregion

                if (!String.IsNullOrEmpty(resultPostCustomer) &&  !resultPostCustomer.Contains("Error"))
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
      
        public string SpirePostMethod_Address(Guid _AddressId, IOrganizationService _service, string MessageName, int _ContextDepth, Guid _ContactAccountForBillAddress)
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

            xrm.CustomerAddress newAddress = null;
            xrm.Account AddressForAccount = null;
            if (_ContactAccountForBillAddress == Guid.Empty)
            {
                newAddress = (from ad in ctx.CustomerAddressSet where ad.CustomerAddressId == _AddressId select ad).FirstOrDefault();
                AddressForAccount = (from ac in ctx.AccountSet where ac.AccountId == newAddress.ParentId.Id select ac).FirstOrDefault();
            }
            else
            {
                AddressForAccount = (from ac in ctx.AccountSet where ac.AccountId == _ContactAccountForBillAddress select ac).FirstOrDefault();
                newAddress = (from ad in ctx.CustomerAddressSet where ad.AddressNumber == 1 &&  ad.ParentId.Id == _ContactAccountForBillAddress select ad).FirstOrDefault();
            }
                
            if (newAddress == null) return "Error: New Address is Empty";
            _SpireCustomerNO = AddressForAccount.AccountNumber;

            #region  Check and create Account in Spire if doesn't exist

            if (AddressForAccount.tsp_SpireID == null)
            {
                _SpireCustomerID = CheckAndCreateAccountInSpire(_SpireCustomerNO, AddressForAccount);
            }
            else _SpireCustomerID = AddressForAccount.tsp_SpireID;
            if (String.IsNullOrEmpty(_SpireCustomerID) || (_SpireCustomerID.Contains("Error"))) return "Server Error, please try again later.";

            #endregion

            AddressSpire modelAddressFromCRM = GetModelAddressFromCRM(AddressForAccount, newAddress, false);

            #region Update Values in address model for Billing Address

            if (newAddress.Name == null || String.IsNullOrEmpty(newAddress.Name))
            {
                modelAddressFromCRM.linkNo = AddressForAccount.AccountNumber;                
                modelAddressFromCRM.type = "B";
                modelAddressFromCRM.name = AddressForAccount.Name;
                modelAddressFromCRM.linkTable = "CUST";
                modelAddressFromCRM.shipId = "";
                modelAddressFromCRM.email = AddressForAccount.EMailAddress1 ?? "";
                modelAddressFromCRM.phone = new PhoneSpire()
                {
                    format = 1,
                    number = AddressForAccount.Telephone1 != null ? AddressForAccount.Telephone1.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : ""
                  };

                modelAddressFromCRM.fax = new PhoneSpire()
                {
                    format = 1,
                    number = AddressForAccount.Fax != null ? AddressForAccount.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : ""
                };

                #region Update Contacts Model for Spire from Contacts Entity for Billing Address

                if (_ContactAccountForBillAddress != Guid.Empty)
                 {
                    var _ListContactsForBilling = (from ac in ctx.ContactSet where (ac.tsp_ContactAddress.Id == _AddressId || ac.tsp_ContactAddress == null) && ac.ParentCustomerId.Id == AddressForAccount.Id && ac.tsp_ContacttoSpire == true select ac).ToList();
                    if (_ListContactsForBilling != null && _ListContactsForBilling.Count() > 0)
                    {
                        OptionSetValue _contactType = new OptionSetValue();
                        ContactSpire[] Contacts = new ContactSpire[3];

                        Contacts[0] = Contacts[1] = Contacts[2] = GetModelContactFromCRM("", "", "", "", "");

                        foreach (var ad in _ListContactsForBilling)
                        {
                            if (ad.tsp_ContactType != null) _contactType.Value = ad.tsp_ContactType.Value;
                            else _contactType.Value = (int)ContactType.Other;
                          
                                if (_contactType.Value == (int)ContactType.Main)
                                {
                                    Contacts[0] = GetModelContactFromCRM(ad.LastName ?? "", ad.FirstName ?? "", ad.MobilePhone ?? "", ad.Fax ?? "", ad.EMailAddress1 ?? "");
                                   
                                }
                                else if (_contactType.Value == (int)ContactType.Sales)
                                {
                                    Contacts[1] = GetModelContactFromCRM(ad.LastName ?? "", ad.FirstName ?? "", ad.MobilePhone ?? "", ad.Fax ?? "", ad.EMailAddress1 ?? "");
                                   
                                }
                                else if (_contactType.Value == (int)ContactType.Accounting)
                                {
                                    Contacts[2] = GetModelContactFromCRM(ad.LastName ?? "", ad.FirstName ?? "", ad.MobilePhone ?? "", ad.Fax ?? "", ad.EMailAddress1 ?? "");
                                    
                                }
                            //Contacts[0] = Contacts[1] = GetModelContactFromCRM("", "", "", "", "");
                        }
                        modelAddressFromCRM.contacts = Contacts;
                    }
                }
                #endregion
            }
            #endregion

            #region Check If Address Exists already in Spire

            _query = newAddress.Name != null? (@"customers/" +_SpireCustomerID+ "/addresses/?filter= {" + "\"shipId" + "\":" + "\"" + newAddress.Name.ToUpper() + "\"}"):
                (@"customers/" + _SpireCustomerID + "/addresses/?filter= {" + "\"type" + "\":" + "\"B" + "\"}");
               _SpireURL = GetSpireURL() + _query;
                _SpireAddressID = CheckObjectExistsInSpire(_SpireURL, "id");
            //    if (!String.IsNullOrEmpty(_SpireCustomerID) && _SpireCustomerID.Contains("Error")) return "Server Error. Please try again later";
            #endregion

            #region Create or Update Address in Spire
            
                try
                {
                    if (String.IsNullOrEmpty(_SpireAddressID) || (_SpireAddressID.Contains("Error") && _SpireAddressID.Contains("404")))
                    {
                        _SpireURL = GetSpireURL() + "customers/" + _SpireCustomerID + "/addresses/";
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

            #endregion

            #region Create Ship to Address in Custom Address Entity and Create Contacts (Main, Sales,Accounting ) realated to this Address

            if (_ContextDepth == 1)
            {
                ContactFirstLastName[] modelAddressFromCRMWithContact = new ContactFirstLastName[3];
                modelAddressFromCRMWithContact = GetModelContactFirstLastNameFromCRM(newAddress);
                CreateUpdateAddressAndContactInCRM(null, modelAddressFromCRM, modelAddressFromCRMWithContact, resultPostAddress, "Create", false, (Guid)AddressForAccount.AccountId);
            }
               #endregion
            
            return resultPostAddress;
        }

        public string SpirePostMethod_WonOpportunity(Guid _OpportunityId, IOrganizationService _service, string ContextMessageName)
        {
            service = _service;
            string Error = String.Empty;
            string resultPostOpportunityID = String.Empty;
            string _SpireURL = String.Empty;
            string _SpireCustomerID = String.Empty;
            string _SpireCustomerNO = String.Empty;
            string _SpireWorkorderNO = String.Empty;
            string _SpireBillingAddressID = String.Empty;
            string _SpireShipAddressID = String.Empty;
            string _query = String.Empty;
          

            #region Validation and Parsing Data
            var newOpportunity = (from op in ctx.OpportunitySet where op.OpportunityId == _OpportunityId select op).FirstOrDefault();
            if (newOpportunity == null) return "Error: Opportunity doesn't exist.";
            Guid _accountId = newOpportunity.ParentAccountId != null? (Guid)newOpportunity.ParentAccountId.Id: Guid.Empty;
            if (_accountId == Guid.Empty) return "Error: Accounyt doesn't exist.";
            var newAccount = (from ac in ctx.AccountSet where ac.AccountId == _accountId select ac).FirstOrDefault();
            if (newAccount == null) return "Error: New Account is Empty";

       
            #endregion

            #region If CRM Account doesn't have SpireID 
            

            if (newAccount.tsp_SpireID == null || String.IsNullOrEmpty(newAccount.tsp_SpireID))
            {
                _SpireCustomerID = CheckAndCreateAccountInSpire(newAccount.AccountNumber, newAccount);
                if (String.IsNullOrEmpty(_SpireCustomerID) || (_SpireCustomerID.Contains("Error"))) return "Server Error, please try again later.";
               newAccount.tsp_SpireID = _SpireCustomerID;
            }
          
        

         //   newAccount.tsp_SpireID = _SpireCustomerID;
            #endregion


            #region create Opportunity/Order Model
            OrderSpire OrderModelFromCRM = GetModelOpportunityFromCRM(newOpportunity, newAccount);
            #endregion

            try
            {
              //  https://localhost:10880/api/v1/companies/inspire2_10/sales/orders

                _SpireURL = GetSpireURL() + "sales/orders/";
                resultPostOpportunityID = POST(OrderModelFromCRM, "POST", _SpireURL, "order");

                if (resultPostOpportunityID != null && !resultPostOpportunityID.Contains("Error"))
                    {
                    _SpireURL = GetSpireURL() + "sales/orders/"+ resultPostOpportunityID;
                    _SpireWorkorderNO = CheckObjectExistsInSpire(_SpireURL, "orderNo");
                    if (!String.IsNullOrEmpty(_SpireWorkorderNO) && !_SpireWorkorderNO.Contains("Error"))
                        resultPostOpportunityID = _SpireWorkorderNO;
                }

                return resultPostOpportunityID;


            }

            catch (Exception e)
            {

                return ("Server Error, please try again later.");

            }


             
        }

        public string CheckObjectExistsInSpire(string _SpireURL, string _returnValueOf)
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

                        if (JObject.Parse(jsonText)["count"] != null && jsonResponse["records"] != null)
                        {
                            string _Count = JObject.Parse(jsonText)["count"].ToString();
                            if (_Count != "0") return jsonResponse["records"].Children().FirstOrDefault()[_returnValueOf].ToString();
                            return result;
                        }
                        else if (JObject.Parse(jsonText) != null && jsonResponse[_returnValueOf] != null)
                        {
                            return jsonResponse[_returnValueOf].ToString();
                        }
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

        public string CheckAndCreateAccountInSpire(string _SpireCustomerNO, xrm.Account _Account)
        {

            #region Check If Customer Exists in Spire

            string _query = @"customers/?filter= {" + "\"customerNo" + "\":" + "\"" + _SpireCustomerNO + "\"}";
            string _SpireURL = GetSpireURL() + _query;
            string _SpireCustomerID = CheckObjectExistsInSpire(_SpireURL, "id");

            if (!String.IsNullOrEmpty(_SpireCustomerID) && !_SpireCustomerID.Contains("Error")) return _SpireCustomerID;

            #endregion

            try
            {
                #region Create Account in Spire if doesn't exist

                if (String.IsNullOrEmpty(_SpireCustomerID) || (_SpireCustomerID.Contains("Error") && _SpireCustomerID.Contains("404")))
                {
                    #region create Account in Spire
                    CustomerAllItemsSpire modelCustomerFromCRM = GetModelAccountFromCRM(_Account, null);
                    _SpireURL = GetSpireURL() + "customers/";
                    _SpireCustomerID = POST(modelCustomerFromCRM, "POST", _SpireURL, "account");
                    #endregion

                    #region Update CRM Account Spire ID If Empty
                    if (!String.IsNullOrEmpty(_SpireCustomerID) && !_SpireCustomerID.Contains("Error"))
                    {
                        xrm.Account AccountForUpdate = (from ac in ctx.AccountSet
                                                        where ac.AccountId == _Account.AccountId
                                                        select new xrm.Account
                                                        {
                                                            AccountId = ac.AccountId,
                                                            tsp_SpireID = ac.tsp_SpireID
                                                        }).FirstOrDefault<xrm.Account>();

                        AccountForUpdate.tsp_SpireID = _SpireCustomerID;
                        if (!ctx.IsAttached(AccountForUpdate)) ctx.Attach(AccountForUpdate);
                        ctx.UpdateObject(AccountForUpdate);
                        ctx.SaveChanges();

                        return _SpireCustomerID;
                    }

                    #endregion
                }

                #endregion

                return ("Server Error, please try again later.");

            }

            catch (Exception e)
            {
                return ("Server Error, please try again later.");
            }

        }

        #endregion

        #region Update CRM records 
        public string CreateUpdateAddressAndContactInCRM(CustomerAllItemsSpire modelCustomerSpire, AddressSpire modelAddressFromCRM, ContactFirstLastName[] contactFromCRM, string _SpireAddressID, string _MessageName, bool _IsBillingAddress,Guid _accountId)
        {

            string _AddressName = _IsBillingAddress ? "Billing Address" : modelAddressFromCRM.name;

            #region Create or Update New Address in Custom Address Entity
            try
            {
                #region Check if Custom Address exists
                xrm.tsp_accountaddresses xrmCustomAddress = ctx.tsp_accountaddressesSet.Where(a => a.tsp_Account.Id == _accountId && a.tsp_name == _AddressName).Select(a => new xrm.tsp_accountaddresses()
                { tsp_accountaddressesId = a.tsp_accountaddressesId }).FirstOrDefault();
                #endregion

                xrm.tsp_accountaddresses newAddressCustomEntity = new xrm.tsp_accountaddresses()
                    {
                        tsp_Account = new EntityReference(xrm.Account.EntityLogicalName, _accountId),
                        tsp_name = _AddressName,
                        tsp_SpireID = _SpireAddressID,
                        tsp_Street1 = _IsBillingAddress ? modelCustomerSpire.address.line1: modelAddressFromCRM.line1,
                        tsp_Street2 = _IsBillingAddress ? modelCustomerSpire.address.line2: modelAddressFromCRM.line2,
                        tsp_Street3 = _IsBillingAddress ? modelCustomerSpire.address.line3: modelAddressFromCRM.line2,
                        tsp_City = _IsBillingAddress ? modelCustomerSpire.address.city: modelAddressFromCRM.city,
                        tsp_CountryRegion = _IsBillingAddress ? modelCustomerSpire.address.country: modelAddressFromCRM.country,
                        tsp_StateProvince = _IsBillingAddress ? modelCustomerSpire.address.provState: modelAddressFromCRM.provState,
                        tsp_ZipPostalCode = _IsBillingAddress ? modelCustomerSpire.address.postalCode: modelAddressFromCRM.postalCode
                };

                if (xrmCustomAddress == null)
                {
                    ctx.AddObject(newAddressCustomEntity);
                }
                else
                {
                    newAddressCustomEntity.tsp_accountaddressesId = xrmCustomAddress.tsp_accountaddressesId;
                    ctx.ClearChanges();
                    if (!ctx.IsAttached(newAddressCustomEntity)) ctx.Attach(newAddressCustomEntity);
                    ctx.UpdateObject(newAddressCustomEntity);
                }
                ctx.SaveChanges();

                #endregion

                #region Create or Update Contacts(Main,Sales and Accounting) from Ship To address

                if (!_IsBillingAddress)
                {
                    if (contactFromCRM != null && contactFromCRM.Count() > 0  )
                    {
                        int i = 0;
                        OptionSetValue _contactType = new OptionSetValue();
                        foreach (var _contact in contactFromCRM)
                        {
                            if (_contact.fisrtName != null && !String.IsNullOrEmpty(_contact.fisrtName) || _contact.lastName != null && !String.IsNullOrEmpty(_contact.lastName))
                            {
                                #region Check if contact exist
                                switch (i)
                                    {
                                    case 0:
                                    _contactType.Value = (int)ContactType.Main;
                                        break;
                                    case 1:
                                        _contactType.Value = (int)ContactType.Sales;
                                        break;
                                    case 2:
                                        _contactType.Value = (int)ContactType.Accounting;
                                        break;
                            }
                   xrm.Contact xrmContact = ctx.ContactSet.Where(a => a.ParentCustomerId.Id == _accountId && a.tsp_ContactAddress.Id == (Guid)newAddressCustomEntity.tsp_accountaddressesId
                   && a.tsp_ContactType != null && a.tsp_ContactType.Value == _contactType.Value).Select(a => new xrm.Contact() { ContactId = a.ContactId }).FirstOrDefault();

                                #endregion
                                xrm.Contact newContact = new xrm.Contact()
                                {
                                    FirstName = _contact.fisrtName,
                                    LastName = _contact.lastName,
                                    Address1_City = modelAddressFromCRM.city,
                                    Address1_Country = modelAddressFromCRM.country,
                                    Address1_Fax = modelAddressFromCRM.fax.number,
                                    Address1_Line1 = modelAddressFromCRM.line1,
                                    Address1_Line2 = modelAddressFromCRM.line2,
                                    Address1_Line3 = modelAddressFromCRM.line3,
                                    Address1_Name = modelAddressFromCRM.name,
                                    Address1_PostalCode = modelAddressFromCRM.postalCode,
                                    Address1_StateOrProvince = modelAddressFromCRM.provState,
                                    EMailAddress1 = _contact.contactSpire.email,
                                    Fax = _contact.contactSpire.fax.number,
                                    MobilePhone = _contact.contactSpire.phone.number,
                                    ParentCustomerId = new EntityReference(xrm.Account.EntityLogicalName, _accountId),
                                    Telephone1 = modelAddressFromCRM.phone.number,
                                    tsp_ContactAddress = new EntityReference(xrm.tsp_accountaddresses.EntityLogicalName, (Guid)newAddressCustomEntity.tsp_accountaddressesId),
                                    tsp_ContactType = _contactType,
                                    tsp_ContacttoSpire = true
                                };

                                if (xrmContact == null)
                                {
                                    ctx.AddObject(newContact);
                                }
                                else
                                {
                                    newContact.ContactId = xrmContact.ContactId;
                                    ctx.ClearChanges();
                                    if (!ctx.IsAttached(newContact)) ctx.Attach(newContact);
                                    ctx.UpdateObject(newContact);
                                }
                                ctx.SaveChanges();
                            }
                            i++;
                        }
                    }
                    #endregion
                }
                return String.Empty;
            }

            catch (Exception e)
            {

                return ("Server Error, please try again later.");

            }

        }
        public Guid CreateUpdateContactInCustomAddress(Guid _ContactId, IOrganizationService _service)
        {

            service = _service;
            string Error = String.Empty;
            string resultPostContact = String.Empty;
            string _SpireURL = String.Empty;
            string _SpireCustomerID = String.Empty;
            string _SpireCustomerNO = String.Empty;
           string _SpireBillingAddressID = String.Empty;
            string _SpireShipAddressID = String.Empty;
            string _SpireContactLinkAddressID = String.Empty;
            Guid _LinkCustomAddressId = Guid.Empty;
            bool IsBillingAddressContact;
            
            string _query = String.Empty;
           
            #region Validation and Parsing Data

            var newContact = (from co in ctx.ContactSet where co.ContactId == _ContactId select co).FirstOrDefault();
            if (newContact == null) return Guid.Empty;
            Guid _accountId = newContact.ParentCustomerId != null ? (Guid)newContact.ParentCustomerId.Id : Guid.Empty;
            if (_accountId == Guid.Empty) return Guid.Empty;
            #endregion

            #region Retrieve parent Account
            xrm.Account newAccount = (from ac in ctx.AccountSet where ac.AccountId == _accountId
                                      select new xrm.Account
                                      {
                                          AccountId = ac.AccountId,
                                          tsp_SpireID=ac.tsp_SpireID,
                                          AccountNumber=ac.AccountNumber,
                                          tsp_BillingAddressID = ac.tsp_BillingAddressID
                                      }).FirstOrDefault<xrm.Account>();

            if (newAccount == null) return Guid.Empty;
            #endregion

            #region If Biiling Address Spire ID is empty in CRM, Check if Account/Billing Address Exists in Spire and Update CRM spireID

            if (newAccount.tsp_BillingAddressID == null || String.IsNullOrEmpty(newAccount.tsp_BillingAddressID))
            {
                _SpireCustomerID = newAccount.tsp_SpireID;
                if (String.IsNullOrEmpty(_SpireCustomerID))
                {
                    _SpireCustomerID = CheckAndCreateAccountInSpire(newAccount.AccountNumber, newAccount);
                    if (String.IsNullOrEmpty(_SpireCustomerID) || (_SpireCustomerID.Contains("Error"))) return Guid.Empty;
                    newAccount.tsp_SpireID = _SpireCustomerID;
                }
                _query = @"customers/" + _SpireCustomerID + "/addresses/?filter= {" + "\"type" + "\":" + "\"B" + "\"}";
                _SpireURL = GetSpireURL() + _query;
                _SpireBillingAddressID = CheckObjectExistsInSpire(_SpireURL, "id");
                if (!String.IsNullOrEmpty(_SpireBillingAddressID))
                {
                    newAccount.tsp_BillingAddressID = _SpireBillingAddressID;
                    if (!ctx.IsAttached(newAccount)) ctx.Attach(newAccount);
                    ctx.UpdateObject(newAccount);
                    ctx.SaveChanges();
                }

            }
            else _SpireBillingAddressID = newAccount.tsp_BillingAddressID;
            #endregion

            #region Get Link Contact to Address (By default link to Billing Address)

            xrm.tsp_accountaddresses _AddressCustomEntity = null;
            string _AddressName = String.Empty;

            if ((newContact.tsp_ContactAddress == null || newContact.tsp_ContactAddress.Id == Guid.Empty))
            {
                IsBillingAddressContact = true;
                _AddressCustomEntity = (from ad in ctx.tsp_accountaddressesSet where ad.tsp_Account.Id == newAccount.AccountId && ad.tsp_SpireID == _SpireBillingAddressID select ad).FirstOrDefault();
            }
            else
            {
                _AddressCustomEntity = (from ad in ctx.tsp_accountaddressesSet where ad.tsp_accountaddressesId == newContact.tsp_ContactAddress.Id select ad).FirstOrDefault();
                if (_AddressCustomEntity != null && !_AddressCustomEntity.tsp_name.Contains("Billing Address"))
                {
                    IsBillingAddressContact = false;
                    _AddressName = _AddressCustomEntity.tsp_name;
                }
                else IsBillingAddressContact = true;
            }

            if (_AddressCustomEntity != null) _LinkCustomAddressId = (Guid)_AddressCustomEntity.tsp_accountaddressesId;
            #endregion
              

            #region Update contact info in OOB Address if contact is Spire contact and not link to Billing
            if (newContact.tsp_ContacttoSpire == true && !IsBillingAddressContact &&_AddressCustomEntity!=null )
            {
                xrm.CustomerAddress _AddressOOB = (from ac in ctx.CustomerAddressSet
                                                   where ac.ParentId.Id == _accountId
                                                   where ac.Name == _AddressName
                                                   select new xrm.CustomerAddress
                                                   {
                                                       CustomerAddressId = ac.CustomerAddressId,
                                                       tsp_AddressID = ac.tsp_AddressID,
                                                       PrimaryContactName = ac.PrimaryContactName,
                                                       tsp_MainFirstName = ac.tsp_MainFirstName,
                                                       tsp_MainContactEmail = ac.tsp_MainContactEmail,
                                                       tsp_MainContactFax = ac.tsp_MainContactFax,
                                                       tsp_MainContactPhone = ac.tsp_MainContactPhone,
                                                       tsp_SalesContactEmail = ac.tsp_SalesContactEmail,
                                                       tsp_SalesContactFax = ac.tsp_SalesContactFax,
                                                       tsp_SalesFirstName = ac.tsp_SalesFirstName,
                                                       tsp_SalesContactName = ac.tsp_SalesContactName,
                                                       tsp_SalesContactPhone = ac.tsp_SalesContactPhone,
                                                       tsp_AccountingContactEmail = ac.tsp_AccountingContactEmail,
                                                       tsp_AccountingContactFax = ac.tsp_AccountingContactFax,
                                                       tsp_AccountingContactName = ac.tsp_AccountingContactName,
                                                       tsp_AccountingFirstName = ac.tsp_AccountingFirstName,
                                                       tsp_AccountingContactPhone = ac.tsp_AccountingContactPhone
                                                   }).FirstOrDefault<xrm.CustomerAddress>();

                OptionSetValue _contactType = new OptionSetValue();

                if (newContact.tsp_ContactType != null) _contactType.Value = newContact.tsp_ContactType.Value;
                else _contactType.Value = (int)ContactType.Main;
                switch (_contactType.Value)
                {
                    case (int)ContactType.Main:
                        _AddressOOB.PrimaryContactName = newContact.LastName;
                        _AddressOOB.tsp_MainFirstName = newContact.FirstName;
                        _AddressOOB.tsp_MainContactEmail = newContact.EMailAddress1;
                        _AddressOOB.tsp_MainContactFax = newContact.Fax;
                        _AddressOOB.tsp_MainContactPhone = newContact.MobilePhone;
                        break;

                    case (int)ContactType.Sales:
                        _AddressOOB.tsp_SalesContactName = newContact.LastName;
                        _AddressOOB.tsp_SalesFirstName = newContact.FirstName;
                        _AddressOOB.tsp_SalesContactEmail = newContact.EMailAddress1;
                        _AddressOOB.tsp_SalesContactFax = newContact.Fax;
                        _AddressOOB.tsp_SalesContactPhone = newContact.MobilePhone;
                        break;

                    case (int)ContactType.Accounting:
                        _AddressOOB.tsp_AccountingContactName = newContact.LastName;
                        _AddressOOB.tsp_AccountingFirstName = newContact.FirstName;
                        _AddressOOB.tsp_AccountingContactEmail = newContact.EMailAddress1;
                        _AddressOOB.tsp_AccountingContactFax = newContact.Fax;
                        _AddressOOB.tsp_AccountingContactPhone = newContact.MobilePhone;
                        break;
                }
                if (_AddressOOB.tsp_AddressID == null || String.IsNullOrEmpty(_AddressOOB.tsp_AddressID)) _AddressOOB.tsp_AddressID = _SpireBillingAddressID;

                if (!ctx.IsAttached(_AddressOOB)) ctx.Attach(_AddressOOB);
                ctx.UpdateObject(_AddressOOB);
                ctx.SaveChanges();

            }
                #endregion
          
              
                
                #region return Custom Address GUID to Link to new Contact
                return _LinkCustomAddressId;
                    //newContact.tsp_ContactAddress = new EntityReference(xrm.tsp_accountaddresses.EntityLogicalName, (Guid)_BillingAddressEntity);
                
                #endregion
          

          
        }
        #endregion

        #region Get Model From CRM Methods

        public OrderSpire GetModelOpportunityFromCRM(xrm.Opportunity op, xrm.Account ac)
        {
            #region Get Currency

            string CurrencyISOCode = "";
            if (ac.TransactionCurrencyId != null && ac.TransactionCurrencyId.Id != Guid.Empty)
            {
                CurrencyISOCode = (from cc in ctx.TransactionCurrencySet where cc.TransactionCurrencyId == ac.TransactionCurrencyId.Id select cc.ISOCurrencyCode).FirstOrDefault();
            }

            #endregion

            #region Get Ship To address

            string _ShipTo = String.Empty;
            AddressSpire ShipToModel = new AddressSpire();
      
            if (op.tsp_ShipTo != null && op.tsp_ShipTo.Id != Guid.Empty)
            {
                _ShipTo = (from cc in ctx.tsp_accountaddressesSet where cc.tsp_accountaddressesId == op.tsp_ShipTo.Id select cc.tsp_name).FirstOrDefault();
                if (!String.IsNullOrEmpty(_ShipTo) && _ShipTo!= "Billing Address")
                {
                    var _ShipToSpireId = (from ad in ctx.CustomerAddressSet where ad.Name == _ShipTo select ad).FirstOrDefault();
                    ShipToModel = GetModelAddressFromCRM(ac, _ShipToSpireId, false);
                }
                else ShipToModel = GetModelAddressFromCRM(ac, null, true);
            }
            else   ShipToModel = GetModelAddressFromCRM(ac, null, true);

            if (ShipToModel != null)
            {
                ShipToModel.type = "S";
                ShipToModel.linkNo = "";
                ShipToModel.linkTable = "";
                ShipToModel.shipId = "";
            }
            #endregion
            

            OrderItemSpire[] modelOrderItemsFromCRM = GetModelItemsFromCRM((Guid)op.OpportunityId);

            #region Create Spire Order Model
            OrderSpire _OrderSpire = new _360PlusPlugin.Models.OrderSpire()
            {
                customer = new CustomerSpire()
                {
                    id = int.Parse(ac.tsp_SpireID),
                    code = ac.AccountNumber,
                    customerNo = ac.AccountNumber
                },
                orderDate =  DateTime.Now.ToString("yyyy-MM-dd"),
                requiredDate = DateTime.Now.ToString("yyyy-MM-dd"),
                items = modelOrderItemsFromCRM,
                shippingAddress = ShipToModel,
                fob = op.tsp_FOB??""
            };
            
            return _OrderSpire;

            #endregion
        }

        public OrderItemSpire[] GetModelItemsFromCRM(Guid _opportunityId)
        {
            
            List<xrm.OpportunityProduct> _OpportunityProduct = (from it in ctx.OpportunityProductSet where it.OpportunityId.Id == _opportunityId select it).ToList();

            if (_OpportunityProduct != null && _OpportunityProduct.Count() > 0)
            {

                List<OrderItemSpire> Items = new List<OrderItemSpire>();

                
                foreach (var _productOpp in _OpportunityProduct)
                {
                    xrm.Product _Product = (from ac in ctx.ProductSet
                                                    where ac.ProductId == _productOpp.ProductId.Id
                                                    select new xrm.Product
                                                    {
                                                        ProductId = ac.ProductId,
                                                        ProductNumber = ac.ProductNumber,
                                                        tsp_SpireID = ac.tsp_SpireID
                                                    }).FirstOrDefault<xrm.Product>();
                                        
                    int SpireIdInt = 0;

                    #region Check If Item Exists in Spire and return ItemID

                    if (_Product != null && (_Product.tsp_SpireID == null || !int.TryParse(_Product.tsp_SpireID, out SpireIdInt)))
                    {
                        string _query = @"inventory/items/?filter= {" + "\"partNo" + "\":" + "\"" + _Product.ProductNumber + "\"}";

                        //https://localhost:10880/api/v1/companies/inspire2_10/inventory/items/?filter={"partNo":"WALEQBELHAR"}
                        string _SpireURL = GetSpireURL() + _query;
                        var _SpireItemID = CheckObjectExistsInSpire(_SpireURL, "id");

                        if (_SpireItemID != null && !_SpireItemID.Contains("Error"))
                        {
                            _Product.tsp_SpireID = _SpireItemID;
                            if (!ctx.IsAttached(_Product)) ctx.Attach(_Product);
                            ctx.UpdateObject(_Product);
                            ctx.SaveChanges();

                            int.TryParse(_SpireItemID, out SpireIdInt);
                        }
                       
                    }
                    else if (_Product != null && _Product.tsp_SpireID != null && int.TryParse(_Product.tsp_SpireID, out SpireIdInt))
                        int.TryParse(_Product.tsp_SpireID, out SpireIdInt);

                    #endregion

                    if (_Product != null && SpireIdInt != 0)
                    {
               Items.Add(new OrderItemSpire { sequence = _productOpp.SequenceNumber,
                                       inventory = new ItemsSpire() { id = SpireIdInt },
                                       partNo = _Product.ProductNumber,
                                       orderQty = _productOpp.Quantity.ToString() });
                        
                    }
                 }

                return Items.Cast<OrderItemSpire>().ToArray();
            }
            return null;
        }

        public CustomerAllItemsSpire GetModelAccountFromCRM( xrm.Account ac, xrm.CustomerAddress ad)
        {
            #region Get Currency

            string CurrencyISOCode = "";
            if (ac.TransactionCurrencyId != null && ac.TransactionCurrencyId.Id != Guid.Empty)
            {
                 CurrencyISOCode = (from cc in ctx.TransactionCurrencySet where cc.TransactionCurrencyId == ac.TransactionCurrencyId.Id select cc.ISOCurrencyCode).FirstOrDefault();
             }

            #endregion

            AddressSpire modelAddressFromCRM = GetModelAddressFromCRM( ac, ad, true);

            _360PlusPlugin.Models.paymentTermsSpire modelPaymentTermsFromCRM = new _360PlusPlugin.Models.paymentTermsSpire();

            if (ac.PaymentTermsCode != null) modelPaymentTermsFromCRM.id = ac.PaymentTermsCode.Value;
            else modelPaymentTermsFromCRM = null;


            #region Create Spire Customer Model
            return new _360PlusPlugin.Models.CustomerAllItemsSpire()
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
            string _email = String.Empty;

            #endregion

            #region Get Data from Account Or Address

            if (fromAccount)
                
            {
                _linkNo = ac.AccountNumber;
                if (ac.tsp_Country != null && ac.tsp_Country.Id != Guid.Empty)
                    _country = (from cc in ctx.tsp_countrySet where cc.tsp_countryId == ac.tsp_Country.Id select cc.tsp_Code).FirstOrDefault();
                else _country = "";
                _type = "B";
                _name = ac.Name;
                _linkTable = "CUST";
                _phone = ac.Telephone1 != null ? ac.Telephone1.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : "";
                _fax = ac.Fax != null ? ac.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : "";

            }
            if (!fromAccount)
            {
                _name = ad.Name;
               
                    if (ad.Country != null && ad.Country.Length == 3) _country = ad.Country; 
                    else _country = (from cc in ctx.tsp_countrySet where cc.tsp_name == ad.Country select cc.tsp_Code).FirstOrDefault();
                _type = ad.AddressTypeCode != null && ad.AddressTypeCode.Value == 1 ? "B": "S";
                _shipId = ad.Name;// ad.AddressTypeCode != null ? ad.FormattedValues["addresstypecode"].ToString() : "Ship To";
                _linkNo = "CUST" + ac.AccountNumber.ToString();// + "             " + _name;
                _linkTable = "SHIP";
                _phone = ad.Telephone1!=null? ad.Telephone1.Replace("(","").Replace(")","").Replace("-","").Replace(" ",""): "";
                _fax = ad.Fax != null ? ad.Fax.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : "";
                _email = ad.tsp_email ?? "";

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
                email = _email,
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

            if (!fromAccount || ad!=null)
            {
                Contacts[0] = GetModelContactFromCRM(ad.PrimaryContactName ?? "", ad.tsp_MainFirstName??"", ad.tsp_MainContactPhone ?? "", ad.tsp_MainContactFax ?? "", ad.tsp_MainContactEmail ?? "");
                Contacts[1] = GetModelContactFromCRM(ad.tsp_SalesContactName ?? "", ad.tsp_SalesFirstName ?? "", ad.tsp_SalesContactPhone ?? "", ad.tsp_SalesContactFax ?? "", ad.tsp_SalesContactEmail ?? "");
                Contacts[2] = GetModelContactFromCRM(ad.tsp_AccountingContactName ?? "", ad.tsp_AccountingFirstName ?? "", ad.tsp_AccountingContactPhone ?? "", ad.tsp_AccountingContactFax ?? "", ad.tsp_AccountingContactEmail ?? "");
            }
            else
            {
                Contacts[0] = Contacts[1] = Contacts[2]= GetModelContactFromCRM("", "","", "",  "");

            }
            AddressSpireModel.contacts = Contacts;
            return AddressSpireModel;

            #endregion
        }

        public ContactFirstLastName[] GetModelContactFirstLastNameFromCRM( xrm.CustomerAddress ad)
        {
            ContactFirstLastName[] Contacts = new ContactFirstLastName[3];

            Contacts[0] = new ContactFirstLastName()
            {
                fisrtName = !String.IsNullOrEmpty(ad.tsp_MainFirstName) ? ad.tsp_MainFirstName : "",
                lastName = !String.IsNullOrEmpty(ad.PrimaryContactName) ? ad.PrimaryContactName : "",
                contactSpire = new ContactSpire()
                {

                    phone = new PhoneSpire()
                    {
                        format = 1,
                        number = ad.tsp_MainContactPhone ?? ""
                    },

                    fax = new PhoneSpire()
                    {
                        format = 1,
                        number = ad.tsp_MainContactFax ?? ""
                    },
                    email = ad.tsp_MainContactEmail ?? ""
                },
            };

            Contacts[1] = new ContactFirstLastName()
            {
                fisrtName = !String.IsNullOrEmpty(ad.tsp_SalesFirstName) ? ad.tsp_SalesFirstName : "",
                lastName = !String.IsNullOrEmpty(ad.tsp_SalesContactName) ? ad.tsp_SalesContactName : "",
                contactSpire = new ContactSpire()
                {

                    phone = new PhoneSpire()
                    {
                        format = 1,
                        number = ad.tsp_SalesContactPhone ?? ""
                    },

                    fax = new PhoneSpire()
                    {
                        format = 1,
                        number = ad.tsp_SalesContactFax ?? ""
                    },
                    email = ad.tsp_SalesContactEmail ?? ""
                },
            };


            Contacts[2] = new ContactFirstLastName()
            {
                fisrtName = !String.IsNullOrEmpty(ad.tsp_AccountingFirstName) ? ad.tsp_AccountingFirstName : "",
                lastName = !String.IsNullOrEmpty(ad.tsp_AccountingContactName) ? ad.tsp_AccountingContactName : "",
                contactSpire = new ContactSpire()
                {

                    phone = new PhoneSpire()
                    {
                        format = 1,
                        number = ad.tsp_AccountingContactPhone ?? ""
                    },

                    fax = new PhoneSpire()
                    {
                        format = 1,
                        number = ad.tsp_AccountingContactFax ?? ""
                    },
                    email = ad.tsp_AccountingContactEmail ?? ""
                },
            };

            return Contacts;
        }
        public ContactSpire GetModelContactFromCRM(string _Name, string _FirstName, string _Phone, string _Fax, string _Email)
        {

            return new ContactSpire()
            {

                name = (!String.IsNullOrEmpty(_FirstName) ? _FirstName + " " : "") + (!String.IsNullOrEmpty(_Name) ? _Name : "" ),
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

        #endregion

        #region POST or PUT CRM records to Spire
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
                serializer  = new DataContractJsonSerializer(typeof(CustomerAllItemsSpire));
                else if(TypeSer == "address")
                    serializer = new DataContractJsonSerializer(typeof(AddressSpire));
                else if(TypeSer == "order")
                    serializer = new DataContractJsonSerializer(typeof(OrderSpire));
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
