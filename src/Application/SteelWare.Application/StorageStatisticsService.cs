using SteelWare.Application.Abstractions;
using SteelWare.Application.Contracts;
using SteelWare.Application.Models;
using SteelWare.Domain;

namespace SteelWare.Application;

public class StorageStatisticsService(ISteelRollRepository repository) : IStorageStatisticsService
{
    public async Task<int> CountAdded()
    {
        var count = 0;

        await foreach (var roll in repository.GetFiltered(default))
            if (roll.DeletedAt is null)
                count++;

        return count;
    }

    public async Task<int> CountDeleted()
    {
        var count = 0;

        await foreach (var roll in repository.GetFiltered(default))
            if (roll.DeletedAt is not null)
                count++;

        return count;
    }

    public async Task<float> AverageLength(TimePeriod period)
    {
        var stats = await AggregateRollsInPeriod(
            period,
            (Count: 0, Total: 0F),
            roll => roll.DeletedAt is null,
            (current, roll) => (current.Count + 1, current.Total + roll.Length));

        return stats.Total / stats.Count;
    }

    public async Task<float> AverageWeight(TimePeriod period)
    {
        var stats = await AggregateRollsInPeriod(
            period,
            (Count: 0, Total: 0F),
            roll => roll.DeletedAt is null,
            (current, roll) => (current.Count + 1, current.Total + roll.Weight));

        return stats.Total / stats.Count;
    }

    public async Task<float> MaxLength(TimePeriod period)
    {
        var maxLength = await AggregateRollsInPeriod(
            period,
            float.MinValue,
            roll => roll.DeletedAt is not null && roll.DeletedAt > period.To,
            (current, roll) => Math.Max(current, roll.Length));

        return maxLength;
    }

    public async Task<float> MinLength(TimePeriod period)
    {
        var minLength = await AggregateRollsInPeriod(
            period,
            float.MaxValue,
            roll => roll.DeletedAt is not null && roll.DeletedAt > period.To,
            (current, roll) => Math.Min(current, roll.Length));

        return minLength;
    }

    public async Task<float> MaxWeight(TimePeriod period)
    {
        var maxWeight = await AggregateRollsInPeriod(
            period,
            float.MinValue,
            roll => roll.DeletedAt is not null && roll.DeletedAt > period.To,
            (current, roll) => Math.Max(current, roll.Weight));

        return maxWeight;
    }

    public async Task<float> MinWeight(TimePeriod period)
    {
        var minWeight = await AggregateRollsInPeriod(
            period,
            float.MaxValue,
            roll => roll.DeletedAt is not null && roll.DeletedAt > period.To,
            (current, roll) => Math.Min(current, roll.Weight));

        return minWeight;
    }

    public Task<float> TotalWeight(TimePeriod period)
    {
        return AggregateRollsInPeriod(
            period,
            0F,
            roll => roll.DeletedAt is null,
            (current, roll) => current + roll.Weight);
    }

    private async Task<TResult> AggregateRollsInPeriod<TResult>(
        TimePeriod period,
        TResult seed,
        Func<SteelRoll, bool> shouldInclude,
        Func<TResult, SteelRoll, TResult> aggregate)
    {
        var result = seed;

        SteelRollFilter filter = new()
        {
            AddedFrom = period.From,
            AddedTo = period.To
        };

        await foreach (var roll in repository.GetFiltered(filter))
        {
            if (!shouldInclude(roll)) continue;
            result = aggregate(result, roll);
        }

        return result;
    }
}