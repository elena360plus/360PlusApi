import React from 'react';
// import Container from "react-bootstrap/Container";
import "./Home.css";
import api from '../../api';

const cards1 = [
    {
        title: "Customer",
        image: "/img/autopilot@2x.png",
        description: `<b>Improved Customer Satisfaction</b><hr/> Collecting and organizing actionable customer data is a full-time job, and one that isn’t very forgiving of mistakes. As such, investing in a high-quality Customer Relationship Management (CRM) tool is a must for any business that wants to take customer satisfaction to the next level.`
    },
    {
        title: "Relationship",
        image: "/img/project@2x.png",
        description: "<b>Greater efficiency</b><hr/> Automatically stored communication allows you to view emails, calendar and phone call details in one easily accessible place. Add that to the ability for multiple teams to access the same information, it simply sky rockets the amount of achievable progress. Sales, marketing, and customer service teams can share valuable information about clients to continue to funnel them down the pipeline to get the desired result of closing a sale, knowledge of new products, or excellent customer service."
    },
    {
        title: "Management",
        image: "img/sales@2x.png",
        description: "<b>Customer Service</b><hr/> With CRM, as soon as a customer contacts your company, your representatives will be able to retrieve all available activity concerning past purchases, preferences, and anything else that might assist them in finding a solution."
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
                {/* <p>{card.description}</p> */}
                <p dangerouslySetInnerHTML={ {__html: card.description} }></p>
            </div>
        </div>   
    );
}

function Home()
{
    const [cards, setCards] = React.useState([]);

    React.useEffect( ()=>
    {
        api.landing()
            .then( landing =>
            {
                console.log(Array.isArray(landing), landing);
                Array.isArray(landing) && setCards(landing);
            })
            .catch(err => console.log(err))
    }, []);

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
