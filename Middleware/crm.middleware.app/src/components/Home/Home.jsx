import React from 'react';
import Container from "react-bootstrap/Container";
import "./Home.css";

const cards = [
    {
        title: "Customer",
        image: "/img/autopilot@2x.png",
        description: "Lorem ipsum dolor sit amet consectetur adipisicing elit. Natus harum cum tempore dicta molestias modi consectetur laborum saepe itaque. Nisi."
    },
    {
        title: "Relationship",
        image: "/img/project@2x.png",
        description: "Lorem ipsum dolor sit amet consectetur adipisicing elit. Natus harum cum tempore dicta molestias modi consectetur laborum saepe itaque. Nisi."
    },
    {
        title: "Management",
        image: "img/sales@2x.png",
        description: "Lorem ipsum dolor sit amet consectetur adipisicing elit. Natus harum cum tempore dicta molestias modi consectetur laborum saepe itaque. Nisi."
    }        
]

//   
function Card({card})
{
    return (
        <div className="box">
            <div className="imgBx">
                <img src={card.image} />
            </div>
            <div className="content">
                <h3>{card.title}</h3>
                <p>{card.description}</p>
            </div>
        </div>   
    );
}

function Home()
{
    return (
        <div className="page-body">

            <div className="container">
            {
                cards.map( (card, index)=> <Card key={index} card={card} />)
            }
            </div>
        </div>
    );
}

export default Home;
