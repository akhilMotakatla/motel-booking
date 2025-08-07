import React from 'react';
import { Link } from 'react-router-dom';
import '../styles/Home.css';

function Home() {
  return (
    <div className="home-container">
      <div className="hero-section">
        <h1>Welcome to Starry Nights Motel</h1>
        <p>Comfortable stays, unforgettable memories. Book your next stay with us today!</p>
        <div className="cta-buttons">
          <Link to="/login" className="cta-btn">Book Now</Link>
          <Link to="/rooms" className="cta-btn">Explore Rooms</Link>
        </div>
      </div>

      <div className="features-section">
        <h2>Why Choose Us?</h2>
        <div className="features">
          <div className="feature-card">
            <h3>Clean & Cozy Rooms</h3>
            <p>Enjoy our clean and comfy rooms with all the modern amenities.</p>
          </div>
          <div className="feature-card">
            <h3>24/7 Front Desk</h3>
            <p>Our friendly staff is always available to assist you.</p>
          </div>
          <div className="feature-card">
            <h3>Free WiFi</h3>
            <p>Stay connected with complimentary high-speed internet.</p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Home;
