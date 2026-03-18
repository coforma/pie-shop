"""
Mock Baker Service
Simulates a robot baker with realistic oven timing and occasional failures.
"""
import random
import uuid
from datetime import datetime, timedelta
from fastapi import FastAPI
import uvicorn

app = FastAPI(title="Baker Mock Service")

jobs = {}

OVEN_IDS = ["oven-1", "oven-2", "oven-3", "oven-4"]


@app.post("/api/v1/bake")
async def start_baking(request: dict):
    if random.random() < 0.05:
        from fastapi import HTTPException
        raise HTTPException(status_code=503, detail="All ovens are currently in use")

    job_id = f"bake_{uuid.uuid4().hex[:8]}"
    duration = request.get("duration", 45)
    oven_id = random.choice(OVEN_IDS)

    completion_time = datetime.utcnow() + timedelta(minutes=duration)

    jobs[job_id] = {
        "jobId": job_id,
        "status": "IN_PROGRESS",
        "pieType": request.get("pieType"),
        "temperature": request.get("temperature"),
        "duration": duration,
        "ovenId": oven_id,
        "progress": 0,
        "startedAt": datetime.utcnow().isoformat() + "Z",
        "estimatedCompletion": completion_time.isoformat() + "Z",
    }

    return {
        "jobId": job_id,
        "ovenId": oven_id,
        "estimatedCompletion": completion_time.isoformat() + "Z",
    }


@app.get("/api/v1/jobs/{job_id}")
async def get_job(job_id: str):
    if job_id not in jobs:
        from fastapi import HTTPException
        raise HTTPException(status_code=404, detail=f"Job {job_id} not found")

    job = jobs[job_id]

    if job["status"] == "IN_PROGRESS":
        start = datetime.fromisoformat(job["startedAt"].replace("Z", ""))
        elapsed = (datetime.utcnow() - start).total_seconds()
        total = job["duration"] * 60
        progress = min(100, int((elapsed / total) * 100))
        job["progress"] = progress

        if elapsed > 10:
            job["status"] = "COMPLETED"
            job["progress"] = 100

    return job


@app.get("/health")
async def health():
    return {"status": "ok", "service": "baker-mock"}


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8082)
