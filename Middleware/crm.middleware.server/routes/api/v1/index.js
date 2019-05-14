const landing = require("../../../landing");
const apiV1 = require("express").Router();

apiV1.use("/campaigns", require("./campaigns"));

apiV1.get("/", async (req, res)=>
{
    res.send("Hello from CRM-Middleware");
});


apiV1.get("/crm", async (req, res) => 
{
    
    // dynamicsWebApi
    // const whoAmIResponse = await crmService.get("WhoAmI()");
    crmService.get("WhoAmI()")
        .then(whoAmIResponse => 
            {
                res.send( whoAmIResponse.body ); 
            });
});

apiV1.get("/landing", async (req, res) => 
{    
    res.send( landing.cards ); 
});



module.exports = apiV1;