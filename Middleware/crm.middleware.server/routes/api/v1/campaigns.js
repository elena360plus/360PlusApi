const express = require("express");
const crm = require("node-dcrm-service");

const campaignsApi = express.Router();

const parameters = {
    baseUrl: process.env.CRM_BASE_URL,
    username: process.env.CRM_USERNAME,
    password: process.env.CRM_PASSWORD,
    domain: process.env.CRM_DOMAIN
};

const customFields = {
    "tsp_campaignurl": Number,
    "tsp_pagecode": Number,
    "tsp_noofsent": Number,
    "tsp_noofpageopenings": Number,
    "tsp_noofresponse": Number,
    "tsp_noofinterested": Number,
    "tsp_noofmaybelater": Number,
    "tsp_noofunsubscribe": Number,
}; 

const config = new crm.CrmAdConnectionConfig(parameters);
const crmService = new crm.CrmService(config);

crmService.initialise()
    .then( crmConnector => console.log(crmConnector) )
    .catch( err => console.log("crmConnector::err: ", err) );


    
function campaignQuery(id)
{
    const query = "campaigns" + (id ? `(${id})` : "");
    return query;
}

function loadCampaign(id)
{
    const query = campaignQuery(id);
    return new Promise( (resolve, reject)=>
    {
        crmService.get(query)
            .then(campaigns => resolve(campaigns.body) )
            .catch(err => reject(err) )
    });
}

function updateCampaign(id, entity)
{
    const query = campaignQuery(id);
    return crmService.patch(query, entity);    
}
// crmService.get("campaigns($filter=contains(name,'(Sales)'))");
// console.log(JSON.parse(r.text));//r.body.UserId) 
// console.log(err);


campaignsApi.get("/:id?", async (req, res) => 
{
    loadCampaign(req.params.id)
        .then(campaign => 
            {
                res.send( campaign ); 
            })
        .catch(err =>
            {
                res.send( err ); 
            })      
            // -- 
    // const query = campaignQuery(req.params.id);
    // console.log(query);
    // crmService.get(query)
    //     .then(campaignsResponse => 
    //         {
    //             res.send( campaignsResponse.body ); 
    //         });
});

campaignsApi.get("/:id/:value", async (req, res) => 
{
    try
    {
        const propName = `tsp_noof${req.params.value}`.toLocaleLowerCase();
        if ( customFields[propName] != Number )
            throw "unknown property name";

        const campaign = await loadCampaign(req.params.id);  
        if ( campaign && req.params.value)
        {
            
            
            // let tsp_noofpageopenings = Number.parseInt(campaign.tsp_noofpageopenings);
            // if ( isNaN(tsp_noofpageopenings) ) tsp_noofpageopenings = 0;
            let customValue = Number.parseInt(campaign[propName]);
            if ( isNaN(customValue) ) customValue = 0;
            

            var entity = {};
            entity[propName] = 1 + customValue;
            
            // name: campaign.name,
            // tsp_noofunsubscribe: 85,
            // tsp_noofresponse: 31,
            // tsp_noofpageopenings: tsp_noofpageopenings + 1, 
        
            const response = await updateCampaign(req.params.id, entity);
            res.send(response); 
        }
        else
        {
            res.sendStatus(404); 
        }
    }
    catch(err)
    {
        res.sendStatus(400, err);
    }
});
/*

Entity campaignResponse = new Entity("campaignresponse");
campaignResponse["subject"] = "Response from landing page";
campaignResponse["description"] = "Created by landing page";
campaignResponse["regardingobjectid"] = campaign.ToEntityReference();

*/

campaignsApi.post("/:id/response", (req, res) => 
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