# 🏍️ MottuRFID — Mapeamento Inteligente do Pátio

## 📖 Sobre o Projeto
O **MottuRFID** é um sistema inteligente de **monitoramento e rastreamento de motos em pátios logísticos**, combinando **visão computacional (YOLO)** com **leitura RFID** para oferecer uma visualização em tempo real do status e localização dos veículos.  

O projeto foi desenvolvido como parte do **Challenge 2025 — FIAP**, integrando múltiplas tecnologias modernas em um ecossistema completo com **.NET 8, FastAPI, PostgreSQL, Docker e Streamlit**.

---

## 🧩 Arquitetura da Solução

```
+-----------------------------------------------------------+
|                         Dashboard                         |
|                   (Streamlit / Plotly)                    |
+-----------------------------------------------------------+
               ▲                                ▲
               │                                │
               │ REST API                       │ REST API
               │                                │
+---------------------------+     +---------------------------+
|        .NET 8 API         |     |    Python Vision API      |
|   (MottuRFID.API - C#)    |     | (FastAPI + YOLOv8)        |
| - Gerencia status das     |     | - Detecta motos via vídeo |
|   motos                   |     | - Envia detecções via API |
| - Persiste dados no DB    |     | - Processa frames YOLO    |
+---------------------------+     +---------------------------+
               │                                │
               ▼                                ▼
                +-------------------------------+
                |         PostgreSQL DB          |
                +--------------------------------+
```

---

## 🚀 Tecnologias Utilizadas

### Backend
- **.NET 8 (C#)** — API principal de gerenciamento e integração  
- **Entity Framework Core** — ORM para persistência de dados  
- **PostgreSQL** — Banco de dados relacional  
- **FastAPI (Python)** — Módulo de visão computacional (YOLOv8)  

### Visão Computacional
- **Ultralytics YOLOv8** — Detecção de motos em vídeo  
- **OpenCV** — Captura e processamento de frames  
- **Torch / Torchvision / Torchaudio (CPU)** — Inferência em tempo real  

### Frontend / Dashboard
- **Streamlit + Plotly** — Visualização do status e métricas das motos  

### Infraestrutura
- **Docker / Docker Compose** — Orquestração dos serviços  
- **WSL2 (Windows)** — Ambiente de execução Linux isolado  

---

## ⚙️ Estrutura do Projeto

```
mottu_project/
├── projects/
│   ├── dotnet-api/         # API principal (.NET 8)
│   ├── vision-python/      # Módulo de visão computacional (FastAPI + YOLO)
│   ├── dashboard/          # Dashboard em Streamlit
│   └── docker-compose.yml  # Orquestra todos os serviços
├── README.md
└── docs/                   # Diagramas e documentação
```

---

## 🐳 Como Executar o Projeto com Docker

> 💡 Antes de começar, garanta que o **Docker Desktop** está aberto e rodando no Windows.

### 1️⃣ — Clonar o repositório
```bash
git clone https://github.com/seuusuario/mottu_project.git
cd mottu_project/projects
```

### 2️⃣ — Limpar build antigo (opcional)
```powershell
docker system prune -af
wsl --shutdown
```

### 3️⃣ — Construir e subir os containers
```powershell
$env:DOCKER_BUILDKIT=0; docker-compose up --build
```

### 4️⃣ — Acessar os serviços
| Serviço | URL | Descrição |
|----------|-----|------------|
| .NET API | [http://localhost:8080/swagger](http://localhost:8080/swagger) | Endpoints REST |
| FastAPI (Vision) | [http://localhost:8000/docs](http://localhost:8000/docs) | Recebe detecções do YOLO |
| Streamlit Dashboard | [http://localhost:8501](http://localhost:8501) | Painel de visualização |

### 5️⃣ — Parar o projeto
```powershell
docker-compose down
```

---

## 🧠 Principais Endpoints

### `POST /api/v1/vision/detection`
Recebe dados de detecção do módulo de visão computacional.  
```json
{
  "motoCount": 5,
  "source": "camera_patio_1",
  "frameId": "frame_00123",
  "timestamp": "2025-11-09T22:30:00Z"
}
```

### `GET /api/v1/vision/latest`
Retorna o status atual de todas as motos para o Dashboard.

---

## 🧾 Integrantes da Equipe
| Nome | Função | RM |
|------|---------|----|
| Lucas Leite | Backend / Docker / Infra | 12345 |
| [Integrante 2] | Visão Computacional | 12346 |
| [Integrante 3] | Dashboard / UI | 12347 |

---

## 🧩 Documentação Técnica
- 📄 **Arquitetura:** Domain Driven Design (DDD)  
- 🧱 **Padrões:** Repository, Factory, Service Layer  
- 🧰 **Banco de Dados:** PostgreSQL (mapeado via EF Core)  
- 🧠 **Visão Computacional:** YOLOv8 (Ultralytics)  
- ☁️ **Deploy:** Containers Docker (com suporte a escalabilidade local)

---

## 🏁 Conclusão
O projeto **MottuRFID** demonstra a integração entre **IA, IoT e desenvolvimento backend** para criar uma solução prática e inteligente de rastreamento de veículos, explorando o potencial do **ecossistema .NET + Python + Docker**.
