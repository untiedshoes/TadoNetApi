using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers
{
    /// <summary>
    /// Maps bridge-scoped boiler DTOs into domain entities.
    /// </summary>
    public static class BoilerByBridgeMapper
    {
        public static BoilerInfo ToDomain(this TadoBoilerInfoResponse dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new BoilerInfo
            {
                BoilerPresent = dto.BoilerPresent,
                BoilerId = dto.BoilerId
            };
        }

        public static BoilerMaxOutputTemperature ToDomain(this TadoBoilerMaxOutputTemperatureResponse dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new BoilerMaxOutputTemperature
            {
                BoilerMaxOutputTemperatureInCelsius = dto.BoilerMaxOutputTemperatureInCelsius
            };
        }

        public static BoilerWiringInstallationState ToDomain(this TadoBoilerWiringInstallationStateResponse dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new BoilerWiringInstallationState
            {
                State = dto.State,
                DeviceWiredToBoiler = dto.DeviceWiredToBoiler == null ? null : new BoilerWiredDevice
                {
                    Type = dto.DeviceWiredToBoiler.Type,
                    SerialNo = dto.DeviceWiredToBoiler.SerialNo,
                    ThermInterfaceType = dto.DeviceWiredToBoiler.ThermInterfaceType,
                    Connected = dto.DeviceWiredToBoiler.Connected,
                    LastRequestTimestamp = dto.DeviceWiredToBoiler.LastRequestTimestamp
                },
                BridgeConnected = dto.BridgeConnected,
                HotWaterZonePresent = dto.HotWaterZonePresent,
                Boiler = dto.Boiler == null ? null : new BoilerWiringBoiler
                {
                    OutputTemperature = dto.Boiler.OutputTemperature == null ? null : new BoilerOutputTemperature
                    {
                        Celsius = dto.Boiler.OutputTemperature.Celsius,
                        Timestamp = dto.Boiler.OutputTemperature.Timestamp
                    }
                }
            };
        }
    }
}