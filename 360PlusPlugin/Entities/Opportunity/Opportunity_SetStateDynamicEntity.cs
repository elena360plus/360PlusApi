using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using xrm = SpirePlusPlugin;
using be = Spire_BusinessEntities;

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
            var ctx = new xrm.XrmServiceContext(service);
            string Result = String.Empty;

            if (context.InputParameters.Contains("OpportunityClose") && context.InputParameters["OpportunityClose"] is Entity)
            {
                Entity entity = (Entity)context.InputParameters["OpportunityClose"];
                if (entity.Attributes.Contains("opportunityid") && entity.Attributes["opportunityid"] != null)
                {

                    EntityReference opportunity = (EntityReference)entity.Attributes["opportunityid"];
                    if (opportunity.LogicalName == "opportunity")
                    {
                        Guid OppId = opportunity.Id;

                        Result = new be.SpireHelper(ctx, context.PrimaryEntityName, OppId).SpirePostMethod_WonOpportunity(OppId, service, "POST");


                        if (!String.IsNullOrEmpty(Result) && !Result.Contains("Error"))
                        {

                            Entity _opportunity = new Entity(context.PrimaryEntityName);
                            _opportunity.Id = OppId;
                            if (!_opportunity.Contains("tsp_orderno"))
                                _opportunity.Attributes.Add("tsp_orderno", Result);
                            else _opportunity["tsp_orderno"] = Result;
                            service.Update(_opportunity);
                        }

                        else if (!String.IsNullOrEmpty(Result) && Result.Contains("Error"))
                        {
                            throw new ArgumentException(Result);
                        }


                    }
                }
                
            }

           
        

    }
    }
}