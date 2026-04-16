using SteelWare.Application.Models;
using SteelWare.Domain;

namespace SteelWare.Application.Abstractions;

public interface ISteelRollRepository
{
    public Task<SteelRoll> Insert(float length, float weight);

    public Task<SteelRoll> SoftDelete(int rollId);

    public IAsyncEnumerable<SteelRoll> GetFiltered(SteelRollFilter filter);
}