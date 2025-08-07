import React from 'react';
import { Link } from 'react-router-dom';
import '../styles/Rooms.css';

function Rooms() {
  const roomList = [
    {
      name: "Deluxe Room",
      description: "Spacious room with queen-sized bed, AC, private bath, and city view.",
      price: "$89/night",
      image: "/images/room1.jpg"
    },
    {
      name: "Executive Suite",
      description: "Luxury suite with king-sized bed, lounge area, mini bar, and balcony.",
      price: "$129/night",
      image: "/images/room2.jpg"
    },
    {
      name: "Standard Room",
      description: "Comfortable room with double bed, free WiFi, and television.",
      price: "$59/night",
      image: "/images/room3.jpg"
    }
  ];

  return (
    <div className="rooms-container">
      <h2>Our Rooms</h2>
      <p>Choose from our comfortable, clean, and cozy accommodations.</p>

      <div className="room-cards">
        {roomList.map((room, index) => (
          <div className="room-card" key={index}>
            <img src={room.image} alt={room.name} className="room-image" />
            <h3>{room.name}</h3>
            <p>{room.description}</p>
            <p className="room-price">{room.price}</p>
            <Link to="/login" className="book-btn">Book Now</Link>
          </div>
        ))}
      </div>
    </div>
  );
}

export default Rooms;
