const express = require('express');
const morgan = require('morgan');
const sql = require('mssql');
const cors = require('cors');
const app = express();
const port = 3000;

//Routers
const dataUsageRoutes = require('./dataUsageRoutes');
const tallyCounterRoutes = require('./tallyCounterRoutes');

app.use(cors());
app.use(express.json()); // Use the JSON middleware
app.use(morgan(':custom'));

morgan.token('custom', (req, res) => {
  return `Request: ${req.method} ${req.url} | Status: ${res.statusCode} | IP: ${req.ip}`;
});

// Mount the data usage routes under /dataUsage
app.use('/dataUsage', dataUsageRoutes);
app.use('/pjs', tallyCounterRoutes);



// Start the server
app.listen(port, () => {
    console.log(`Server running on http://localhost:${port}`);
});