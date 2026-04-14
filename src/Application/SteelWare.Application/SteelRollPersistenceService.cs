using SteelWare.Application.Abstractions;
using SteelWare.Application.Contracts;
using SteelWare.Application.Models;
using SteelWare.Domain;

namespace SteelWare.Application;

public class SteelRollPersistenceService(ISteelRollRepository repository) : ISteelRollPersistenceService
{
    public Task<SteelRoll> Add(AddSteelRollRequest request)
    {
        return repository.Insert(request.Length, request.Weight);
    }

    public Task<SteelRoll> Delete(DeleteSteelRollRequest request)
    {
        return repository.SoftDelete(request.Id);
    }

    public IAsyncEnumerable<SteelRoll> GetFiltered(SteelRollFilter filter)
    {
        return repository.GetFiltered(filter);
    }
}