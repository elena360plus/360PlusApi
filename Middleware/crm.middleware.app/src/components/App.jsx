import React from 'react';
import api from "../api";
import {createNotification} from "../api/notifications";

import {NotificationContainer} from 'react-notifications';
import Loading from "./Loading/Loading";
import './App.css';

import Button from "react-bootstrap/Button";
import Jumbotron from "react-bootstrap/Jumbotron";
import Container from "react-bootstrap/Container";

import Unsubscribe from "./Pages/Unsubscribe";


class App extends React.Component
{
	constructor(props)
	{
		super(props);
		
		// const {email, campaign} = props.params && props.params;
		this.state = {params: props.params};
		console.log(this.state)
	}



	async componentWillMount()
	{
		const campaign = await api.campaign(this.state.params.campaign);
		this.setState({campaign: campaign});
	}

	render()
	{
		// console.log("campaign: ", this.state.campaign);

		if ( ! this.state.campaign )
			return ( <Loading />);

		// /yes
		// /no
		// /maybe

		return (
			<>
				<Jumbotron>
					<h1>Hello, John</h1>
					<p>
						We are sorry to see you go !
					</p>
				</Jumbotron>
				<Container>
					<Unsubscribe />
				</Container>

				<NotificationContainer/>
			</>
		)
	}
}
// ?campaign=id&email=email@asdasd.com
export default App;
