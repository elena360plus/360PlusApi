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
    public class Account_SetStateDynamicEntity : PluginBase
    {
        #region Constructor/Configuration
        private string _secureConfig = null;
        private string _unsecureConfig = null;
       

        public Account_SetStateDynamicEntity(string unsecure, string secureConfig)
            : base(typeof(Account_SetStateDynamicEntity))
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecure;
        }
        #endregion

        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));


            IPluginExecutionContext context = localContext.PluginExecutionContext;
            IOrganizationService service = localContext.OrganizationService;
            ITracingService tracingService = localContext.TracingService;
            var ctx = new xrm.XrmServiceContext(service);

            string Result = String.Empty;

        
            // if (context.MessageName == "Create" && context.InputParameters.Contains("Target"))
            try
            {

                if (context.InputParameters.Contains("Target"))
                {
                    Entity entity = (Entity)context.InputParameters["Target"];
                    Guid entityId = (Guid)entity.Id;
                    Result = new be.SpireHelper(ctx, context.PrimaryEntityName, entityId).SpirePostMethod_Account(entityId, service);

                }

            }

            catch (WebException ex)
            {
                ExceptionRouter.handlePluginException(ex, ChildClassName, tracingService);
            }

        }
        

    }
}