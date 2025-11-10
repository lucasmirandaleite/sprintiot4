# Moto Vision MVP (Detecção de Motos + Backend + Dashboard)

Este projeto foi configurado para rodar diretamente em Windows, com o vídeo de teste **já dentro da pasta do projeto**.

## 🎯 O que está contemplado
- **Detecção em tempo real** (ou de vídeo) usando YOLOv8 (Ultralytics).  
- **Envio dos dados** via HTTP para um **Backend FastAPI**, que persiste em **SQLite**.  
- **Dashboard Streamlit** em tempo (quase) real: contagem, histórico e último frame anotado.  
- **Organização** de repositório e **README completo** (este).  

---

## 📦 Requisitos
- Python 3.10+  
- Internet para baixar o modelo `yolov8n.pt` na primeira execução  

Crie e ative o ambiente virtual:
```powershell
python -m venv .venv
.venv\Scripts\activate
```

Instale as dependências:
```powershell
pip install --upgrade pip setuptools wheel
pip install -r requirements.txt --extra-index-url https://download.pytorch.org/whl/cpu
```

---

## 🔧 Configuração rápida
O arquivo `.env` já está configurado para usar o vídeo **que está dentro da pasta do projeto**:  

```
VIDEO_PATH=video IOT.mp4
```

Basta colocar o arquivo `video IOT.mp4` dentro da raiz do projeto.

Se `VIDEO_PATH` estiver vazio, o script usa **webcam (0)**.

---

## ▶️ Como rodar (Windows)

### 1) Rodar o backend (FastAPI + SQLite)
```powershell
uvicorn src.app:app --host 0.0.0.0 --port 8000 --reload
```
> Deixe essa janela aberta.

### 2) Rodar a detecção (YOLOv8)
```powershell
.venv\Scripts\activate
python src/detect.py
```

### 3) Rodar o dashboard (Streamlit)
```powershell
.venv\Scripts\activate
streamlit run src/dashboard.py
```
O navegador abrirá automaticamente em `http://localhost:8501`.

---

## 🧪 O que gravar no vídeo de apresentação
1. Terminal com **backend** iniciando e criando o banco `data/detections.db`.  
2. **Detecção** rodando sobre o vídeo dentro do projeto (`video IOT.mp4`) e salvando frames em `./frames`.  
3. **Dashboard** mostrando:  
   - **Contagem atual** de motos  
   - **Histórico** (gráfico ao longo do tempo)  
   - **Último frame anotado** com boxes  

---

## 🗂️ Estrutura do projeto
```
moto-vision-mvp/
├─ src/
│  ├─ app.py
│  ├─ detect.py
│  └─ dashboard.py
├─ data/
│  └─ detections.db
├─ frames/
├─ video IOT.mp4          # vídeo colocado dentro da pasta do projeto
├─ .env                   # já configurado
├─ requirements.txt
└─ README.md
```

---

## 🔍 Testando a API manualmente
- Última detecção: [http://127.0.0.1:8000/detections/latest](http://127.0.0.1:8000/detections/latest)  
- Série histórica: [http://127.0.0.1:8000/detections/series](http://127.0.0.1:8000/detections/series)  
- Documentação Swagger: [http://127.0.0.1:8000/docs](http://127.0.0.1:8000/docs)  
