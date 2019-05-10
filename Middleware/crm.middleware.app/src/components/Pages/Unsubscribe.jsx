import React from 'react';
import api, {showNotification} from "../../api";

import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col";
import Form from "react-bootstrap/Form";
import Button from "react-bootstrap/Button";
import Container from "react-bootstrap/Container";
import Jumbotron from "react-bootstrap/Jumbotron";

// import { BrowserRouter as Router, Route, Link, Switch, Redirect } from "react-router-dom";
import {withRouter} from 'react-router-dom';


class Unsubscribe extends React.Component
{

    state = {
        description: ""
    };

    handleUnsubscribe = (e) => 
    {        
        showNotification("success", "Thank you for your feedback");
        api.unsubscribe({description: this.state.description});
        this.props.history.push('/');
    }

    render()
    {
        return (
            <Container>
                <Jumbotron>
                    <h1>Hello, John</h1>
                    <h2>We are sorry to see you go !</h2>
                </Jumbotron>					
                <Form style={ {maxWidth: "500px", "margin": "auto"} }>
        
                    <Form.Group as={Row}>
                        <Form.Label column sm={3}>Reason: </Form.Label>
                        <Col sm={9}>
                            <Form.Control as="textarea" rows="3" 
                                            onChange={ (e)=> this.setState({description: e.target.value}) } />
                        </Col>
                    </Form.Group>

                    <hr />
                    <Button variant="success" block style={{float: "right"}}
                                onClick={this.handleUnsubscribe}>Unsubscribe</Button>
                </Form>        
            </Container>
        )
    }
}

export default withRouter(Unsubscribe);