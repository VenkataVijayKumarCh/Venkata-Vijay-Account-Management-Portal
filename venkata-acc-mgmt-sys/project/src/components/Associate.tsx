import express from 'express';
import fs from 'fs';
import cors from 'cors';

const app = express();
const PORT = 5173;

app.use(cors());
app.use(express.json());

app.get('/associates', (req, res) => {
  const data = fs.readFileSync('./src/data/mockData.json', 'utf-8');
  res.json(JSON.parse(data));
});

app.post('/associates', (req, res) => {
  const newAssociate = req.body;
  const data = JSON.parse(fs.readFileSync('./src/data/mockData.json', 'utf-8'));
  data.push(newAssociate);
  fs.writeFileSync('./src/data/mockData.json', JSON.stringify(data, null, 2));
  res.status(201).json(newAssociate);
});

app.listen(PORT, () => {
  console.log(`Server running on http://localhost:${PORT}`);
});