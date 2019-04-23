using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CampaignResponseDemo
{
    class Program
    {


        static void Main(string[] args)
        {
            //incomming parameters: email, campaign name
            string email = "test1@test.tes";
            string campaignName = "Test Campaign 1";
            //variables
            Entity product = null;
            Guid result = Guid.Empty;

            //get crm connection with connection string
            CrmServiceClient crmConn = new CrmServiceClient(ConfigurationManager.ConnectionStrings["SpireCrm"].ConnectionString);
            string er = crmConn.LastCrmError;
            Exception ex = crmConn.LastCrmException;


            if (crmConn.IsReady)
            {
                //get lead by email
                Entity lead = getEntityByAttr(crmConn, "lead", "emailaddress1", email);
                if (ReferenceEquals(lead, null))
                {
                    Console.Write($"No Lead found for email {email}");
                    Console.Read();
                    return;
                }

                //get campaign by name
                Entity campaign = getEntityByAttr(crmConn, "campaign", "name", campaignName);
                if (ReferenceEquals(campaign, null))
                {
                    Console.Write($"No Campaign with name {campaignName} found");
                    Console.Read();
                    return;
                }

                //get first of associated products if any
                //campaignproduct_association
                QueryExpression query = new QueryExpression("campaign");
                query.Distinct = true;
                LinkEntity linkEntity1 = new LinkEntity("campaign", "campaignitem",  "campaignid", "campaignid", JoinOperator.Inner);
                LinkEntity linkEntity2 = new LinkEntity("campaignitem", "product",  "campaignitemid", "productid", JoinOperator.Inner);
                FilterExpression filter = new FilterExpression(LogicalOperator.And);
                ConditionExpression cond1 = new ConditionExpression("campaignid", ConditionOperator.Equal, campaign.Id);
                filter.AddCondition(cond1);
                query.Criteria = filter;
                linkEntity1.LinkEntities.Add(linkEntity2);
                query.LinkEntities.Add(linkEntity1);
                linkEntity2.Columns = new ColumnSet(new string[] { "name" });
                linkEntity2.EntityAlias = "Prod";
                query.ColumnSet = new ColumnSet(new string[] { "name" });

                EntityCollection products = crmConn.RetrieveMultiple(query);
                if (!ReferenceEquals(products, null) && !ReferenceEquals(products.Entities, null) && products.Entities.Count > 0)
                {
                    product = products.Entities[0];
                }

                //create campaign response
                Entity campaignResponse = new Entity("campaignresponse");
                campaignResponse["subject"] = "Response from landing page";
                    
                //campaignResponse["subject"] = (product == null)
                //    ? "Response from landing page"
                //    : $"Interestd in {((string)product.GetAttributeValue<AliasedValue>("Prod.name").Value)}";
                campaignResponse["description"] = "Created by landing page";
                campaignResponse["regardingobjectid"] = campaign.ToEntityReference();

                try
                {
                    result = crmConn.Create(campaignResponse);

                }
                catch (Exception e)
                {
                    Console.Write($"Error on campaign response create: {e.Message}");
                    Console.Read();
                    return;
                }
            }

            Console.Write($"Done. Campaign response id: {result.ToString("D")}");
            Console.Read();
        }

        private static Entity getEntityByAttr(CrmServiceClient service, string entityName, string attrName, string attrValue, string[] columnSet = null)
        {
            QueryByAttribute query = new QueryByAttribute(entityName);

            query.AddAttributeValue(attrName, attrValue);
            if(ReferenceEquals(columnSet, null)) query.ColumnSet = new ColumnSet(true);
            else query.ColumnSet = new ColumnSet(columnSet);

            EntityCollection enColl = service.RetrieveMultiple(query);
            //if any result - return first
            if (!ReferenceEquals(enColl, null) && !ReferenceEquals(enColl.Entities, null) && enColl.Entities.Count > 0)
            {
                return enColl.Entities[0];
            }

            //no results - return null
            return null;
        }
    }
}
