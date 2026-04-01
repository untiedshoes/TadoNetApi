using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers
{
    /// <summary>
    /// Provides mapping extensions between Tado API HomeState DTOs and domain entities.
    /// </summary>
    public static class HomeStateMapper
    {
        /// <summary>
        /// Maps a <see cref="TadoHomeStateResponse"/> to a <see cref="HomeState"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="HomeState"/>.</returns>
        public static HomeState ToDomain(this TadoHomeStateResponse dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new HomeState
            {
                Presence = dto.Presence
            };
        }

        /// <summary>
        /// Maps a collection of <see cref="TadoHomeStateResponse"/> to a list of <see cref="HomeState"/> domain entities.
        /// </summary>
        /// <param name="dtos">The list of API DTOs.</param>
        /// <returns>A list of mapped <see cref="HomeState"/>.</returns>
        public static List<HomeState> ToDomainList(this IEnumerable<TadoHomeStateResponse> dtos)
        {
            return dtos.Select(d => d.ToDomain()).ToList();
        }
    }
}
