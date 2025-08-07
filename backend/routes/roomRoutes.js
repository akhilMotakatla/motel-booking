const express = require('express');
const router = express.Router();
const poolPromise = require('../database/db').poolPromise;

// Get all rooms
router.get('/', async (req, res) => {
  try {
    const pool = await poolPromise;
    const result = await pool.request().query('SELECT RoomID, Name, Description, Price FROM Rooms');
    res.json(result.recordset);
  } catch (err) {
    console.error(err);
    res.status(500).json({ message: 'Server error' });
  }
});

module.exports = router;
