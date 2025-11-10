
import os
import time
import ast
import cv2
import json
import requests
from ultralytics import YOLO
from dotenv import load_dotenv
from datetime import datetime

ROOT = os.path.dirname(os.path.dirname(__file__))
FRAMES_DIR = os.path.join(ROOT, "frames")
os.makedirs(FRAMES_DIR, exist_ok=True)

load_dotenv(os.path.join(ROOT, ".env"))

# BACKEND_URL agora aponta para a API .NET (ajuste a porta e o IP conforme o docker-compose)
BACKEND_URL = os.getenv("BACKEND_URL", "http://127.0.0.1:5193") # Porta padrão do .NET
VIDEO_PATH = os.getenv("VIDEO_PATH", "").strip()
SAVE_ANNOTATED = os.getenv("SAVE_ANNOTATED_FRAMES", "true").lower() == "true"

# Load model (downloads on first run if not cached)
model = YOLO("yolov8n.pt")

def open_capture():
    if VIDEO_PATH:
        return cv2.VideoCapture(VIDEO_PATH), f"file:{os.path.basename(VIDEO_PATH)}"
    return cv2.VideoCapture(0), "webcam:0"

cap, source_name = open_capture()
if not cap.isOpened():
    raise RuntimeError("Não foi possível abrir o vídeo ou webcam. Ajuste VIDEO_PATH no .env")

frame_id = 0

names = model.names  # dict id->name

def is_motorcycle(cls_id: int) -> bool:
    name = names.get(int(cls_id), "").lower()
    return "motorcycle" in name  # robusto a variações

while True:
    ok, frame = cap.read()
    if not ok:
        print("Fim do vídeo / sem frame da webcam.")
        break

    # Run inference
    results = model(frame, verbose=False)[0]

    boxes_xyxy = []
    moto_count = 0

    for b in results.boxes:
        cls_id = int(b.cls.item())
        if is_motorcycle(cls_id):
            x1, y1, x2, y2 = [float(v) for v in b.xyxy[0].tolist()]
            boxes_xyxy.append([x1, y1, x2, y2])
            moto_count += 1

    # Annotate frame (draw boxes)
    annotated = frame.copy()
    for (x1, y1, x2, y2) in boxes_xyxy:
        cv2.rectangle(annotated, (int(x1), int(y1)), (int(x2), int(y2)), (0, 255, 0), 2)
        cv2.putText(annotated, "moto", (int(x1), int(y1)-6), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 255, 0), 2)

    # Save annotated frame periodically (e.g., every 5th frame)
    if SAVE_ANNOTATED and (frame_id % 5 == 0):
        out_path = os.path.join(FRAMES_DIR, f"frame_{frame_id:06d}.jpg")
        cv2.imwrite(out_path, annotated)

    # Post detection to backend
    payload = {
        "ts": datetime.utcnow().isoformat(),
        "source": source_name,
        "frame_id": frame_id,
        "moto_count": moto_count,
        "boxes": boxes_xyxy
    }
    try:
        # Envia a detecção para o novo endpoint da API .NET
        r = requests.post(f"{BACKEND_URL}/api/v1/Vision/detection", json=payload, timeout=2.5)
        if r.status_code != 200:
            print("Falha POST:", r.status_code, r.text)
    except Exception as e:
        print("Erro ao enviar para backend:", e)

    # Show window for quick visual validation (opcional)
    cv2.imshow("Deteccao de Motos (YOLOv8)", annotated)
    if cv2.waitKey(1) & 0xFF == 27:  # ESC para sair
        break

    frame_id += 1

cap.release()
cv2.destroyAllWindows()
