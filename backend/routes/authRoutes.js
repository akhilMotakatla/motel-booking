const express = require('express');
const router = express.Router();


const { registerUser, loginUser, registerAdmin, loginAdmin } = require('../controllers/authController');

// User routes
router.post('/user/register', registerUser);
router.post('/user/login', loginUser);

// Admin routes
router.post('/admin/register', registerAdmin);
router.post('/admin/login', loginAdmin);

module.exports = router;
