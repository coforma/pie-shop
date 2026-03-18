"""
Mock Delivery Service
Simulates drone delivery with variable timing based on address distance.
"""
import random
import uuid
from datetime import datetime, timedelta
from fastapi import FastAPI
import uvicorn

app = FastAPI(title="Delivery Mock Service")

deliveries = {}

DRONE_IDS = [f"drone-{i}" for i in range(1, 16)]


@app.post("/api/v1/deliveries")
async def schedule_delivery(request: dict):
    if random.random() < 0.08:
        from fastapi import HTTPException
        raise HTTPException(status_code=400, detail="Address not in delivery zone")

    delivery_id = f"del_{uuid.uuid4().hex[:8]}"
    drone_id = random.choice(DRONE_IDS)

    eta_minutes = random.randint(15, 45)
    eta = datetime.utcnow() + timedelta(minutes=eta_minutes)

    deliveries[delivery_id] = {
        "deliveryId": delivery_id,
        "status": "SCHEDULED",
        "droneId": drone_id,
        "destination": request.get("destination", {}),
        "package": request.get("package", {}),
        "eta": eta.isoformat() + "Z",
        "createdAt": datetime.utcnow().isoformat() + "Z",
        "location": None,
    }

    return {
        "deliveryId": delivery_id,
        "droneId": drone_id,
        "eta": eta.isoformat() + "Z",
    }


@app.get("/api/v1/deliveries/{delivery_id}")
async def get_delivery(delivery_id: str):
    if delivery_id not in deliveries:
        from fastapi import HTTPException
        raise HTTPException(status_code=404, detail=f"Delivery {delivery_id} not found")

    delivery = deliveries[delivery_id]

    created = datetime.fromisoformat(delivery["createdAt"].replace("Z", ""))
    elapsed = (datetime.utcnow() - created).total_seconds()

    if elapsed > 5 and delivery["status"] == "SCHEDULED":
        delivery["status"] = "IN_TRANSIT"
        delivery["location"] = {"lat": 39.7817, "lng": -89.6501}

    if elapsed > 15 and delivery["status"] == "IN_TRANSIT":
        delivery["status"] = "DELIVERED"

    return delivery


@app.get("/health")
async def health():
    return {"status": "ok", "service": "delivery-mock"}


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8083)
