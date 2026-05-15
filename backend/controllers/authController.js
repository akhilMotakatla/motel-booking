const db = require('../database/db');
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');

const JWT_SECRET = process.env.JWT_SECRET || 'your_jwt_secret_key';

// Helper to generate JWT
function generateToken(id, role) {
  return jwt.sign({ id, role }, JWT_SECRET, { expiresIn: '1d' });
}

// Register User
exports.registerUser = (req, res) => {
  const { fullName, email, password, address, city, state, zipCode, contact } = req.body;

  if (!email || !password || !fullName) {
    return res.status(400).json({ message: 'Full name, email and password are required.' });
  }

  // Hash password
  bcrypt.hash(password, 10, (err, hashedPassword) => {
    if (err) return res.status(500).json({ message: 'Error hashing password.' });

    // Insert into users table
    const query = `INSERT INTO users 
      (fullName, email, password, address, city, state, zipCode, contact) 
      VALUES (?, ?, ?, ?, ?, ?, ?, ?)`;

    db.run(query, [fullName, email, hashedPassword, address, city, state, zipCode, contact], function(err) {
      if (err) {
        if (err.message.includes('UNIQUE constraint failed')) {
          return res.status(400).json({ message: 'Email already registered.' });
        }
        return res.status(500).json({ message: 'Database error.' });
      }

      const token = generateToken(this.lastID, 'user');
      res.status(201).json({ token, userId: this.lastID });
    });
  });
};

// Login User
exports.loginUser = (req, res) => {
  const { email, password } = req.body;

  db.get('SELECT * FROM users WHERE email = ?', [email], (err, user) => {
    if (err) return res.status(500).json({ message: 'Database error.' });
    if (!user) return res.status(400).json({ message: 'Invalid email or password.' });

    bcrypt.compare(password, user.password, (err, isMatch) => {
      if (err) return res.status(500).json({ message: 'Error comparing passwords.' });
      if (!isMatch) return res.status(400).json({ message: 'Invalid email or password.' });

      const token = generateToken(user.id, 'user');
      res.json({ token, userId: user.id });
    });
  });
};

// Register Admin
exports.registerAdmin = (req, res) => {
  const { email, password } = req.body;

  if (!email || !password) {
    return res.status(400).json({ message: 'Email and password are required.' });
  }

  bcrypt.hash(password, 10, (err, hashedPassword) => {
    if (err) return res.status(500).json({ message: 'Error hashing password.' });

    const query = `INSERT INTO admins (email, password) VALUES (?, ?)`;
    db.run(query, [email, hashedPassword], function(err) {
      if (err) {
        if (err.message.includes('UNIQUE constraint failed')) {
          return res.status(400).json({ message: 'Email already registered.' });
        }
        return res.status(500).json({ message: 'Database error.' });
      }

      const token = generateToken(this.lastID, 'admin');
      res.status(201).json({ token, adminId: this.lastID });
    });
  });
};

// Login Admin
exports.loginAdmin = (req, res) => {
  const { email, password } = req.body;

  db.get('SELECT * FROM admins WHERE email = ?', [email], (err, admin) => {
    if (err) return res.status(500).json({ message: 'Database error.' });
    if (!admin) return res.status(400).json({ message: 'Invalid email or password.' });

    bcrypt.compare(password, admin.password, (err, isMatch) => {
      if (err) return res.status(500).json({ message: 'Error comparing passwords.' });
      if (!isMatch) return res.status(400).json({ message: 'Invalid email or password.' });

      const token = generateToken(admin.id, 'admin');
      res.json({ token, adminId: admin.id });
    });
  });
};
