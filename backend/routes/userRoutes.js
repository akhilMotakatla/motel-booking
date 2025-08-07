const express = require('express');
const router = express.Router();
const authMiddleware = require('../middleware/authMiddleware');
const poolPromise = require('../database/db').poolPromise;  // adjust path as per your project

// Get current user info
router.get('/me', authMiddleware, async (req, res) => {
  try {
    const pool = await poolPromise;
    const result = await pool.request()
      .input('UserID', req.user.id)
      .query('SELECT UserID, FullName, Email, Address, City, State, ZipCode, Contact FROM Users WHERE UserID = @UserID');

    if (result.recordset.length === 0) return res.status(404).json({ message: 'User not found' });

    res.json(result.recordset[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ message: 'Server error' });
  }
});

// Update user profile
router.put('/me', authMiddleware, async (req, res) => {
  const { fullName, email, address, city, state, zipCode, contact } = req.body;
  try {
    const pool = await poolPromise;
    await pool.request()
      .input('UserID', req.user.id)
      .input('FullName', fullName)
      .input('Email', email)
      .input('Address', address)
      .input('City', city)
      .input('State', state)
      .input('ZipCode', zipCode)
      .input('Contact', contact)
      .query(`
        UPDATE Users SET 
          FullName = @FullName, 
          Email = @Email, 
          Address = @Address, 
          City = @City, 
          State = @State, 
          ZipCode = @ZipCode, 
          Contact = @Contact
        WHERE UserID = @UserID
      `);

    res.json({ message: 'Profile updated successfully' });
  } catch (err) {
    console.error(err);
    res.status(500).json({ message: 'Server error' });
  }
});

module.exports = router;
