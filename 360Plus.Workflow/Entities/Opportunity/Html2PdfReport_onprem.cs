using _360PlusPlugin.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace _360Plus.Workflow.Entities.Opportunity
{
    public class Html2PdfReport_onprem : WorkFlowActivityBase
    {
        protected override void ExecuteCrmWorkFlowActivity(CodeActivityContext context, LocalWorkflowContext localContext)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            IWorkflowContext wContext = localContext.WorkflowExecutionContext;
            IOrganizationService service = localContext.OrganizationService;
            ITracingService tracingService = localContext.TracingService;

            try
            {

            }
            catch (Exception ex)
            {
                tracingService.Trace(ex.Message);
                ExceptionRouter.handlePluginException(new InvalidPluginExecutionException(ex.Message), this.DisplayName, localContext.TracingService);
            }
        }
    }
}