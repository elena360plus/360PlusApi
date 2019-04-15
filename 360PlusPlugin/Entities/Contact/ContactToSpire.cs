using System;
using System.Linq;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using xrm = SpirePlusPlugin;
using be = Spire_BusinessEntities;
using _360PlusPlugin.Utility;
using _360PlusPlugin.Models;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;

namespace _360PlusPlugin.Entities.Contact
{
    public class ContactIntoSpire : IPlugin
    {
        #region Constructor/Configuration
        private string _secureConfig = null;
        private string _unsecureConfig = null;
        private const string contactEntity = "contact";
        private const string accountEntity = "account";
        private const string addressEntity = "customeraddress";
        private const string customaddressEntity = "tsp_accountaddresses";

        //public AccountIntoSpire(string unsecure, string secureConfig)
        //    : base(typeof(AccountIntoSpire))
        //{
        //    _secureConfig = secureConfig;
        //    _unsecureConfig = unsecure;
        //}
        #endregion

        public void Execute(IServiceProvider serviceProvider)
        {


            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            // Get the notification service from the service provider.
            IServiceEndpointNotificationService NotificationService = (IServiceEndpointNotificationService)serviceProvider.GetService(typeof(IServiceEndpointNotificationService));
            // Obtain the organization factory service from the service provider.
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            // Use the factory to generate the organization service.
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);
            if (context.Depth > 1) return;

            var ctx = new xrm.XrmServiceContext(service);

            Guid LinkToAddress = Guid.Empty;
            string Result = String.Empty;
            string MessageName = context.MessageName;

          //  if (context.MessageName == "Create" && context.InputParameters.Contains("Target"))
            try
            {

                if ( context.InputParameters.Contains("Target"))
                {
                    Entity entity = (Entity)context.InputParameters["Target"];
                    Guid entityId = (Guid)entity.Id;
                    bool _ParsToSpire = false;
                    Entity postEntity = null;
                    if (context.PostEntityImages.Contains("Target")) postEntity = context.PostEntityImages["Target"];

                    if (context.PrimaryEntityName == contactEntity)
                    {
  
                      LinkToAddress = new be.SpireHelper(ctx, contactEntity, entityId).CreateUpdateContactInCustomAddress(entityId, service);

                        if (LinkToAddress != Guid.Empty)
                        {
                         if (entity.Contains("tsp_contacttospire") && entity["tsp_contacttospire"] != null && (bool)entity["tsp_contacttospire"] == true) _ParsToSpire = true;
                          else if (postEntity.Contains("tsp_contacttospire") && postEntity["tsp_contacttospire"] != null && (bool)postEntity["tsp_contacttospire"] == true) _ParsToSpire = true;

                            if (!_ParsToSpire && MessageName == "Update" && entity.Contains("tsp_contacttospire") && entity["tsp_contacttospire"] != null && (bool)entity["tsp_contacttospire"] == false) _ParsToSpire = true;

                            if (_ParsToSpire)
                            {
                             var _CustomAddressEntity = service.Retrieve(customaddressEntity, LinkToAddress, new Microsoft.Xrm.Sdk.Query.ColumnSet(new string[] { "tsp_name" }));
                                if (_CustomAddressEntity["tsp_name"] != null && !String.IsNullOrEmpty(_CustomAddressEntity["tsp_name"].ToString())
                                     && _CustomAddressEntity["tsp_name"].ToString().Contains("Billing Address"))

                                    if (postEntity.Contains("parentcustomerid") && (EntityReference)(postEntity.Attributes["parentcustomerid"]) != null)
                                    {
                                        Guid _AddressIdForContact = ((EntityReference)(postEntity.Attributes["parentcustomerid"])).Id;

                                     if(_AddressIdForContact != Guid.Empty) Result = new be.SpireHelper(ctx, addressEntity, LinkToAddress).SpirePostMethod_Address(_CustomAddressEntity.Id, service, MessageName, 2, _AddressIdForContact);

                                    }
                            }
                        }

                        
                            if (!postEntity.Contains("tsp_contactaddress") || (EntityReference)(postEntity.Attributes["tsp_contactaddress"]) == null || ((EntityReference)(postEntity.Attributes["tsp_contactaddress"])).Id == Guid.Empty)
                            {

                                if (LinkToAddress != Guid.Empty)
                                {
                                    Entity _contact = new Entity("contact");
                                    _contact.Id = entityId;
                                    if (!_contact.Contains("tsp_contactaddress"))
                                        _contact.Attributes.Add("tsp_contactaddress", new EntityReference(xrm.tsp_accountaddresses.EntityLogicalName, LinkToAddress));
                                    else _contact["tsp_contactaddress"] = new EntityReference(xrm.tsp_accountaddresses.EntityLogicalName, LinkToAddress);
                                    service.Update(_contact);

                                }
                            }
                        
                    }
                } 

            }

            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message );
            }

        }
        

    }
}