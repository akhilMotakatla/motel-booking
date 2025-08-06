const sql = require('mssql/msnodesqlv8');

const config = {
  server: '(localdb)\\MSSQLLocalDB',
  database: 'MotelBooking',
  options: {
    trustedConnection: true,
    trustServerCertificate: true,
    enableArithAbort: true,
    instanceName: 'MSSQLLocalDB',
  }
};

async function testConnection() {
  try {
    const pool = await sql.connect(config);
    console.log('Connected to MSSQL');
    pool.close();
  } catch (err) {
    console.error('DB connection error:');
    console.dir(err, { depth: null });
  }
}

testConnection();
