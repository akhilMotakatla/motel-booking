const express = require('express');
const router = express.Router();
const authMiddleware = require('../middleware/authMiddleware');
const poolPromise = require('../database/db').poolPromise;

// Get bookings for logged-in user
router.get('/user', authMiddleware, async (req, res) => {
  try {
    const pool = await poolPromise;
    const result = await pool.request()
      .input('UserID', req.user.id)
      .query(`
        SELECT b.BookingID, b.CheckInDate, b.CheckOutDate, b.CheckInTime, b.CheckOutTime, r.Name AS RoomName
        FROM Bookings b
        INNER JOIN Rooms r ON b.RoomID = r.RoomID
        WHERE b.UserID = @UserID
        ORDER BY b.CheckInDate DESC
      `);

    res.json(result.recordset);
  } catch (err) {
    console.error(err);
    res.status(500).json({ message: 'Server error' });
  }
});

// Create a new booking
router.post('/', authMiddleware, async (req, res) => {
  const { roomId, checkInDate, checkOutDate, checkInTime, checkOutTime } = req.body;
  if (!roomId || !checkInDate || !checkOutDate) {
    return res.status(400).json({ message: 'Missing required booking fields' });
  }
  try {
    const pool = await poolPromise;
    await pool.request()
      .input('UserID', req.user.id)
      .input('RoomID', roomId)
      .input('CheckInDate', checkInDate)
      .input('CheckOutDate', checkOutDate)
      .input('CheckInTime', checkInTime || null)
      .input('CheckOutTime', checkOutTime || null)
      .query(`
        INSERT INTO Bookings (UserID, RoomID, CheckInDate, CheckOutDate, CheckInTime, CheckOutTime)
        VALUES (@UserID, @RoomID, @CheckInDate, @CheckOutDate, @CheckInTime, @CheckOutTime)
      `);

    res.status(201).json({ message: 'Booking created successfully' });
  } catch (err) {
    console.error(err);
    res.status(500).json({ message: 'Server error' });
  }
});

module.exports = router;
