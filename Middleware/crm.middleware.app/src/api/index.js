import {NotificationManager} from 'react-notifications';

class api
{
	constructor()
	{
		this.params = {};
		let searchParams = new URLSearchParams(window.location.search);
		
		this.params.email = searchParams.get("email");
		this.params.campaign = searchParams.get("campaign");
		this.params.response = searchParams.get("response");
		
		if ( this.params.campaign )
		{        
			this.campaign("pageopenings");

			searchParams.delete("email");
			searchParams.delete("campaign");
			searchParams.delete("response");
			
			this.sessionParams(this.params);
			if (window.history.replaceState) 
			{
				let params = searchParams.toString();
				if ( params.length > 0 ) params = `?${params}`;
				const url = `${window.location.origin}${window.location.pathname}${params}`

				window.history.replaceState({}, null, url);
			}        
		}
		else
		{
			this.params = this.sessionParams();
		}
		
		console.log("initialParams: ", this.params)	
	}

	landing = async ()=>
	{
		try
		{
			const landing = await fetch("/api/v1/landing");
			return await landing.json();
		}
		catch(err)
		{
			console.log(err);
		}
	}

	campaign = async (value)=>
	{
		try
		{
			if ( this.params.campaign )
			{
				const campaign = await fetch(`/api/v1/campaigns/${this.params.campaign}/${value ? value : ""}`);
				return await campaign.json();
			}
		}
		catch(err)
		{
			console.log(err);
			this.clearParams();
		}
	}
	
	interested = async (data)=>
	{
  		if ( this.params.campaign )
		{
			data.responsecode = 1;
			
			await this.campaign("response");
			await this.campaign("interested");

			return await this.campaignResponse(data);
		}
	}

	maybeLater = async (data)=>
	{
		// data.company || data.email

		if ( this.params.campaign )
		{
			data.responsecode = 2;
			
			await this.campaign("response");
			await this.campaign("maybelater");

			return await this.campaignResponse(data);
		}
	}

	unsubscribe = async (data)=>
	{
		if ( this.params.campaign )
		{
			data.responsecode = 3;

			await this.campaign("response");
			await this.campaign("unsubscribe");
			
			return await this.campaignResponse(data);
		}
	}

	campaignResponse = async (response) =>
	{
		try
		{
			if ( this.params.campaign )
			{
				const options = { 
					method: "POST", 
					body: JSON.stringify(response),
					headers: {"Content-Type": "application/json"}
				};

				const campaign = await fetch(`/api/v1/campaigns/${this.params.campaign}/response`, options);
				
				this.clearParams();
				return await campaign.json();
			}
		}
		catch(err)
		{
			console.log(err);
		}
	}

	clearParams()
	{
		const data = this.sessionParams();

		this.params = {};

		window.sessionStorage.removeItem("email");
		window.sessionStorage.removeItem("campaign");
		window.sessionStorage.removeItem("response");
		
		return data;
	}

	sessionParams(data = undefined)
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
	
}

export function showNotification(type, message, title = undefined, delay = undefined)
{
	// info | success | warning | error
	if ( typeof NotificationManager[type] == "function" )
	{
		NotificationManager[type](message, title, delay, 4000);

	}

	// switch (type) 
	// {
	// 	case 'info':
	// 		NotificationManager.info(message);
	// 		break;
	// 	case 'success':
	// 		NotificationManager.success(message, title);
	// 		break;
	// 	case 'warning':
	// 		NotificationManager.warning(message, title, delay);
	// 		break;
	// 	case 'error':
	// 		NotificationManager.error('Error message', 'Click me!', 5000, () => { });
	// 		break;
	// }
}


export default new api();