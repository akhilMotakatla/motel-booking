const express = require('express');
const router = express.Router();
const { getDB } = require('../database/db'); // Adjust path as needed
const authMiddleware = require('..backend/middleware/authMiddleware'); // Use auth for user routes

// Get bookings for logged in user
router.get('/', authMiddleware, async (req, res) => {
  try {
    const userId = req.user.id; // from authMiddleware
    const db = await getDB();

    const bookings = await db.all(
      `SELECT b.BookingID, r.Name as RoomName, b.CheckInDate, b.CheckOutDate, b.CheckInTime, b.CheckOutTime
       FROM Bookings b
       JOIN Rooms r ON b.RoomID = r.RoomID
       WHERE b.UserID = ?`,
      [userId]
    );

    res.json(bookings);
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Failed to get bookings' });
  }
});

// Create a new booking (logged in user)
router.post('/', authMiddleware, async (req, res) => {
  try {
    const userId = req.user.id;
    const { roomId, checkInDate, checkOutDate, checkInTime, checkOutTime } = req.body;

    if (!roomId || !checkInDate || !checkOutDate) {
      return res.status(400).json({ message: 'roomId, checkInDate and checkOutDate are required' });
    }

    const db = await getDB();
    const result = await db.run(
      `INSERT INTO Bookings (UserID, RoomID, CheckInDate, CheckOutDate, CheckInTime, CheckOutTime)
       VALUES (?, ?, ?, ?, ?, ?)`,
      [userId, roomId, checkInDate, checkOutDate, checkInTime || null, checkOutTime || null]
    );

    res.status(201).json({ message: 'Booking created', bookingId: result.lastID });
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Failed to create booking' });
  }
});

module.exports = router;
