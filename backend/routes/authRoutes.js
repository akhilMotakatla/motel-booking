const express = require('express');
const router = express.Router();
const authController = require(newFunction());

// User routes
router.post('/register/user', authController.registerUser);
router.post('/login/user', authController.loginUser);

// Admin routes
router.post('/register/admin', authController.registerAdmin);
router.post('/login/admin', authController.loginAdmin);

module.exports = router;
function newFunction() {
  return '../controllers/authController';
}

