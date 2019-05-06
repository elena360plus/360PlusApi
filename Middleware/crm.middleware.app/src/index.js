import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './components/App';
import * as serviceWorker from './serviceWorker';


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
        if (window.history.replaceState) 
        {
            let params = searchParams.toString();
            if ( params.length > 0 ) params = `?${params}`;
            window.history.replaceState({}, null, `${window.location.origin}${params}`);                
        }        
    }
    
    return initialParams;
}


ReactDOM.render(<App params={getParams()}/>, document.getElementById('root'));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
