const express = require('express');
const cors = require('cors');
const dbConfig = require('./config/db');
const app = express();

app.use(cors());
app.use(express.json());
app.use('/uploads', express.static('uploads'));

// Add routes later

const PORT = 5000;
app.listen(PORT, () => console.log(`Server running on port ${PORT}`));
