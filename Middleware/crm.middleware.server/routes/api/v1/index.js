const apiV1 = require("express").Router();

apiV1.get("/", async (req, res)=>
{
    res.send("Hello from CRM-Middleware");
});


apiV1.use("/crm", require("./crm"));






module.exports = apiV1;