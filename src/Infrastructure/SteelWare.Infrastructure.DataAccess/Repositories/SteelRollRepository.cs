using Microsoft.EntityFrameworkCore;
using SteelWare.Application.Abstractions;
using SteelWare.Application.Models;
using SteelWare.Domain;

namespace SteelWare.Infrastructure.DataAccess.Repositories;

public class SteelRollRepository(SteelWareDbContext dbContext) : ISteelRollRepository
{
    public async Task<SteelRoll> Insert(float length, float weight)
    {
        SteelRoll roll = new()
        {
            Length = length,
            Weight = weight,
            AddedAt = DateTime.UtcNow
        };

        dbContext.SteelRolls.Add(roll);
        await dbContext.SaveChangesAsync();
        return roll;
    }

    public async Task<SteelRoll> SoftDelete(int rollId)
    {
        var roll = await dbContext.SteelRolls.FirstOrDefaultAsync(x => x.Id == rollId)
                   ?? throw new KeyNotFoundException($"Steel roll with id={rollId} was not found.");

        roll.DeletedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return roll;
    }

    public IAsyncEnumerable<SteelRoll> GetFiltered(SteelRollFilter filter)
    {
        IQueryable<SteelRoll> query = dbContext.SteelRolls.AsNoTracking();

        if (filter.IdsFrom is not null)
            query = query.Where(x => x.Id >= filter.IdsFrom.Value);
        if (filter.IdsTo is not null)
            query = query.Where(x => x.Id <= filter.IdsTo.Value);

        if (filter.WeightsFrom is not null)
            query = query.Where(x => x.Weight >= filter.WeightsFrom.Value);
        if (filter.WeightsTo is not null)
            query = query.Where(x => x.Weight <= filter.WeightsTo.Value);

        if (filter.AddedFrom is not null)
            query = query.Where(x => x.AddedAt >= filter.AddedFrom.Value);
        if (filter.AddedTo is not null)
            query = query.Where(x => x.AddedAt <= filter.AddedTo.Value);

        if (filter.DeletedFrom is not null)
            query = query.Where(x => x.DeletedAt != null && x.DeletedAt >= filter.DeletedFrom.Value);
        if (filter.DeletedTo is not null)
            query = query.Where(x => x.DeletedAt != null && x.DeletedAt <= filter.DeletedTo.Value);

        return query.AsAsyncEnumerable();
    }
}
