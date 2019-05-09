import React from 'react';
import ReactDOM from 'react-dom';
import App from './components/App';
import * as serviceWorker from './serviceWorker';
import "bootstrap/dist/css/bootstrap.min.css";
import "react-notifications/lib/notifications.css";
import './index.css';

function getParams()
{
    let initialParams = {};
    let searchParams = new URLSearchParams(window.location.search);
    
    initialParams.email = searchParams.get("email");
    initialParams.campaign = searchParams.get("campaign");
    initialParams.response = searchParams.get("response");
    
    if ( initialParams.campaign )
    {        
        searchParams.delete("email");
        searchParams.delete("campaign");
        searchParams.delete("response");
        
        sessionParams(initialParams);

        if (window.history.replaceState) 
        {
            let params = searchParams.toString();
            if ( params.length > 0 ) params = `?${params}`;
            window.history.replaceState({}, null, `${window.location.origin}${params}`);                
        }        
    }
    else
    {
        initialParams = sessionParams();
    }
    
    console.log("initialParams: ", initialParams)
    return initialParams;
}

function sessionParams(data = undefined)
{
    
    if ( ! data )
    {
        data = {};
        
        data.email = window.sessionStorage.getItem("email");
        data.campaign = window.sessionStorage.getItem("campaign");
        data.response = window.sessionStorage.getItem("response");
    }
    else
    {
        window.sessionStorage.setItem("email", data.email);
        window.sessionStorage.setItem("campaign", data.campaign);
        window.sessionStorage.setItem("response", data.response);
    }

    return data;
}


ReactDOM.render(<App params={ getParams() }/>, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
