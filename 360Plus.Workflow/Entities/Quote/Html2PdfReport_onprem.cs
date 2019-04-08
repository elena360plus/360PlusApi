using _360PlusPlugin.Utility;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Text;
using SelectPdf;
using System.IO;

namespace _360Plus.Workflow.Entities.Quote
{
    public class Html2PdfReport_onprem : WorkFlowActivityBase
    {

        [RequiredArgument]
        [Input("Current Quote")]
        [ReferenceTarget("quote")]
        public InArgument<EntityReference> quoteRef
        {
            get; set;
        }

        [RequiredArgument]
        [Input("Quote Template:Web Resource with HTML content(js)")]
        public InArgument<string> qWebResourceName
        {
            get; set;
        }

        [RequiredArgument]
        [Input("Quote Items Template: Web Resource with HTML content(js)")]
        public InArgument<string> qItemsWebResourceName
        {
            get; set;
        }


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
                bool isProducts = false;

                //get the quote
                Entity theQuote = service.Retrieve("quote", quoteRef.Get<EntityReference>(context).Id, new ColumnSet(true));
                if (ReferenceEquals(quoteRef, null))
                    throw new InvalidPluginExecutionException("No quote can be retrieved");

                //parent opportunity ref
                EntityReference opp = theQuote.GetAttributeValue<EntityReference>("opportunityid");
                if (ReferenceEquals(opp, null))
                {
                    tracingService.Trace("Html2PdfReport_onprem :  No origin opportunity found");
                    return;
                }
                
                                
                //get web resource names
                string quoteTemplateName = qWebResourceName.Get<string>(context);
                if (!GlobalHelper.isValidString(quoteTemplateName))
                    throw new InvalidPluginExecutionException("The 'qWebResourceName' argument passed to workflow is empty");

                string qItemsTemplateName = qItemsWebResourceName.Get<string>(context);
                if (!GlobalHelper.isValidString(qItemsTemplateName))
                    throw new InvalidPluginExecutionException("The 'qItemsWebResourceName' argument passed to workflow is empty");

                //get html
                string htmlQuote = GlobalHelper.retrieveHTML_JS_CSS_WebResource(service, quoteTemplateName, tracingService);
                if (!GlobalHelper.isValidString(htmlQuote))
                    throw new InvalidPluginExecutionException("The web resource " + quoteTemplateName + " is empty");

                string htmlQuoteItem = GlobalHelper.retrieveHTML_JS_CSS_WebResource(service, qItemsTemplateName, tracingService);
                if (!GlobalHelper.isValidString(htmlQuoteItem))
                    throw new InvalidPluginExecutionException("The web resource " + qItemsTemplateName + " is empty");

                //get opportunity products //opportunityid
                QueryByAttribute query = new QueryByAttribute("quotedetail");
                query.AddAttributeValue("quoteid", theQuote.Id);
                query.ColumnSet = new ColumnSet(true);
                EntityCollection items = service.RetrieveMultiple(query);
                if (!ReferenceEquals(items, null) && !ReferenceEquals(items.Entities, null) && items.Entities.Count > 0)
                    isProducts = true;

                #region update html

                StringBuilder sbQuote = new StringBuilder(htmlQuote);

                sbQuote.Replace("{{comapnyName}}", theQuote.Contains("customerid") ? theQuote.GetAttributeValue<EntityReference>("customerid").Name : string.Empty);
                sbQuote.Replace("{{quoteNumber}}", theQuote.Contains("quotenumber") ? theQuote.GetAttributeValue<string>("quotenumber") : string.Empty);
                sbQuote.Replace("{{quoteDate}}", theQuote.Contains("effectivefrom") && theQuote.GetAttributeValue<DateTime?>("effectivefrom").HasValue ? theQuote.GetAttributeValue<DateTime?>("effectivefrom").Value.ToString("MM/dd/yyyy") : string.Empty);
                sbQuote.Replace("{{customerNumber}}", theQuote.Contains("revisionnumber") && theQuote.GetAttributeValue<int?>("revisionnumber").HasValue ? theQuote.GetAttributeValue<int?>("revisionnumber").Value.ToString() : string.Empty);

                //get billing address
                if (theQuote.Contains("billto_addressid"))
                {
                    sbQuote.Replace("{{billToCompany}}", theQuote.Contains("billto_name") 
                        ? theQuote.GetAttributeValue<string>("billto_name") : string.Empty);
                    sbQuote.Replace("{{billToStreet}}", theQuote.Contains("billto_line1") 
                        ? theQuote.GetAttributeValue<string>("billto_line1") : string.Empty);
                    string address = "";
                    if (theQuote.Contains("billto_city"))
                        address += (theQuote.GetAttributeValue<string>("billto_city") + ", ");
                    if (theQuote.Contains("billto_stateorprovince"))
                        address += (theQuote.GetAttributeValue<string>("billto_stateorprovince") + ", ");
                    if (theQuote.Contains("billto_country"))
                        address += (theQuote.GetAttributeValue<string>("billto_country") + " ");
                    if (theQuote.Contains("billto_postalcode"))
                        address += theQuote.GetAttributeValue<string>("billto_postalcode");
                    sbQuote.Replace("{{billToAddress}}", address);
                }
                //get shipping address
                if (theQuote.Contains("shipto_addressid"))
                {
                    sbQuote.Replace("{{shipToCompany}}", theQuote.Contains("shipto_name") 
                        ? theQuote.GetAttributeValue<string>("shipto_name") : string.Empty);
                    sbQuote.Replace("{{shipToStreet}}", theQuote.Contains("shipto_line1") 
                        ? theQuote.GetAttributeValue<string>("shipto_line1") : string.Empty);
                    string address = "";
                    if (theQuote.Contains("shipto_city"))
                        address += (theQuote.GetAttributeValue<string>("shipto_city") + ", ");
                    if (theQuote.Contains("shipto_stateorprovince"))
                        address += (theQuote.GetAttributeValue<string>("shipto_stateorprovince") + ", ");
                    if (theQuote.Contains("shipto_country"))
                        address += (theQuote.GetAttributeValue<string>("shipto_country") + " ");
                    if (theQuote.Contains("shipto_postalcode"))
                        address += theQuote.GetAttributeValue<string>("shipto_postalcode");
                    sbQuote.Replace("{{shipToAddress}}", address);
                }
                

                if (isProducts)
                {
                    StringBuilder sbItems = new StringBuilder();


                    foreach (Entity prdct in items.Entities)
                    {

                        StringBuilder sbItemRow = new StringBuilder(htmlQuoteItem);

                        bool writeIn = prdct.Contains("isproductoverridden") && prdct.GetAttributeValue<bool?>("isproductoverridden").HasValue
                            ? prdct.GetAttributeValue<bool?>("isproductoverridden").Value
                            : false;
                        if (writeIn)
                        {
                            sbItemRow.Replace("{{itemPartNumber}}", "N/A");
                            sbItemRow.Replace("{{itemDescription}}", prdct.Contains("productdescription") ? theQuote.GetAttributeValue<string>("productdescription") : string.Empty);
                        }
                        else
                        {
                            EntityReference productRef = prdct.GetAttributeValue<EntityReference>("productid");
                            if (!ReferenceEquals(productRef, null))
                            {
                                Entity product = service.Retrieve("product", productRef.Id, new ColumnSet(true));
                                sbItemRow.Replace("{{itemPartNumber}}", prdct.Contains("productnumber") ? theQuote.GetAttributeValue<string>("productnumber") : string.Empty);
                                sbItemRow.Replace("{{itemDescription}}", prdct.Contains("name") ? theQuote.GetAttributeValue<string>("name") : string.Empty);
                            }

                        }

                        //sbItems.Replace("{{itemOrder}}", prdct.Contains("") ? theQuote.GetAttributeValue<string>("") : string.Empty);
                        //sbItems.Replace("{{itemShipped}}", prdct.Contains("") ? theQuote.GetAttributeValue<string>("") : string.Empty);
                        //sbItems.Replace("{{itemBO}}", prdct.Contains("") ? theQuote.GetAttributeValue<string>("") : string.Empty);
                        //sbItems.Replace("{{itemTax}}", prdct.Contains("") ? theQuote.GetAttributeValue<string>("") : string.Empty);
                        //sbItems.Replace("{{itemUnitPrice}}", prdct.Contains("") ? theQuote.GetAttributeValue<string>("") : string.Empty);
                        //sbItems.Replace("{{itemExtendedPrice}}", prdct.Contains("") ? theQuote.GetAttributeValue<string>("") : string.Empty);

                        sbItems.Append(sbItemRow);
                    }

                    //add products to quote
                    sbQuote.Replace("{{quoteItems}}", sbItems.ToString());
                }


                #endregion update html

                string html = sbQuote.ToString();
                string htmlNoSpaces = html.Replace("&nbsp;", string.Empty);

                //convert and append
                HtmlToPdf converter = new HtmlToPdf();
                PdfDocument doc = converter.ConvertHtmlString(htmlNoSpaces);
                byte[] bytes = doc.Save();
                string fileBody = Convert.ToBase64String(bytes);

                //application/octet-stream  "application/pdf" text/plain
                string fileNameSubject = theQuote.GetAttributeValue<string>("name");
                Guid? pdfNote = GlobalHelper.createAnnotationDocument(service, fileBody, fileNameSubject + ".pdf", "application/octet-stream", fileNameSubject, opp, "opportunity");

            }
            catch (Exception ex)
            {
                tracingService.Trace(ex.Message);
                ExceptionRouter.handlePluginException(new InvalidPluginExecutionException(ex.Message), this.DisplayName, localContext.TracingService);
            }
        }
    }
}