const express = require('express');
const { v4: uuidv4 } = require('uuid');

const app = express();
app.use(express.json());

const jobs = new Map();

app.post('/api/v1/pick-fruit', (req, res) => {
  const { fruitType, quantity, quality } = req.body;
  
  const jobId = `pick_${uuidv4().substring(0, 8)}`;
  
  const shouldFail = Math.random() < 0.1;
  
  if (shouldFail) {
    return res.status(503).json({
      error: 'SERVICE_UNAVAILABLE',
      message: 'Fruit picker robots are currently offline'
    });
  }
  
  const estimatedCompletion = new Date();
  estimatedCompletion.setSeconds(estimatedCompletion.getSeconds() + 45);
  
  jobs.set(jobId, {
    jobId,
    fruitType,
    quantity,
    quality,
    status: 'IN_PROGRESS',
    estimatedCompletion: estimatedCompletion.toISOString()
  });
  
  setTimeout(() => {
    const job = jobs.get(jobId);
    if (job) {
      job.status = 'COMPLETED';
      job.fruits = Array(quantity).fill({ type: fruitType, quality });
    }
  }, 5000);
  
  res.status(201).json({
    jobId,
    estimatedCompletion: estimatedCompletion.toISOString()
  });
});

app.get('/api/v1/jobs/:jobId', (req, res) => {
  const job = jobs.get(req.params.jobId);
  
  if (!job) {
    return res.status(404).json({
      error: 'JOB_NOT_FOUND',
      message: 'Job not found'
    });
  }
  
  res.json(job);
});

const PORT = process.env.PORT || 8081;
app.listen(PORT, () => {
  console.log(`Fruit picker mock service running on port ${PORT}`);
});
