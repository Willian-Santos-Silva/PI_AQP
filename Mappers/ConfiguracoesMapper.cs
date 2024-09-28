using Aquaponia.DTO.Entities;
using PI_AQP.Models;

namespace PI_AQP.Mapper
{
    public static class ConfiguracoesMapper
    {
        public static Configuracao ToModel(this ConfiguracoesDTO configuracoes)
        {
            return new Configuracao()
            {
                min_temperatura = configuracoes.min_temperature,
                max_temperatura = configuracoes.max_temperature,
                min_ph = configuracoes.min_ph,
                max_ph = configuracoes.max_ph,
                dosagem_solucao_acida = configuracoes.dosagem_solucao_acida,
                dosagem_solucao_base = configuracoes.dosagem_solucao_base,
                rtc = configuracoes.rtc,
                dataRTC = DateTimeOffset.FromUnixTimeSeconds(configuracoes.rtc).Date.ToLocalTime(),
                timeRTC = DateTimeOffset.FromUnixTimeSeconds(configuracoes.rtc).DateTime.ToLocalTime().TimeOfDay,
                tempo_reaplicacao = TimeSpan.FromSeconds(configuracoes.tempo_reaplicacao),
                //tempo_reaplicacao = DateTimeOffset.FromUnixTimeSeconds(configuracoes.tempo_reaplicacao).DateTime.ToLocalTime().TimeOfDay,
            };
        }
        public static ConfiguracoesDTO ToDTO(this Configuracao configuracoes)
        {
            return new ConfiguracoesDTO()
            {
                min_temperature = configuracoes.min_temperatura,
                max_temperature = configuracoes.max_temperatura,
                min_ph = configuracoes.min_ph,
                max_ph = configuracoes.max_ph,
                dosagem_solucao_acida = configuracoes.dosagem_solucao_acida,
                dosagem_solucao_base = configuracoes.dosagem_solucao_base,
                tempo_reaplicacao = (long)configuracoes.tempo_reaplicacao.TotalSeconds,
                rtc = configuracoes.rtc
            };
        }
    }
}
