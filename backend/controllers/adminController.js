// backend/controllers/adminController.js

const db = require('../database/db');

// Add a new room
const addRoom = async (req, res) => {
  try {
    const { name, description, price, image } = req.body;
    await db.run(
      `INSERT INTO Rooms (Name, Description, Price, Image) VALUES (?, ?, ?, ?)`,
      [name, description, price, image]
    );
    res.json({ message: 'Room added successfully' });
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Server error' });
  }
};

// Get all rooms
const getAllRooms = async (req, res) => {
  try {
    const rooms = await db.all('SELECT * FROM Rooms');
    res.json(rooms);
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Server error' });
  }
};

// Get all bookings (admin view)
const getAllBookings = async (req, res) => {
  try {
    const bookings = await db.all(`
      SELECT B.BookingID, B.CheckInDate, B.CheckOutDate, B.CheckInTime, B.CheckOutTime,
             U.FullName, U.Email, U.Contact,
             R.Name as RoomName
      FROM Bookings B
      JOIN Users U ON B.UserID = U.UserID
      JOIN Rooms R ON B.RoomID = R.RoomID
      ORDER BY B.CheckInDate DESC
    `);
    res.json(bookings);
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Server error' });
  }
};

// Get booking details by ID
const getBookingDetails = async (req, res) => {
  try {
    const bookingId = req.params.id;
    const booking = await db.get(`
      SELECT B.BookingID, B.CheckInDate, B.CheckOutDate, B.CheckInTime, B.CheckOutTime,
             U.FullName, U.Email, U.Contact,
             R.Name as RoomName, R.Description, R.Price
      FROM Bookings B
      JOIN Users U ON B.UserID = U.UserID
      JOIN Rooms R ON B.RoomID = R.RoomID
      WHERE B.BookingID = ?
    `, [bookingId]);

    if (!booking) return res.status(404).json({ message: 'Booking not found' });
    res.json(booking);
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Server error' });
  }
};

module.exports = {
  addRoom,
  getAllRooms,
  getAllBookings,
  getBookingDetails,
};
