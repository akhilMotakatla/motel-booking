// backend/controllers/userController.js

const db = require('../database/db'); // your sqlite DB helper or connection
const { v4: uuidv4 } = require('uuid');

// Get user profile
const getUserProfile = async (req, res) => {
  try {
    const userId = req.user.id;
    const user = await db.get('SELECT UserID, FullName, Email, Address, City, State, ZipCode, Contact FROM Users WHERE UserID = ?', [userId]);
    if (!user) return res.status(404).json({ message: 'User not found' });
    res.json(user);
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Server error' });
  }
};

// Update user profile
const updateUserProfile = async (req, res) => {
  try {
    const userId = req.user.id;
    const { fullName, email, address, city, state, zipCode, contact } = req.body;
    await db.run(
      `UPDATE Users SET FullName = ?, Email = ?, Address = ?, City = ?, State = ?, ZipCode = ?, Contact = ? WHERE UserID = ?`,
      [fullName, email, address, city, state, zipCode, contact, userId]
    );
    res.json({ message: 'Profile updated successfully' });
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Server error' });
  }
};

// Get user's bookings
const getUserBookings = async (req, res) => {
  try {
    const userId = req.user.id;
    const bookings = await db.all(`SELECT * FROM Bookings WHERE UserID = ? ORDER BY CheckInDate`, [userId]);
    res.json(bookings);
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Server error' });
  }
};

// Book a room
const bookRoom = async (req, res) => {
  try {
    const userId = req.user.id;
    const { roomId, checkInDate, checkOutDate, checkInTime, checkOutTime } = req.body;
    
    // Add validation here if needed
    
    await db.run(
      `INSERT INTO Bookings (UserID, RoomID, CheckInDate, CheckOutDate, CheckInTime, CheckOutTime) VALUES (?, ?, ?, ?, ?, ?)`,
      [userId, roomId, checkInDate, checkOutDate, checkInTime, checkOutTime]
    );
    res.json({ message: 'Room booked successfully' });
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Server error' });
  }
};

module.exports = {
  getUserProfile,
  updateUserProfile,
  getUserBookings,
  bookRoom,
};
