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

namespace _360PlusPlugin.Entities.Account
{
    public class AccountIntoSpire : IPlugin
    {
        #region Constructor/Configuration
        private string _secureConfig = null;
        private string _unsecureConfig = null;
        private const string accountEntity = "account";
        private const string addressEntity = "customeraddress";

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

            if (context.PrimaryEntityName == addressEntity && context.Depth > 2) return;
            var ctx = new xrm.XrmServiceContext(service);

            string Result = String.Empty;
            string MessageName = context.MessageName;

          //  if (context.MessageName == "Create" && context.InputParameters.Contains("Target"))
            try
            {

                if (context.InputParameters.Contains("Target"))
                {
                    Entity entity = (Entity)context.InputParameters["Target"];
                    Guid entityId = (Guid)entity.Id;

                    if (context.PrimaryEntityName == accountEntity)
                    {

                        Result = new be.SpireHelper(ctx, context.PrimaryEntityName, entityId).SpirePostMethod_Account(entityId, service, MessageName, context.Depth);

                        if (!String.IsNullOrEmpty(Result) && !Result.Contains("Error"))
                        {

                            string[] resultIds = Result.Split(':');
                            
                            Entity _account = new Entity("account");
                            _account.Id = entityId;
                            if (!_account.Contains("tsp_spireid"))
                                _account.Attributes.Add("tsp_spireid", resultIds[0]);
                            else _account["tsp_spireid"] = resultIds[0];


                            if (!String.IsNullOrEmpty(resultIds[1]))
                                {

                                if (!_account.Contains("tsp_billingaddressid"))
                                    _account.Attributes.Add("tsp_billingaddressid", resultIds[1]);
                                else _account["tsp_billingaddressid"] = resultIds[1];

                               }

                            service.Update(_account);
                        }

                        else if(!String.IsNullOrEmpty(Result) && Result.Contains("Error")) {
                            throw new ArgumentException(Result);
                        }
                    }
                    else if (context.PrimaryEntityName == addressEntity)
                    {

                        Result = new be.SpireHelper(ctx, context.PrimaryEntityName, entityId).SpirePostMethod_Address(entityId, service, MessageName, context.Depth, Guid.Empty);

                        if (!String.IsNullOrEmpty(Result) && !Result.Contains("Error"))
                        {

                            Entity _address = new Entity(context.PrimaryEntityName);
                            _address.Id = entityId;
                            if (!_address.Contains("tsp_addressid"))
                                _address.Attributes.Add("tsp_addressid", Result);
                            else _address["tsp_addressid"] = Result;
                            service.Update(_address);
                        }

                        else if (!String.IsNullOrEmpty(Result) && Result.Contains("Error"))
                        {
                            throw new ArgumentException(Result);
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