using Aquaponia.Domain.Entities;

namespace PI_AQP.Mapper
{
    public static class SystemInformationMapper
    {
        public static SystemInformationModel ToModel(this SystemInformationDTO systemInformation)
        {
            return new SystemInformationModel()
            {
                remaining_time_irrigation = systemInformation.remaining_time_irrigation,
                temperatura = systemInformation.temperatura,
                turbidade = systemInformation.turbidade,
                ph = systemInformation.ph,
                status_heater = systemInformation.status_heater,
                status_water_pump = systemInformation.status_water_pump,
            };
        }
        public static SystemInformationDTO ToDTO(this SystemInformationModel systemInformation)
        {
            return new SystemInformationDTO()
            {
                remaining_time_irrigation = systemInformation.remaining_time_irrigation,
                temperatura = systemInformation.temperatura,
                turbidade = systemInformation.turbidade,
                ph = systemInformation.ph,
                status_heater = systemInformation.status_heater,
                status_water_pump = systemInformation.status_water_pump,
            };
        }
    }
}
