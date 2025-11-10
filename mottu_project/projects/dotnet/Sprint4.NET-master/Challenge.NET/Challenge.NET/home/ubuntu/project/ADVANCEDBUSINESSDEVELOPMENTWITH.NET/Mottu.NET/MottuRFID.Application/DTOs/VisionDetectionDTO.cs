namespace MottuRFID.Application.DTOs
{
    public class VisionDetectionDTO
    {
        public string Timestamp { get; set; }
        public string Source { get; set; }
        public int FrameId { get; set; }
        public int MotoCount { get; set; }
        // Simplificando o envio das boxes para o .NET, apenas a contagem é crítica para o MVP
        // public List<List<float>> Boxes { get; set; } 
    }
}
