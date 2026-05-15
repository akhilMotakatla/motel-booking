// backend/routes/adminRoutes.js

const express = require('express');
const router = express.Router();
const authMiddleware = require('..backend/middleware/authMiddleware'); // protect routes
const { addRoom, getAllRooms, getAllBookings, getBookingDetails } = require('../controllers/adminController');

// Add a new room (protected)
router.post('/rooms', authMiddleware, addRoom);

// Get all rooms
router.get('/rooms', authMiddleware, getAllRooms);

// Get all bookings (past, current, future)
router.get('/bookings', authMiddleware, getAllBookings);

// Get booking details by booking ID
router.get('/bookings/:id', authMiddleware, getBookingDetails);

module.exports = router;
