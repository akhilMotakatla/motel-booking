// backend/routes/userRoutes.js

const express = require('express');
const router = express.Router();
const authMiddleware = require('../middleware/authMiddleware');
 // protect routes
const { getUserProfile, updateUserProfile, getUserBookings, bookRoom } = require('../controllers/userController');

// Get current user profile (protected)
router.get('/me', authMiddleware, getUserProfile);

// Update user profile (protected)
router.put('/me', authMiddleware, updateUserProfile);

// Get all bookings of the logged-in user
router.get('/bookings', authMiddleware, getUserBookings);

// Book a room (protected)
router.post('/bookings', authMiddleware, bookRoom);

module.exports = router;
