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


            var ctx = new xrm.XrmServiceContext(service);

            string Result = String.Empty;

        
            // if (context.MessageName == "Create" && context.InputParameters.Contains("Target"))
            try
            {

                if (context.InputParameters.Contains("Target"))
                {
                    Entity entity = (Entity)context.InputParameters["Target"];
                    Guid entityId = (Guid)entity.Id;

                    if (context.PrimaryEntityName == accountEntity)
                    {

                        Result = new be.SpireHelper(ctx, context.PrimaryEntityName, entityId).SpirePostMethod_Account(entityId, service);

                        if (!String.IsNullOrEmpty(Result))
                        {

                            Entity _account = new Entity("account");
                            _account.Id = entityId;
                            if (!_account.Contains("tsp_spireid"))
                                _account.Attributes.Add("tsp_spireid", Result);
                            else _account["tsp_spireid"] = Result;
                            service.Update(_account);
                        }
                    }
                    else if (context.PrimaryEntityName == addressEntity)
                    {

                        Result = new be.SpireHelper(ctx, context.PrimaryEntityName, entityId).SpirePostMethod_Address(entityId, service);


                    }
                }

            }

            catch (Exception ex)
            {
                
            }

        }
        

    }
}