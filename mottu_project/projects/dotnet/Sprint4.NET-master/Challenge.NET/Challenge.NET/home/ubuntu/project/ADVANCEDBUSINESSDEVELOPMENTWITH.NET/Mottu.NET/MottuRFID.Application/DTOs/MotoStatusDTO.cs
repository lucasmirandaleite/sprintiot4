namespace MottuRFID.Application.DTOs
{
    public class MotoStatusDTO
    {
        public int MotoId { get; set; }
        public string Placa { get; set; }
        public string Status { get; set; } // Ex: "Parada", "Em Uso", "Detectada"
        public string Localizacao { get; set; } // Ex: "Pátio Principal", "Ponto de Leitura 1"
        public int X { get; set; } // Coordenada X (para visualização no dashboard)
        public int Y { get; set; } // Coordenada Y (para visualização no dashboard)
        public string LastUpdated { get; set; }
    }
}
