const express = require('express');
const router = express.Router();
const { getDB } = require('../database/db'); // Adjust path as needed

// Get all rooms
router.get('/', async (req, res) => {
  try {
    const db = await getDB();
    const rooms = await db.all('SELECT * FROM Rooms');
    res.json(rooms);
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Failed to get rooms' });
  }
});

// Add a new room (Admin only - add authMiddleware if you have)
router.post('/', async (req, res) => {
  try {
    const { name, description, price, image } = req.body;
    if (!name || !price) return res.status(400).json({ message: 'Name and price are required' });

    const db = await getDB();
    const result = await db.run(
      'INSERT INTO Rooms (Name, Description, Price, Image) VALUES (?, ?, ?, ?)',
      [name, description, price, image || null]
    );
    res.status(201).json({ message: 'Room added', roomId: result.lastID });
  } catch (error) {
    console.error(error);
    res.status(500).json({ message: 'Failed to add room' });
  }
});

module.exports = router;
