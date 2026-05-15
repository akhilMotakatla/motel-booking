const { getDB } = require('./db');

async function init() {
  const db = await getDB();

  await db.exec(`
    CREATE TABLE IF NOT EXISTS Users (
      UserID INTEGER PRIMARY KEY AUTOINCREMENT,
      FullName TEXT NOT NULL,
      Email TEXT NOT NULL UNIQUE,
      PasswordHash TEXT NOT NULL,
      Address TEXT,
      City TEXT,
      State TEXT,
      ZipCode TEXT,
      Contact TEXT,
      CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
    );

    CREATE TABLE IF NOT EXISTS Admins (
      AdminID INTEGER PRIMARY KEY AUTOINCREMENT,
      FullName TEXT NOT NULL,
      Email TEXT NOT NULL UNIQUE,
      PasswordHash TEXT NOT NULL,
      CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
    );

    CREATE TABLE IF NOT EXISTS Rooms (
      RoomID INTEGER PRIMARY KEY AUTOINCREMENT,
      Name TEXT NOT NULL,
      Description TEXT,
      Price TEXT NOT NULL,
      Image TEXT,
      CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
    );

    CREATE TABLE IF NOT EXISTS Bookings (
      BookingID INTEGER PRIMARY KEY AUTOINCREMENT,
      UserID INTEGER NOT NULL,
      RoomID INTEGER NOT NULL,
      CheckInDate TEXT NOT NULL,
      CheckOutDate TEXT NOT NULL,
      CheckInTime TEXT,
      CheckOutTime TEXT,
      CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
      FOREIGN KEY (UserID) REFERENCES Users(UserID),
      FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID)
    );
  `);

  console.log('Tables created or already exist');
  process.exit(0);
}

init();
