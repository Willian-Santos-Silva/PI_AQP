using Aquaponia.DTO.Entities;
using PI_AQP.Models;
using System.Diagnostics;
using System.Globalization;

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
                dataRTC = DateTimeOffset.FromUnixTimeSeconds(configuracoes.rtc).UtcDateTime.Date,
                timeRTC = DateTimeOffset.FromUnixTimeSeconds(configuracoes.rtc).UtcDateTime.TimeOfDay,
                tempo_reaplicacao = TimeSpan.FromSeconds(configuracoes.tempo_reaplicacao),
            };
        }
        public static ConfiguracoesDTO ToDTO(this Configuracao configuracoes)
        {
            var utcTime = DateTime.SpecifyKind(configuracoes.dataRTC.Add(configuracoes.timeRTC), DateTimeKind.Utc);

            return new ConfiguracoesDTO()
            {
                min_temperature = configuracoes.min_temperatura,
                max_temperature = configuracoes.max_temperatura,
                min_ph = configuracoes.min_ph,
                max_ph = configuracoes.max_ph,
                dosagem_solucao_acida = configuracoes.dosagem_solucao_acida,
                dosagem_solucao_base = configuracoes.dosagem_solucao_base,
                tempo_reaplicacao = (long)configuracoes.tempo_reaplicacao.TotalSeconds,
                rtc = ((DateTimeOffset)utcTime).ToUnixTimeSeconds()
            };
        }
    }
}
