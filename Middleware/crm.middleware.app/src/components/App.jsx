import React from 'react';
import { BrowserRouter as Router, Route, Link, Switch, Redirect } from "react-router-dom";

import api from "../api";
import {NotificationContainer} from 'react-notifications';
import Loading from "./Loading/Loading";
import './App.css';

// import Button from "react-bootstrap/Button";
import Nav from "react-bootstrap/Nav"
import Navbar from "react-bootstrap/Navbar";
// import Container from "react-bootstrap/Container";
// import Jumbotron from "react-bootstrap/Jumbotron";

import Home from "./Home/Home";
import ContactInfo from "./Pages/ContactInfo";
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
		const campaign = await api.campaign();
		this.setState({campaign: campaign});
	}

	render()
	{
		if ( api.params.campaign && ! this.state.campaign )
		{
			return <Loading />;
		}


		return (
			<>
				<Navbar bg="dark" variant="dark">
					<Navbar.Brand>
						{this.state.campaign && this.state.campaign.name}
					</Navbar.Brand>
					<Nav className="mr-auto"></Nav>
					<Navbar.Text>360+ IT</Navbar.Text>
				</Navbar>			

				<Router>
					{/* <Route path="/" exact component={Home} /> */}

					<Switch>
						<Route path="/interested" render={ (e) => <ContactInfo mandatoryOnly={false} /> } />
						<Route path="/notInterested" render={ (e) => <ContactInfo mandatoryOnly={true} /> } />

						<Route path="/unsubscribe" render={ (e) => <Unsubscribe />} />
						<Route path="*" component={Home} />
					</Switch> 

				</Router>

				<NotificationContainer/>
			</>
		)
	}
}
// ?campaign=id&email=email@asdasd.com
export default App;
