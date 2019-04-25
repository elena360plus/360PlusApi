const http = require("http");
const express = require("express");
const bodyParser = require('body-parser');

const app = express();

app
    .use( bodyParser.urlencoded({ extended: false }) )
    .use( bodyParser.json({limit: '50mb', type: 'application/json'}) )
    // .use( express.static(path.join(__dirname, 'www/build')) )

    .use("/api/v1", require("./routes/api/v1") );
    // .use("/resources", require("./routes/resources") );


// Handle React routing, return all requests to React app
app.get('*', function(req, res) 
{
    // console.log("get: ", req.baseUrl);
    res.sendFile(path.join(__dirname, 'www/build', 'index.html'));
});


const port = process.env.PORT ? process.env.PORT :  9999;

const httpServer = http.createServer(app);
httpServer.listen(port, ()=>
{
    console.log("server is running on port: %s", port);
});
