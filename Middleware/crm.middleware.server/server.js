const path = require("path");
const express = require("express");
const bodyParser = require("body-parser");

// const fetch = require("node-fetch");

require("dotenv").config({path: path.join(__dirname, ".env") });

// const domain = "360plus.dev";
// const username = "Administrator";
// const password = "User12345";
// "http://win-p7or6uki9ss/PlusDev/api/data/v8.2/quotes"
// const url = `http://win-p7or6uki9ss/PlusDev/api/data/v8.2/quotes`;

// { CrmService, CrmResponse, CrmConnectionConfig, CrmO365ConnectionConfig, CrmAdConnectionConfig }
// http://blog.yagasoft.com/2018/09/connect-dynamics-crm-node
// https://docs.microsoft.com/en-us/dynamics365/customer-engagement/web-api/accountleads?view=dynamics-ce-odata-9


// await crmService.initialise();
 
const app = express();

app
    .use( bodyParser.urlencoded({ extended: false }) )
    .use( bodyParser.json({limit: '50mb', type: 'application/json'}) )
    // .use( express.static(path.join(__dirname, 'www/build')) )
    .use("/api/v1", require("./routes/api/v1") );


// Handle React routing, return all requests to React app
app.get('*', function(req, res) 
{
    res.sendFile(path.join(__dirname, 'www/build', 'index.html'));
});

const port = process.env.PORT ? process.env.PORT :  9999;
app.listen(port, ()=> console.log("CRM Middleware Server is running on port: %s", port) );