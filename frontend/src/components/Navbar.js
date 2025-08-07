// src/components/Navbar.js

import React from 'react';
import { Link } from 'react-router-dom';
import '../styles/Navbar.css';


const Navbar = () => {
  return (
    <nav className="navbar">
      <div className="navbar-logo">
        <Link to="/" className="navbar-title">Motel Booking</Link>
      </div>
      <ul className="navbar-links">
        <li><Link to="/login">User</Link></li>
        
        <li><Link to="/admin/login">Admin</Link></li>
      </ul>
    </nav>
  );
};

export default Navbar;
