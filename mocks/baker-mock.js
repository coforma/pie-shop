const express = require('express');
const { v4: uuidv4 } = require('uuid');

const app = express();
app.use(express.json());

const jobs = new Map();

app.post('/api/v1/bake', (req, res) => {
  const { pieType, temperature, duration } = req.body;
  
  const jobId = `bake_${uuidv4().substring(0, 8)}`;
  const ovenId = `oven-${Math.floor(Math.random() * 5) + 1}`;
  
  const shouldFail = Math.random() < 0.05;
  
  if (shouldFail) {
    return res.status(503).json({
      error: 'ALL_OVENS_BUSY',
      message: 'All ovens are currently in use'
    });
  }
  
  const estimatedCompletion = new Date();
  estimatedCompletion.setMinutes(estimatedCompletion.getMinutes() + Math.floor(duration / 5));
  
  jobs.set(jobId, {
    jobId,
    pieType,
    temperature,
    duration,
    ovenId,
    status: 'IN_PROGRESS',
    progress: 0,
    estimatedCompletion: estimatedCompletion.toISOString()
  });
  
  let progress = 0;
  const progressInterval = setInterval(() => {
    const job = jobs.get(jobId);
    if (job && job.status === 'IN_PROGRESS') {
      progress += 20;
      job.progress = Math.min(progress, 100);
      
      if (progress >= 100) {
        job.status = 'COMPLETED';
        clearInterval(progressInterval);
      }
    } else {
      clearInterval(progressInterval);
    }
  }, 2000);
  
  res.status(201).json({
    jobId,
    ovenId,
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

const PORT = process.env.PORT || 8082;
app.listen(PORT, () => {
  console.log(`Baker mock service running on port ${PORT}`);
});
