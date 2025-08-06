const db = require('../database/db');
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');

const JWT_SECRET = 'your_jwt_secret_key'; // replace with .env in real apps

// Register User
exports.registerUser = (req, res) => {
  const { fullName, email, password, address, city, state, zipCode, contact } = req.body;

  const hashedPassword = bcrypt.hashSync(password, 10);

  const sql = `INSERT INTO Users (FullName, Email, PasswordHash, Address, City, State, ZipCode, Contact)
               VALUES (?, ?, ?, ?, ?, ?, ?, ?)`;

  db.run(sql, [fullName, email, hashedPassword, address, city, state, zipCode, contact], function (err) {
    if (err) {
      return res.status(500).json({ error: 'User registration failed', details: err.message });
    }
    res.status(201).json({ message: 'User registered successfully', userId: this.lastID });
  });
};

// Login User
exports.loginUser = (req, res) => {
  const { email, password } = req.body;

  const sql = `SELECT * FROM Users WHERE Email = ?`;

  db.get(sql, [email], (err, row) => {
    if (err) return res.status(500).json({ error: err.message });
    if (!row) return res.status(404).json({ message: 'User not found' });

    const isPasswordValid = bcrypt.compareSync(password, row.PasswordHash);
    if (!isPasswordValid) return res.status(401).json({ message: 'Invalid credentials' });

    const token = jwt.sign({ userId: row.UserID, email: row.Email }, JWT_SECRET, { expiresIn: '1h' });
    res.json({ message: 'Login successful', token });
  });
};

// Register Admin
exports.registerAdmin = (req, res) => {
  const { fullName, email, password } = req.body;

  const hashedPassword = bcrypt.hashSync(password, 10);

  const sql = `INSERT INTO Admins (FullName, Email, PasswordHash)
               VALUES (?, ?, ?)`;

  db.run(sql, [fullName, email, hashedPassword], function (err) {
    if (err) {
      return res.status(500).json({ error: 'Admin registration failed', details: err.message });
    }
    res.status(201).json({ message: 'Admin registered successfully', adminId: this.lastID });
  });
};

// Login Admin
exports.loginAdmin = (req, res) => {
  const { email, password } = req.body;

  const sql = `SELECT * FROM Admins WHERE Email = ?`;

  db.get(sql, [email], (err, row) => {
    if (err) return res.status(500).json({ error: err.message });
    if (!row) return res.status(404).json({ message: 'Admin not found' });

    const isPasswordValid = bcrypt.compareSync(password, row.PasswordHash);
    if (!isPasswordValid) return res.status(401).json({ message: 'Invalid credentials' });

    const token = jwt.sign({ adminId: row.AdminID, email: row.Email }, JWT_SECRET, { expiresIn: '1h' });
    res.json({ message: 'Login successful', token });
  });
};
