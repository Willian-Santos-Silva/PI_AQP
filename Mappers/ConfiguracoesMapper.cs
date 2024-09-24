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
                dosagem = configuracoes.dosagem,
                rtc = configuracoes.rtc,
                tempo_reaplicacao = TimeSpan.FromSeconds(configuracoes.tempo_reaplicacao),
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
                rtc = configuracoes.rtc,
                dosagem = configuracoes.dosagem
            };
        }
    }
}
