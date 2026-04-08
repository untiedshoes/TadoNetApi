using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers
{
    /// <summary>
    /// Maps bridge DTOs into domain entities.
    /// </summary>
    public static class BridgeMapper
    {
        public static Bridge ToDomain(this TadoBridgeResponse dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Bridge
            {
                Partner = dto.Partner,
                HomeId = dto.HomeId
            };
        }
    }
}