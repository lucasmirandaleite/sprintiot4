# Imagem base leve do Python 3.10
FROM python:3.10-slim

# Impede prompts interativos no build
ENV DEBIAN_FRONTEND=noninteractive
ENV PYTHONUNBUFFERED=1

# Define o diretório de trabalho
WORKDIR /app

# Copia apenas requirements primeiro (melhora cache de build)
COPY requirements.txt ./

# Instala dependências do sistema necessárias para compilar pacotes Python
RUN apt-get update && apt-get install -y --no-install-recommends \
        gcc \
        libffi-dev \
        libssl-dev \
        libjpeg-dev \
        zlib1g-dev \
    && rm -rf /var/lib/apt/lists/*

# Atualiza pip e instala dependências Python com menos uso de RAM/disk
RUN pip install --no-cache-dir --upgrade pip setuptools wheel \
 && pip install --no-cache-dir -r requirements.txt

# Agora copia o restante do projeto
COPY . .

# Garante que o container falhe se o script quebrar
CMD ["python", "-u", "src/detect.py"]
