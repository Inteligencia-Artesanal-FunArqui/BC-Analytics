using OsitoPolar.Analytics.Service.Domain.Model.Aggregates;
using OsitoPolar.Analytics.Service.Domain.Model.Commands;
using OsitoPolar.Analytics.Service.Domain.Model.Entities;
using OsitoPolar.Analytics.Service.Domain.Repositories;
using OsitoPolar.Analytics.Service.Domain.Services;
using OsitoPolar.Analytics.Service.Shared.Domain.Repositories;

namespace OsitoPolar.Analytics.Service.Application.Internal.CommandServices;

/// <summary>
/// Implementation of analytics command service
/// </summary>
public class AnalyticsCommandService(
    IAnalyticsRepository analyticsRepository,
    IUnitOfWork unitOfWork) : IAnalyticsCommandService
{
    public async Task<TemperatureReading?> Handle(RecordTemperatureReadingCommand command)
    {
        if (command.Temperature < -50 || command.Temperature > 100)
            throw new ArgumentException("Temperature reading is out of valid range (-50°C to 100°C)");

        var reading = new TemperatureReading(command.EquipmentId, command.Temperature, 
            command.Timestamp ?? DateTimeOffset.UtcNow);
        
        await analyticsRepository.AddAsync(reading);
        await unitOfWork.CompleteAsync();
        
        return reading;
    }

    public async Task<EnergyReading?> Handle(RecordEnergyReadingCommand command)
    {
        if (command.Consumption < 0)
            throw new ArgumentException("Energy consumption cannot be negative");

        var reading = new EnergyReading(command.EquipmentId, command.Consumption, command.Unit);
        
        await analyticsRepository.AddEnergyReadingAsync(reading);
        await unitOfWork.CompleteAsync();
        
        return reading;
    }
}