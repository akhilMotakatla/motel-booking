const sql = require('mssql/msnodesqlv8'); 

const config = {
  server: '(localdb)\\MSSQLLocalDB',
  database: 'MotelBooking',
  options: {
    trustedConnection: true,
    trustServerCertificate: true
  },
  driver: 'msnodesqlv8'
};

const poolPromise = new sql.ConnectionPool(config)
  .connect()
  .then(pool => {
    console.log('Connected to MSSQL');
    return pool;
  })
  .catch(err => {
    console.error('Database connection failed', err);
  });

module.exports = {
  sql,
  poolPromise
};
