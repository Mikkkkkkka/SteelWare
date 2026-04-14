using SteelWare.Application.Models;
using SteelWare.Domain;

namespace SteelWare.Application.Contracts;

public interface ISteelRollPersistenceService
{
    public Task<SteelRoll> Add(AddSteelRollRequest request);

    public Task<SteelRoll> SoftDelete(DeleteSteelRollRequest request);

    public IAsyncEnumerable<SteelRoll> GetFiltered(SteelRollFilter filter);
}