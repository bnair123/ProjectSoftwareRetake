const express = require('express');
const fs = require('fs');
const router = express.Router();

// Function to get the file path
function getFilePath() {
    return __dirname + '/tallyCounters.json'; // File to store counters
}

// Function to read the JSON file
function readJsonFile() {
    try {
        const filePath = getFilePath();
        if (!fs.existsSync(filePath)) {
            return { counters: [] }; // Return an empty structure if the file doesn't exist
        }
        const jsonData = fs.readFileSync(filePath, 'utf8');
        return JSON.parse(jsonData);
    } catch (error) {
        console.error('Error reading the JSON file:', error.message);
        return null;
    }
}

// Function to save data to the JSON file
function saveJsonFile(data) {
    fs.writeFileSync(getFilePath(), JSON.stringify(data, null, 2), 'utf8');
}

// GET endpoint to retrieve a specific counter
router.get('/:id', (req, res) => {
    const id = parseInt(req.params.id); // Convert ID to integer
    const data = readJsonFile();
    if (!data || !Array.isArray(data.counters) || id < 0 || id >= data.counters.length) {
        return res.status(404).send('Counter not found.');
    }

    const counter = data.counters[id];
    res.json(counter);
});

//Post request to handle updating counters
router.post('/:id', (req, res) => {
    const id = parseInt(req.params.id); // Convert ID to integer
    const { value, limit } = req.body;
    const data = readJsonFile();

    if (!data || !Array.isArray(data.counters)) {
        return res.status(500).send('Error retrieving counter data.');
    }

    // Validate ID is within range
    if (id < 0 || id >= 3) {
        return res.status(400).send('Counter ID out of bounds.');
    }

    // Update or initialize the counter at the given ID
    if (data.counters[id]) {
        data.counters[id] = { value, limit };
    } else {
        return res.status(400).send('Counter ID out of bounds.');
    }

    saveJsonFile(data);

    res.json(data.counters[id]);
});


module.exports = router;
