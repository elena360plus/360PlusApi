import React from 'react';
import Loading from "./Loading/Loading";
import './App.css';


class api
{
	campaign = async (id)=>
	{
		try
		{
			const campaign = await fetch(`/api/v1/crm/campaigns/${id}`);
			const data = await campaign.json();
			return data;
	
		}
		catch(err)
		{
			console.log(err);
		}
	}
	
}



class App extends React.Component
{
	server = new api();
	
	constructor(props)
	{
		super(props);
		
		// const {email, campaign} = props.params && props.params;
		this.state = {params: props.params};
	}


	async componentWillMount()
	{
		const campaign = await this.server.campaign(this.state.campaign);
		this.setState({campaign: campaign});
	}

	render()
	{
		console.log("campaign: ", this.state.campaign);

		if ( ! this.state.campaign )
			return ( <Loading />);

		return (<div>We've received your response</div>)
	}
}
// ?campaign=id&email=email@asdasd.com
export default App;
