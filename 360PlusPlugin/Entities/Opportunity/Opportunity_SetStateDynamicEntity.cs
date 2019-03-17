using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace _360PlusPlugin.Entities.Opportunity
{
    public class Opportunity_SetStateDynamicEntity : PluginBase
    {
        #region Constructor/Configuration
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public Opportunity_SetStateDynamicEntity(string unsecure, string secureConfig)
            : base(typeof(Opportunity_SetStateDynamicEntity))
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

            // TODO: Implement your custom code

        }
    }
}