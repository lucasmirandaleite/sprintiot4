
import os
import sqlite3
from typing import Optional, List
from fastapi import FastAPI, Body
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, Field
from datetime import datetime

DB_PATH = os.path.join(os.path.dirname(os.path.dirname(__file__)), "data", "detections.db")
os.makedirs(os.path.dirname(DB_PATH), exist_ok=True)

app = FastAPI(title="Moto Vision Backend", version="0.1.0")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

def init_db():
    with sqlite3.connect(DB_PATH) as conn:
        cur = conn.cursor()
        cur.execute("""
            CREATE TABLE IF NOT EXISTS detections (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                ts TEXT NOT NULL,
                source TEXT,
                frame_id INTEGER,
                moto_count INTEGER,
                boxes_json TEXT
            )
        """)
        conn.commit()

init_db()

class DetectionIn(BaseModel):
    ts: Optional[str] = Field(default_factory=lambda: datetime.utcnow().isoformat())
    source: Optional[str] = "unknown"
    frame_id: Optional[int] = 0
    moto_count: int = 0
    boxes: List[List[float]] = Field(default_factory=list)  # [x1, y1, x2, y2]

@app.post("/detections")
def post_detection(payload: DetectionIn):
    with sqlite3.connect(DB_PATH) as conn:
        cur = conn.cursor()
        cur.execute(
            "INSERT INTO detections (ts, source, frame_id, moto_count, boxes_json) VALUES (?, ?, ?, ?, ?)",
            (payload.ts, payload.source, payload.frame_id, payload.moto_count, str(payload.boxes))
        )
        conn.commit()
    return {"status": "ok"}

@app.get("/detections/latest")
def latest():
    with sqlite3.connect(DB_PATH) as conn:
        cur = conn.cursor()
        cur.execute("SELECT ts, source, frame_id, moto_count, boxes_json FROM detections ORDER BY id DESC LIMIT 1")
        row = cur.fetchone()
    if not row:
        return {}
    ts, source, frame_id, moto_count, boxes_json = row
    return {"ts": ts, "source": source, "frame_id": frame_id, "moto_count": moto_count, "boxes": boxes_json}

@app.get("/detections/series")
def series(limit: int = 500):
    with sqlite3.connect(DB_PATH) as conn:
        cur = conn.cursor()
        cur.execute("SELECT ts, moto_count FROM detections ORDER BY id DESC LIMIT ?", (limit,))
        rows = cur.fetchall()
    rows = rows[::-1]
    return [{"ts": r[0], "moto_count": r[1]} for r in rows]
