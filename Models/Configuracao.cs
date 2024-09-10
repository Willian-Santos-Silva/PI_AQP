namespace PI_AQP.Models
{
    public class Configuracao
    {
        public double min_temperatura { get; set; } = 0;
        public double max_temperatura { get; set; } = 0;
        public int min_ph { get; set; } = 0;
        public int max_ph { get; set; } = 0;
        public int dosagem { get; set; } = 0;
        public TimeSpan tempo_reaplicacao { get; set; }
        public long rtc { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
    }
}
