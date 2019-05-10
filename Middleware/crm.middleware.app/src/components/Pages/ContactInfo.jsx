import React from 'react';
import {withRouter} from 'react-router-dom';
import api, {showNotification} from "../../api";

import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col";
import Form from "react-bootstrap/Form";
import Button from "react-bootstrap/Button";
import Container from "react-bootstrap/Container";
import Jumbotron from "react-bootstrap/Jumbotron";

const formFields = [
    { name: "firstName", title: "First Name", mandatory: false },
    { name: "lastName", title: "Last Name", mandatory: true },
    { name: "company", title: "Company", mandatory: true },
    { name: "email", title: "eMail", mandatory: false },
    { name: "phone", title: "Phone", mandatory: false },
];

function FormRow({name, type="text", title, required, onChange=undefined})
{
    return (
        <Form.Group as={Row}>
            <Form.Label column sm={3}>{title}</Form.Label>
            <Col sm={9}>
                <Form.Control name={name} type={type} placeholder={title} 
                                required={required} onChange={onChange}/>
            </Col>
        </Form.Group>        
    );
}
//{mandatory}
class ContactInfo extends React.Component
{
    state = {
        data: {},
        validated: false,
    };

    constructor(props)
    {
        super(props);
        this.state.isInvalid = props.mandatoryOnly ? true : undefined;
        // this.state.mandatory = props.mandatory == true ? 
    }

    handleSubmit = async (e)=>
    {
        e.preventDefault();
        e.stopPropagation();

        this.setState({ validated: true });


        for(let i=0; i < formFields.length; i++)
        {
            if ( (!this.props.mandatoryOnly || formFields[i].mandatory) 
                    && !this.state.data[formFields[i].name] )
                return;
        }


        showNotification("success", "Thank you for your feedback");
        
        // lastname: req.body.lastName,
        // firstname: req.body.firstName,
        // telephone: req.body.phone,
        // companyname: req.body.company,
        // emailaddress: req.body.email,
                
        if ( this.props.mandatoryOnly )
        {
            await api.maybeLater(this.state.data);
        }
        else
        {
            await api.interested(this.state.data);
        }

        this.props.history.push('/');
    }

    handleFormChange = (e)=>
    {
        // console.log(e);
        if ( e.target.name )
        {
            let data = {...this.state.data};
            data[e.target.name] = e.target.value;

            let isInvalid = undefined;
            if ( !this.props.mandatoryOnly )
            {
                isInvalid = !(data.company || data.lastName);
            }
            

            this.setState({data, isInvalid, disabledAction: isInvalid});
        }
        console.log(this.state.data);
    }
    
    hasMandatoryValue = ()=>
    {
        if ( this.props.mandatoryOnly )
        {
            for(let i=0; i < formFields.length; i++)
            {
                if ( formFields[i].mandatory && this.state.data[formFields[i].name] )
                {
                    return true;
                }
            }
        }        
        return false;
    }

    render()
    {
        let oneOfMandatory = this.hasMandatoryValue();

        return (
            
            <Container>
                <Jumbotron>

                    <div className="row">
                        <div className="col-md-6">
                            <h1>Hello, John</h1>
                            {/* <h2>We are sorry to see you go !</h2> */}
                        </div>
                        <div className="col-md-6">
                            {/* <img src="img/bizDev.png" style={ {height: "140px"} }/> */}
                        </div>
                    </div>            
            
                </Jumbotron>					
                <Form noValidate validated={this.state.validated} onSubmit={this.handleSubmit}
                        style={ {maxWidth: "500px", "margin": "auto"} } >
                    {
                        formFields.map( (field)=>
                        {
                            return (
                                <FormRow key={field.name} name={field.name} title={field.title} type={field.type} 
                                            required={this.props.mandatoryOnly ? field.mandatory && !oneOfMandatory : true}
                                            onChange={this.handleFormChange} />
                            );
                        })
                    }
                    
                    <hr />
                    <Button type="submit" variant="success" block style={{float: "right"}} 
                            onClick={this.handleSubmit}>Submit</Button>

{/* disabled={this.state.disabledAction}  */}
                </Form>        
            </Container>
        )
    }
}


export default withRouter(ContactInfo);