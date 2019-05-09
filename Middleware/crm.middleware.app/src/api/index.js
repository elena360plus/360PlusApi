class api
{
	campaign = async (id)=>
	{
		try
		{
			const campaign = await fetch(`/api/v1/campaigns/${id}`);
            return await campaign.json();
		}
		catch(err)
		{
			console.log(err);
		}
	}
	
}

export default new api();