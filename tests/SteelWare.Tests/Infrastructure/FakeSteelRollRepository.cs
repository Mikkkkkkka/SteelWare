using SteelWare.Application.Abstractions;
using SteelWare.Application.Models;
using SteelWare.Domain;

namespace SteelWare.Tests.Infrastructure;

internal sealed class FakeSteelRollRepository(IEnumerable<SteelRoll> rolls) : ISteelRollRepository
{
    private readonly List<SteelRoll> _rolls = rolls.ToList();

    public SteelRollFilter? LastFilter { get; private set; }

    public Task<SteelRoll> Insert(float length, float weight)
    {
        throw new NotSupportedException();
    }

    public Task<SteelRoll> SoftDelete(int rollId)
    {
        throw new NotSupportedException();
    }

    public async IAsyncEnumerable<SteelRoll> GetFiltered(SteelRollFilter filter)
    {
        LastFilter = filter;

        foreach (var roll in _rolls)
        {
            if (filter.IdsFrom is not null && roll.Id < filter.IdsFrom.Value)
                continue;
            if (filter.IdsTo is not null && roll.Id > filter.IdsTo.Value)
                continue;
            if (filter.WeightsFrom is not null && roll.Weight < filter.WeightsFrom.Value)
                continue;
            if (filter.WeightsTo is not null && roll.Weight > filter.WeightsTo.Value)
                continue;
            if (filter.AddedFrom is not null && roll.AddedAt < filter.AddedFrom.Value)
                continue;
            if (filter.AddedTo is not null && roll.AddedAt > filter.AddedTo.Value)
                continue;
            if (filter.DeletedFrom is not null && (roll.DeletedAt is null || roll.DeletedAt < filter.DeletedFrom.Value))
                continue;
            if (filter.DeletedTo is not null && (roll.DeletedAt is null || roll.DeletedAt > filter.DeletedTo.Value))
                continue;

            yield return roll;
            await Task.Yield();
        }
    }
}