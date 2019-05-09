const express = require("express");
const crm = require("node-dcrm-service");

const campaignsApi = express.Router();

let parameters = {
    baseUrl: process.env.CRM_BASE_URL,
    username: process.env.CRM_USERNAME,
    password: process.env.CRM_PASSWORD,
    domain: process.env.CRM_DOMAIN
};


const config = new crm.CrmAdConnectionConfig(parameters);
const crmService = new crm.CrmService(config);

function createCampaignQuery(id)
{
    const query = "campaigns" + (id ? `(${id})` : "");
    return query;
}



crmService.initialise()
    .then( crmConnector => console.log(crmConnector) )
    .catch( err => console.log("crmConnector::err: ", err) );


// crmService.get("campaigns($filter=contains(name,'(Sales)'))");
// console.log(JSON.parse(r.text));//r.body.UserId) 
// console.log(err);


campaignsApi.get("/campaigns/:id?", async (req, res) => 
{
    
    const query = "campaigns" + (req.param.id ? `(${req.param.id})` : "");
    console.log(query);
    crmService.get(query)
        .then(campaignsResponse => 
            {
                res.send( campaignsResponse.body ); 
            });
});


campaignsApi.get("/campaigns/:id/visit", async (req, res) => 
{
    
    const query = "campaigns" + (req.param.id ? `(${req.param.id})` : "");
    console.log(query);
    
    var entity = { 
        name: "Sales",
        tsp_noofpageopenings: 7, 
        // "regardingobjectid_campaign@odata.bind": `/campaigns(${req.params.id})`,
    };

    crmService.post(query, entity)
        .then(campaign => 
            {
                res.send( campaign ); 
            })
        .catch(err =>
            {
                res.send( err ); 
            })            
});
/*

Entity campaignResponse = new Entity("campaignresponse");
campaignResponse["subject"] = "Response from landing page";
campaignResponse["description"] = "Created by landing page";
campaignResponse["regardingobjectid"] = campaign.ToEntityReference();

*/

campaignsApi.post("/campaigns/:id/response", (req, res) => 
{
    var entity = { 
        subject: req.body.subject ? req.body.subject : "Response from email", 
        responsecode: req.body.responsecode,
        "regardingobjectid_campaign@odata.bind": `/campaigns(${req.params.id})`,
    };  

    crmService.post("campaignresponses", entity)
        .then(campaignsResponse => 
            {
                res.send( campaignsResponse ); 
            })
        .catch(err =>
            {
                res.send( err ); 
            })
});




module.exports = campaignsApi;