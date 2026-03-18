"""
Mock Fruit Picker Service
Simulates a robot fruit picker with realistic delays and occasional failures.
"""
import random
import uuid
import time
from datetime import datetime, timedelta
from fastapi import FastAPI
import uvicorn

app = FastAPI(title="Fruit Picker Mock Service")

jobs = {}


@app.post("/api/v1/pick-fruit")
async def pick_fruit(request: dict):
    if random.random() < 0.1:
        from fastapi import HTTPException
        raise HTTPException(status_code=503, detail="Picker robot unavailable")

    job_id = f"pick_{uuid.uuid4().hex[:8]}"
    delay_seconds = random.randint(30, 60)
    completion_time = datetime.utcnow() + timedelta(seconds=delay_seconds)

    jobs[job_id] = {
        "jobId": job_id,
        "status": "IN_PROGRESS",
        "fruitType": request.get("fruitType"),
        "quantity": request.get("quantity"),
        "startedAt": datetime.utcnow().isoformat() + "Z",
        "estimatedCompletion": completion_time.isoformat() + "Z",
    }

    return {
        "jobId": job_id,
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
        if elapsed > 5:
            job["status"] = "COMPLETED"
            job["fruits"] = [
                {"type": job["fruitType"], "quantity": job["quantity"], "quality": "premium"}
            ]

    return job


@app.get("/health")
async def health():
    return {"status": "ok", "service": "fruit-picker-mock"}


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8081)
