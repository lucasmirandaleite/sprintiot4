import streamlit as st
import requests
import pandas as pd
import time
import os
import altair as alt

# --- Configurações ---
# O Dashboard agora consome a API .NET (dotnet-api)
# No ambiente Docker, o nome do serviço é 'dotnet-api'
# Fora do Docker, use o IP e porta corretos (ex: http://localhost:5193)
# A porta do container .NET no docker-compose é 8080
DOTNET_API_URL = os.getenv("DOTNET_API_URL", "http://localhost:5193")
# O endpoint foi ajustado para o novo endpoint GET /api/v1/Vision/latest
API_ENDPOINT = f"{DOTNET_API_URL}/api/v1/Vision/latest"

# --- Layout do Streamlit ---
st.set_page_config(
    page_title="Mottu - Dashboard de Monitoramento Integrado",
    layout="wide",
    initial_sidebar_state="expanded"
)

st.title("🏍️ Monitoramento de Pátio - Visão Integrada")

# --- Funções de Visualização ---

def get_moto_statuses():
    """Busca o status das motos na API .NET."""
    try:
        # No Docker, o DOTNET_API_URL será 'http://dotnet-api:8080'
        response = requests.get(API_ENDPOINT)
        response.raise_for_status()
        data = response.json()
        
        # O DTO do .NET usa PascalCase, o pandas precisa de snake_case ou ajuste
        # Vamos normalizar as chaves para minúsculas para facilitar o acesso
        normalized_data = []
        for item in data:
            normalized_item = {k.lower(): v for k, v in item.items()}
            normalized_data.append(normalized_item)
            
        return pd.DataFrame(normalized_data)
    except requests.exceptions.RequestException as e:
        # st.error(f"Erro ao conectar com a API .NET: {e}") # Comentar para não poluir o dashboard
        return pd.DataFrame()

def draw_patio_visualization(df):
    """Desenha a visualização do pátio com base nas coordenadas X e Y."""
    if df.empty:
        st.warning("Nenhum dado de status de moto disponível.")
        return

    # Mapeamento de cores para o status
    color_map = {
        "Detectada por Visão": "red",
        "Em Uso": "orange",
        "Parada": "green",
        "Manutenção": "blue"
    }

    # Cria o canvas
    canvas_width = 1000
    canvas_height = 500
    
    # Cria o mapa de pontos para o gráfico
    chart_data = []
    for index, row in df.iterrows():
        chart_data.append({
            "x": row['x'],
            "y": row['y'],
            "status": row['status'],
            "placa": row['placa'],
            "localizacao": row['localizacao'],
            "lastupdated": row['lastupdated'],
            "color": color_map.get(row['status'], "gray")
        })

    chart_df = pd.DataFrame(chart_data)

    st.subheader("Visualização do Pátio (Simulação)")
    
    # Usa o Altair para desenhar o gráfico de dispersão
    chart = alt.Chart(chart_df).mark_circle(size=200).encode(
        x=alt.X('x', scale=alt.Scale(domain=[0, canvas_width])),
        y=alt.Y('y', scale=alt.Scale(domain=[0, canvas_height])),
        color=alt.Color('status', scale=alt.Scale(domain=list(color_map.keys()), range=list(color_map.values()))),
        tooltip=['placa', 'status', 'localizacao', 'lastupdated']
    ).properties(
        width=canvas_width,
        height=canvas_height,
        title="Localização das Motos"
    ).interactive() # Permite zoom e pan

    st.altair_chart(chart, use_container_width=True)

# --- Loop Principal do Dashboard ---

refresh_sec = st.sidebar.slider("Refresh (s)", min_value=1, max_value=10, value=2, step=1)

# Container para o status em tempo real
status_container = st.empty()

while True:
    df_statuses = get_moto_statuses()

    with status_container.container():
        st.subheader("Status Geral")
        
        if not df_statuses.empty:
            # Exibe os dados em formato de tabela (grid)
            st.dataframe(df_statuses, use_container_width=True)
            
            # Exibe a visualização do pátio
            draw_patio_visualization(df_statuses)
            
            # Indicadores em tempo real
            col1, col2, col3, col4 = st.columns(4)
            
            total_motos = len(df_statuses)
            motos_detectadas = df_statuses[df_statuses['status'] == 'Detectada por Visão'].shape[0]
            motos_paradas = df_statuses[df_statuses['status'] == 'Parada'].shape[0]
            
            col1.metric("Total de Motos", total_motos)
            col2.metric("Detectadas por Visão", motos_detectadas, delta=f"{motos_detectadas} alertas")
            col3.metric("Paradas", motos_paradas)
            col4.metric("Última Atualização", df_statuses['lastupdated'].max() if not df_statuses.empty else "N/A")
            
        else:
            st.info("Aguardando dados da API .NET...")

    time.sleep(refresh_sec)
