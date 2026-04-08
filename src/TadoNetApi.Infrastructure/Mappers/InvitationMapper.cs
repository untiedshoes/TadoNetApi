using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

/// <summary>
/// Provides mapping between Tado invitation DTOs and domain invitation entities.
/// </summary>
public static class InvitationMapper
{
    /// <summary>
    /// Maps a <see cref="TadoInvitationResponse"/> to an <see cref="Invitation"/> domain entity.
    /// </summary>
    public static Invitation ToDomain(this TadoInvitationResponse dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new Invitation
        {
            Token = dto.Token,
            Email = dto.Email,
            FirstSent = dto.FirstSent,
            LastSent = dto.LastSent,
            Inviter = dto.Inviter == null
                ? null
                : new InvitationInviter
                {
                    Name = dto.Inviter.Name,
                    Email = dto.Inviter.Email,
                    Username = dto.Inviter.Username,
                    Enabled = dto.Inviter.Enabled,
                    Id = dto.Inviter.Id,
                    HomeId = dto.Inviter.HomeId,
                    Locale = dto.Inviter.Locale,
                    Type = dto.Inviter.Type,
                    Home = dto.Inviter.Home == null ? null : HomeMapper.ToDomain(dto.Inviter.Home)
                }
        };
    }

    /// <summary>
    /// Maps a list of <see cref="TadoInvitationResponse"/> to a list of <see cref="Invitation"/> domain entities.
    /// </summary>
    public static List<Invitation> ToDomainList(this IEnumerable<TadoInvitationResponse> dtos)
    {
        return dtos.Select(d => d.ToDomain()).ToList();
    }
}