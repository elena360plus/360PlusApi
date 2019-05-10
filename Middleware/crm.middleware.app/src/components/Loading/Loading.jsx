import React from 'react';
import Container from "react-bootstrap/Container";
import './Loading.css';

function Loading()
{
    return (
        <Container>
            <div className="loading">
                Loading...
            </div>
        </Container>
    );
}

export default Loading;
