using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace _360PlusPlugin.Utility
{
    public class GlobalHelper
    {


        #region Global Methods


        /// <summary>
        /// returns an attribute string value from entity
        /// Returns empty string if nothing found
        /// </summary>
        /// <param name="en"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string GetStringValue(Entity en, string attributeName)
        {
            string retVal = "";
            if (en != null && isValidString(en.GetAttributeValue<string>(attributeName)))
            {
                retVal = en.GetAttributeValue<string>(attributeName);
            }
            return retVal;
        }

        /// <summary>
        /// returns an attribute string value from entity
        /// if it doesn't have it checks second parameter - entity image
        /// Returns empty string if nothing found
        /// </summary>
        /// <param name="en"></param>
        /// <param name="preImage"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string GetStringValue(Entity en, Entity enImage, string attributeName)
        {

            string retVal = GetStringValue(en, attributeName);
            return retVal != ""
                ? retVal
                : GetStringValue(enImage, attributeName);
        }

        /// <summary>
        /// Returns an option set integer value from entity
        /// Returns 0 if nothing found
        /// </summary>
        /// <param name="en"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static int GetOptionsetValue(Entity en, string attributeName)
        {
            int retVal = 0;
            if (en != null && en.Attributes.ContainsKey(attributeName) && en.GetAttributeValue<OptionSetValue>(attributeName) != null)
            {
                retVal = en.GetAttributeValue<OptionSetValue>(attributeName).Value;
            }
            return retVal;
        }

        /// <summary>
        /// Returns an option set integer value from entity
        /// if it doesn't have it checks second parameter - entity image
        /// Returns 0 if nothing found
        /// </summary>
        /// <param name="en"></param>
        /// <param name="enImage"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static int GetOptionsetValue(Entity en, Entity enImage, string attributeName)
        {
            int retVal = GetOptionsetValue(en, attributeName);

            return retVal != 0
                ? retVal
                : GetOptionsetValue(enImage, attributeName);
        }

        public static bool isValidString(string source)
        {
            return !string.IsNullOrEmpty(source) && !string.IsNullOrWhiteSpace(source) && source != "&nbsp;";
        }

        public static void SetAttributeValue(Entity entity, string attributename, object value)
        {
            if (!entity.Attributes.Contains(attributename))
            {
                entity.Attributes.Add(attributename, value);
            }
            else
            {
                entity.Attributes[attributename] = value;
            }
        }

        /// <summary>
        /// JSON to object of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string json)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(json);
                writer.Flush();
                stream.Position = 0;
                T responseObject = (T)serializer.ReadObject(stream);
                return responseObject;
            }
        }

        /// <summary>
        /// Object of type T to JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(object obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
                //stream.Position = 0;
                //StreamReader reader = new StreamReader(stream);
                //string requestBody = reader.ReadToEnd();
                //return requestBody;
            }
        }

        /// <summary>
        /// converts QueryExpression to FetchXml
        /// </summary>
        /// <param name="service"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public static string getFetchString(IOrganizationService service, QueryExpression current)
        {

            // Convert the query expression to FetchXML.
            var conversionRequest = new QueryExpressionToFetchXmlRequest
            {
                Query = current
            };
            var conversionResponse =
                (QueryExpressionToFetchXmlResponse)service.Execute(conversionRequest);

            return conversionResponse.FetchXml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="entity"></param>
        /// <param name="entityImage"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeValue(string attributeName, Entity entity, Entity entityImage = null)
        {
            DateTime targetDate = DateTime.MinValue;
            if (entity != null
                && entity.Attributes.Keys.Contains(attributeName)
                && entity.GetAttributeValue<DateTime>(attributeName) > DateTime.MinValue)
            {
                targetDate = entity.GetAttributeValue<DateTime>(attributeName);
            }
            else if (entityImage != null
                && entityImage.Attributes.Keys.Contains(attributeName)
                && (entityImage.GetAttributeValue<DateTime>(attributeName) > DateTime.MinValue))
            {
                targetDate = entityImage.GetAttributeValue<DateTime>(attributeName);
            }
            return targetDate;
        }

        public static double getTimeStampInSecondsDouble()
        {

            return (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static Int32 getTimeStampInSecondsInt()
        {

            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static string GetStringValueFromConfig(IOrganizationService service, string configName)
        {
            string retValue = "";
            //QueryExpression qe = new QueryExpression(Constants.Entities.Config);
            //qe.ColumnSet = new ColumnSet(
            //    Constants.ConfigFields.Name,
            //    Constants.ConfigFields.String1

            //);
            //qe.Criteria = new FilterExpression();
            //qe.Criteria.FilterOperator = LogicalOperator.And;
            //qe.Criteria.AddCondition(Constants.ConfigFields.Name, ConditionOperator.Equal, configName);
            //EntityCollection results = service.RetrieveMultiple(qe);

            //if (results != null && results.Entities != null && results.Entities.Count > 0)
            //{
            //    Entity apiToken = results.Entities[0];
            //    retValue = apiToken.GetAttributeValue<string>(Constants.ConfigFields.String1);
            //}
            return retValue;
        }

        #endregion Global Methods
        
        #region Attachments, Annotations, WebResources

        public static XmlDocument retrieveXMLWebResource(IOrganizationService service, string webresourceName)
        {
            XmlDocument xmlDoc = new XmlDocument();

            string result = string.Empty;
            QueryByAttribute query = new QueryByAttribute("webresource");
            query.ColumnSet = new ColumnSet(new string[] { "content" });
            query.Attributes.AddRange("name");
            query.Values.AddRange(webresourceName);

            Entity webResource = null;
            EntityCollection wrCollection = service.RetrieveMultiple(query);

            if (!ReferenceEquals(wrCollection.Entities, null)
                && wrCollection.Entities.Count > 0)
            {
                webResource = wrCollection.Entities[0];
                byte[] binary = Convert.FromBase64String(webResource.Attributes["content"].ToString());
                string resourceContent = Encoding.UTF8.GetString(binary);
                string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                if (resourceContent.StartsWith("\""))
                {
                    resourceContent = resourceContent.Remove(0, byteOrderMarkUtf8.Length);
                }

                xmlDoc.LoadXml(resourceContent);

            }
            return xmlDoc;
        }

        public static string retrieveHTML_JS_CSS_WebResource(IOrganizationService service, string webresourceName, ITracingService TracingService)
        {

            string result = string.Empty;
            QueryByAttribute query = new QueryByAttribute("webresource");
            query.ColumnSet = new ColumnSet(new string[] { "content" });
            query.Attributes.AddRange("name");
            query.Values.AddRange(webresourceName);

            Entity webResource = null;
            EntityCollection wrCollection = service.RetrieveMultiple(query);

            TracingService.Trace("retrieveHTMLWebResource wrCollection.Entities " + wrCollection.Entities.Count);


            if (!ReferenceEquals(wrCollection.Entities, null)
                && wrCollection.Entities.Count > 0)
            {
                webResource = wrCollection.Entities[0];
                byte[] binary = Convert.FromBase64String(webResource.Attributes["content"].ToString());
                string resourceContent = Encoding.UTF8.GetString(binary);
                //string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                //result = byteOrderMarkUtf8;
                result = resourceContent;

                TracingService.Trace("retrieveHTMLWebResource result " + result.Length);

            }
            return result;
        }

        public static Guid addAttachmentToEmail(IOrganizationService service, Guid emailId, string fileName, string documentBody, string mimeType, string subject, ITracingService TracingService)
        {
            TracingService.Trace("Attach begins");

            Entity attachment = new Entity("activitymimeattachment");
            attachment["subject"] = subject;
            attachment["filename"] = fileName;
            attachment["mimetype"] = mimeType;
            attachment["body"] = documentBody;
            attachment["attachmentnumber"] = 1;
            attachment["objectid"] = new EntityReference("email", emailId);
            attachment["objecttypecode"] = "email";

            try
            {
                TracingService.Trace("Create attachment");

                Guid attachmentId = service.Create(attachment);
                return attachmentId;
            }
            catch (Exception ex)
            {
                TracingService.Trace("Exception:" + ex.Message + "; trace: " + ex.StackTrace);
                throw new InvalidPluginExecutionException(ex.Message);

            }

            return new Guid();
        }

        public static Guid? createAnnotationDocument(IOrganizationService service, string documentbody, string filename, string mimetype, string subject, EntityReference objectid, string objectTypeCode)
        {
            Entity note = new Entity("annotation");

            note["documentbody"] = documentbody;
            note["filename"] = filename;
            note["isdocument"] = true;
            note["mimetype"] = mimetype;
            note["objectid"] = objectid;
            note["objecttypecode"] = objectTypeCode;
            note["subject"] = subject;

            return service.Create(note);

        }

        #endregion

        #region quotes

        public static void quoteToOrder(IOrganizationService service, Entity quote, bool deleteOrder)
        {

            //set quote won
            WinQuoteRequest winQuoteRequest = new WinQuoteRequest();



            Entity QuoteClose = new Entity("quoteclose");
            
            QuoteClose["subject"] = "Quote Close" + DateTime.Now.ToString();
            QuoteClose["quoteid"] = quote.ToEntityReference();
            //service.Create(QuoteClose);
            winQuoteRequest.Status = new OptionSetValue(-1);
            winQuoteRequest.QuoteClose = QuoteClose;
            service.Execute(winQuoteRequest);
            
            //setQuoteStatus(service, quote.ToEntityReference(), 1, 4);//active, won

            ColumnSet salesOrderColumns = new ColumnSet("salesorderid", "totalamount");

            // Convert the quote to a sales order
            ConvertQuoteToSalesOrderRequest convertQuoteRequest =
                new ConvertQuoteToSalesOrderRequest()
                {
                    QuoteId = quote.Id,
                    ColumnSet = salesOrderColumns
                };
            ConvertQuoteToSalesOrderResponse convertQuoteResponse =
                (ConvertQuoteToSalesOrderResponse)service.Execute(convertQuoteRequest);
            if (deleteOrder)
            {
                Entity OrderClose = new Entity("orderclose");
                OrderClose["salesorderid"] = convertQuoteResponse.Entity.ToEntityReference();
                OrderClose["subject"] = "Close Sales Order " + DateTime.Now;

                CancelSalesOrderRequest cancelRequest = new CancelSalesOrderRequest()
                {
                    OrderClose = OrderClose,
                    Status = new OptionSetValue(-1)
                };
                service.Execute(cancelRequest);
                service.Delete("salesorder", convertQuoteResponse.Entity.Id);

            }
        }

        public static void setQuoteStatus(IOrganizationService service, EntityReference quoteRef, int statecode, int statuscode)
        {

            SetStateRequest activateQuote = new SetStateRequest()
            {
                EntityMoniker = quoteRef,
                State = new OptionSetValue(statecode), 
                Status = new OptionSetValue(statuscode) 
            };
            service.Execute(activateQuote);

        }

        #endregion quotes

    }
}
