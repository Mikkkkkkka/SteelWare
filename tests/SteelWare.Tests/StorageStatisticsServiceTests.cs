using SteelWare.Application;
using SteelWare.Application.Models;
using SteelWare.Domain;
using SteelWare.Tests.Infrastructure;
using Xunit;

namespace SteelWare.Tests;

public sealed class StorageStatisticsServiceTests
{
    [Fact]
    public async Task CountAdded_CountsOnlyRollsWithoutDeletionDate()
    {
        var service = CreateService(
            CreateRoll(1, 10, 100, new DateTime(2026, 4, 1), null),
            CreateRoll(2, 20, 200, new DateTime(2026, 4, 2), new DateTime(2026, 4, 4)));

        var result = await service.CountAdded();

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task CountDeleted_CountsOnlySoftDeletedRolls()
    {
        var service = CreateService(
            CreateRoll(1, 10, 100, new DateTime(2026, 4, 1), null),
            CreateRoll(2, 20, 200, new DateTime(2026, 4, 2), new DateTime(2026, 4, 4)));

        var result = await service.CountDeleted();

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task PeriodStatistics_UseOnlyRollsAvailableAtPeriodEnd()
    {
        var period = new TimePeriod(new DateTime(2026, 4, 1), new DateTime(2026, 4, 30));
        var service = CreateService(
            CreateRoll(1, 10, 100, new DateTime(2026, 4, 5), null),
            CreateRoll(2, 20, 50, new DateTime(2026, 4, 10), new DateTime(2026, 5, 2)),
            CreateRoll(3, 30, 10, new DateTime(2026, 4, 12), new DateTime(2026, 4, 20)),
            CreateRoll(4, 40, 999, new DateTime(2026, 5, 1), null));

        var averageLength = await service.AverageLength(period);
        var averageWeight = await service.AverageWeight(period);
        var maxLength = await service.MaxLength(period);
        var minLength = await service.MinLength(period);
        var maxWeight = await service.MaxWeight(period);
        var minWeight = await service.MinWeight(period);
        var totalWeight = await service.TotalWeight(period);

        Assert.Equal(15, averageLength);
        Assert.Equal(75, averageWeight);
        Assert.Equal(20, maxLength);
        Assert.Equal(10, minLength);
        Assert.Equal(100, maxWeight);
        Assert.Equal(50, minWeight);
        Assert.Equal(150, totalWeight);
    }

    [Fact]
    public async Task PeriodStatistics_RequestRepositoryDataForSelectedPeriod()
    {
        var period = new TimePeriod(new DateTime(2026, 4, 1), new DateTime(2026, 4, 30));
        var repository = new FakeSteelRollRepository([CreateRoll(1, 10, 100, new DateTime(2026, 4, 5), null)]);
        var service = new StorageStatisticsService(repository);

        await service.TotalWeight(period);

        Assert.Equal(period.From, repository.LastFilter?.AddedFrom);
        Assert.Equal(period.To, repository.LastFilter?.AddedTo);
    }

    private static StorageStatisticsService CreateService(params SteelRoll[] rolls)
    {
        return new StorageStatisticsService(new FakeSteelRollRepository(rolls));
    }

    private static SteelRoll CreateRoll(int id, float length, float weight, DateTime addedAt, DateTime? deletedAt)
    {
        return new SteelRoll
        {
            Id = id,
            Length = length,
            Weight = weight,
            AddedAt = addedAt,
            DeletedAt = deletedAt
        };
    }
}