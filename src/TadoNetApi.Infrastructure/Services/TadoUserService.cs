using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;

namespace TadoNetApi.Infrastructure.Services;

/// <summary>
/// Concrete implementation of IUserService using the Tado API.
/// </summary>
public class TadoUserService : IUserService
{
    private readonly ITadoHttpClient _httpClient;

    public TadoUserService(ITadoHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<User?> GetMeAsync(CancellationToken cancellationToken = default)
    {
        var dto = await _httpClient.GetAsync<TadoUserResponse>("me", cancellationToken);
        return dto == null ? null : UserMapper.ToDomain(dto);
    }
}