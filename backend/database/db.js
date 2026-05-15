// db.js
const sqlite3 = require('sqlite3');
const { open } = require('sqlite');
const path = require('path');

async function getDB() {
  const db = await open({
    filename: path.join(__dirname, 'motel.db'),
    driver: sqlite3.Database
  });
  return db;
}

module.exports = { getDB };
