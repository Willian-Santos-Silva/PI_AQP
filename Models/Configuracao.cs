namespace PI_AQP.Models
{
    public class Configuracao
    {
        public int min_temperatura { get; set; } = 0;
        public int max_temperatura { get; set; } = 0;
        public int min_ph { get; set; } = 0;
        public int max_ph { get; set; } = 0;
        public int dosagem_solucao_acida { get; set; } = 0;
        public int dosagem_solucao_base { get; set; } = 0;
        public TimeSpan tempo_reaplicacao { get; set; }
        public long rtc { get; set; } = 0;
        public DateTime dataRTC { get; set; }
        public TimeSpan timeRTC { get; set; }
    }
}
