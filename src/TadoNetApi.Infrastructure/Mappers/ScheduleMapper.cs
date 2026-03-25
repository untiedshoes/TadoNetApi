using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Dtos.Requests;

namespace TadoNetApi.Infrastructure.Mappers;

public static class ScheduleMapper
{
    public static Schedule ToDomain(TadoScheduleResponse dto, int zoneId)
    {
        return new Schedule
        {
            ZoneId = zoneId,
            Name = dto.Name,
            TargetTemperature = dto.TargetTemperature,
            IsActive = dto.IsActive ?? false,
            StartTime = DateTime.Parse(dto.StartTime),
            EndTime = DateTime.Parse(dto.EndTime)
        };
    }

    public static TadoScheduleRequest ToRequest(Schedule schedule)
    {
        return new TadoScheduleRequest
        {
            Name = schedule.Name,
            TargetTemperature = schedule.TargetTemperature,
            StartTime = schedule.StartTime.ToString("o"), // ISO 8601
            EndTime = schedule.EndTime.ToString("o")
        };
    }

    public static List<Schedule> ToDomainList(List<TadoScheduleResponse> dtos, int zoneId)
    {
        return dtos.Select(d => ToDomain(d, zoneId)).ToList();
    }
}