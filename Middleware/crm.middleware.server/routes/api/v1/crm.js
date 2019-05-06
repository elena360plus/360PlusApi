const express = require("express");
const crm = require("node-dcrm-service");

const crmApi = express.Router();

let parameters = {
    baseUrl: process.env.CRM_BASE_URL,
    username: process.env.CRM_USERNAME,
    password: process.env.CRM_PASSWORD,
    domain: process.env.CRM_DOMAIN
};


const config = new crm.CrmAdConnectionConfig(parameters);
const crmService = new crm.CrmService(config);

let crmConnector = undefined
crmService.initialise()
    .then( crmConnector => console.log(crmConnector) )
    .catch( err => console.log("crmConnector::err: ", err) );


// crmService.get("campaigns($filter=contains(name,'(Sales)'))");
// console.log(JSON.parse(r.text));//r.body.UserId) 
// console.log(err);



crmApi.get("/", async (req, res) => 
{
    
    // dynamicsWebApi
    // const whoAmIResponse = await crmService.get("WhoAmI()");
    crmService.get("WhoAmI()")
        .then(whoAmIResponse => 
            {
                res.send( whoAmIResponse.body ); 
            });
});


crmApi.get("/campaigns/:id?", async (req, res) => 
{
    
    const query = "campaigns" + (req.param.id ? `(${req.param.id})` : "");
    console.log(query);
    crmService.get(query)
        .then(campaignsResponse => 
            {
                res.send( campaignsResponse.body ); 
            });
});


crmApi.post("/campaigns/:id?", async (req, res) => 
{
    
    const query = "campaigns" + (req.param.id ? `(${req.param.id})` : "");
    console.log(query);
    crmService.post(query)
        .then(campaignsResponse => 
            {
                res.send( campaignsResponse.body ); 
            });
});





module.exports = crmApi;