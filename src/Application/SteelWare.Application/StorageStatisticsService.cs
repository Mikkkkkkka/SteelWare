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
            roll => WasAvailableAtPeriodEnd(roll, period),
            (current, roll) => (current.Count + 1, current.Total + roll.Length));

        return stats.Total / stats.Count;
    }

    public async Task<float> AverageWeight(TimePeriod period)
    {
        var stats = await AggregateRollsInPeriod(
            period,
            (Count: 0, Total: 0F),
            roll => WasAvailableAtPeriodEnd(roll, period),
            (current, roll) => (current.Count + 1, current.Total + roll.Weight));

        return stats.Total / stats.Count;
    }

    public async Task<float> MaxLength(TimePeriod period)
    {
        var maxLength = await AggregateRollsInPeriod(
            period,
            float.MinValue,
            roll => WasAvailableAtPeriodEnd(roll, period),
            (current, roll) => Math.Max(current, roll.Length));

        return maxLength;
    }

    public async Task<float> MinLength(TimePeriod period)
    {
        var minLength = await AggregateRollsInPeriod(
            period,
            float.MaxValue,
            roll => WasAvailableAtPeriodEnd(roll, period),
            (current, roll) => Math.Min(current, roll.Length));

        return minLength;
    }

    public async Task<float> MaxWeight(TimePeriod period)
    {
        var maxWeight = await AggregateRollsInPeriod(
            period,
            float.MinValue,
            roll => WasAvailableAtPeriodEnd(roll, period),
            (current, roll) => Math.Max(current, roll.Weight));

        return maxWeight;
    }

    public async Task<float> MinWeight(TimePeriod period)
    {
        var minWeight = await AggregateRollsInPeriod(
            period,
            float.MaxValue,
            roll => WasAvailableAtPeriodEnd(roll, period),
            (current, roll) => Math.Min(current, roll.Weight));

        return minWeight;
    }

    public Task<float> TotalWeight(TimePeriod period)
    {
        return AggregateRollsInPeriod(
            period,
            0F,
            roll => WasAvailableAtPeriodEnd(roll, period),
            (current, roll) => current + roll.Weight);
    }

    public Task<TimeSpan> MinStorageDuration(TimePeriod period)
    {
        return AggregateDeletedRollsInPeriod(
            period,
            TimeSpan.MaxValue,
            (current, roll) => Min(current, GetStorageDuration(roll)));
    }

    public Task<TimeSpan> MaxStorageDuration(TimePeriod period)
    {
        return AggregateDeletedRollsInPeriod(
            period,
            TimeSpan.Zero,
            (current, roll) => Max(current, GetStorageDuration(roll)));
    }

    public async Task<DateTime> GetDayWithMinRollCount(TimePeriod period)
    {
        var dailyStats = await BuildDailyStats(period);
        return dailyStats.MinBy(static entry => entry.RollCount)!.Day;
    }

    public async Task<DateTime> GetDayWithMaxRollCount(TimePeriod period)
    {
        var dailyStats = await BuildDailyStats(period);
        return dailyStats.MaxBy(static entry => entry.RollCount)!.Day;
    }

    public async Task<DateTime> GetDayWithMinTotalWeight(TimePeriod period)
    {
        var dailyStats = await BuildDailyStats(period);
        return dailyStats.MinBy(static entry => entry.TotalWeight)!.Day;
    }

    public async Task<DateTime> GetDayWithMaxTotalWeight(TimePeriod period)
    {
        var dailyStats = await BuildDailyStats(period);
        return dailyStats.MaxBy(static entry => entry.TotalWeight)!.Day;
    }

    private static bool WasAvailableAtPeriodEnd(SteelRoll roll, TimePeriod period)
    {
        return roll.DeletedAt is null || roll.DeletedAt > period.To;
    }

    private static TimeSpan GetStorageDuration(SteelRoll roll)
    {
        return roll.DeletedAt!.Value - roll.AddedAt;
    }

    private static TimeSpan Min(TimeSpan left, TimeSpan right)
    {
        return left <= right ? left : right;
    }

    private static TimeSpan Max(TimeSpan left, TimeSpan right)
    {
        return left >= right ? left : right;
    }

    private async Task<IReadOnlyList<DailyStat>> BuildDailyStats(TimePeriod period)
    {
        var firstDay = period.From.Date;
        var lastDay = period.To.Date;

        if (firstDay > lastDay)
            throw new ArgumentException("The period must contain at least one day.", nameof(period));

        var statsByDay = new List<DailyStat>();

        for (var day = firstDay; day <= lastDay; day = day.AddDays(1))
            statsByDay.Add(new DailyStat(day));

        SteelRollFilter filter = new()
        {
            AddedTo = lastDay.AddDays(1).AddTicks(-1)
        };

        await foreach (var roll in repository.GetFiltered(filter))
            for (var index = 0; index < statsByDay.Count; index++)
            {
                var day = statsByDay[index].Day;
                if (!WasAvailableAtDayEnd(roll, day))
                    continue;

                statsByDay[index] = statsByDay[index] with
                {
                    RollCount = statsByDay[index].RollCount + 1,
                    TotalWeight = statsByDay[index].TotalWeight + roll.Weight
                };
            }

        return statsByDay;
    }

    private static bool WasAvailableAtDayEnd(SteelRoll roll, DateTime day)
    {
        var dayEndExclusive = day.Date.AddDays(1);
        return roll.AddedAt < dayEndExclusive && (roll.DeletedAt is null || roll.DeletedAt >= dayEndExclusive);
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

    private async Task<TResult> AggregateDeletedRollsInPeriod<TResult>(
        TimePeriod period,
        TResult seed,
        Func<TResult, SteelRoll, TResult> aggregate)
    {
        var result = seed;

        SteelRollFilter filter = new()
        {
            DeletedFrom = period.From,
            DeletedTo = period.To
        };

        await foreach (var roll in repository.GetFiltered(filter))
            result = aggregate(result, roll);

        return result;
    }

    private sealed record DailyStat(DateTime Day)
    {
        public int RollCount { get; init; }

        public float TotalWeight { get; init; }
    }
}