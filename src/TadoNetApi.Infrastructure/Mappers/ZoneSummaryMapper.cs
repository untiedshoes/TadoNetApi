using System;
using System.Collections.Generic;
using System.Linq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers
{
    /// <summary>
    /// Provides mapping from TadoZoneSummaryResponse DTO to ZoneSummary domain entity.
    /// </summary>
    public static class ZoneSummaryMapper
    {
        public static ZoneSummary ToDomain(this TadoZoneSummaryResponse dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new ZoneSummary
            {
                Setting = dto.Setting?.ToDomain(),
                Termination = dto.Termination?.ToDomain()
            };
        }

        public static List<ZoneSummary> ToDomainList(this IEnumerable<TadoZoneSummaryResponse> dtos)
            => dtos.Select(ToDomain).ToList();
    }
}