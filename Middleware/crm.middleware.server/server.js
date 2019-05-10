const path = require("path");
const express = require("express");
const bodyParser = require("body-parser");

// const fetch = require("node-fetch");

require("dotenv").config({path: path.join(__dirname, ".env") });
 

const app = express();

app
    .use( bodyParser.urlencoded({ extended: false }) )
    .use( bodyParser.json({limit: '50mb', type: 'application/json'}) )

    .use( express.static(path.join(__dirname, 'www/build')) )
    .use("/api/v1", require("./routes/api/v1") );


// Handle React routing, return all requests to React app
app.get('*', function(req, res) 
{
    res.sendFile(path.join(__dirname, 'www/build', 'index.html'));
});

const port = process.env.PORT ? process.env.PORT :  9999;
app.listen(port, ()=> console.log("CRM Middleware Server is running on port: %s", port) );