using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SteelWare.Application.Contracts;
using SteelWare.Application.Models;
using SteelWare.Domain;
using SteelWare.Presentation.Http.Contracts;
using SteelWare.Presentation.Http.Controllers;
using Xunit;

namespace SteelWare.Tests;

public sealed class SteelRollsControllerTests
{
    [Fact]
    public async Task Add_ReturnsCreatedResponseWithMappedPayload()
    {
        var service = new FakeSteelRollPersistenceService
        {
            AddedRoll = new SteelRoll
            {
                Id = 42,
                Length = 15,
                Weight = 200,
                AddedAt = new DateTime(2026, 4, 17, 12, 0, 0, DateTimeKind.Utc)
            }
        };

        var controller = new SteelRollsController(service);

        var result = await controller.Add(new AddSteelRollHttpRequest { Length = 15, Weight = 200 },
            CancellationToken.None);

        var created = Assert.IsType<CreatedResult>(result.Result);
        var payload = Assert.IsType<SteelRollResponse>(created.Value);

        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        Assert.Equal("/api/steel-rolls/42", created.Location);
        Assert.Equal(42, payload.Id);
        Assert.Equal(15, service.LastAddRequest?.Length);
        Assert.Equal(200, service.LastAddRequest?.Weight);
    }

    [Fact]
    public async Task Delete_WhenRollDoesNotExist_ReturnsNotFoundProblemDetails()
    {
        var controller = new SteelRollsController(new FakeSteelRollPersistenceService
        {
            DeleteException = new KeyNotFoundException("missing")
        });

        var result = await controller.Delete(15, CancellationToken.None);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var details = Assert.IsType<ProblemDetails>(notFound.Value);

        Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
        Assert.Equal("Steel roll was not found.", details.Title);
        Assert.Equal("missing", details.Detail);
    }

    [Fact]
    public async Task GetFiltered_PassesLengthRangeToService()
    {
        var service = new FakeSteelRollPersistenceService();
        var controller = new SteelRollsController(service);

        await controller.GetFiltered(new GetSteelRollsQuery
        {
            LengthsFrom = 10,
            LengthsTo = 20
        }, CancellationToken.None);

        Assert.Equal(10, service.LastFilter?.LengthsFrom);
        Assert.Equal(20, service.LastFilter?.LengthsTo);
    }

    private sealed class FakeSteelRollPersistenceService : ISteelRollPersistenceService
    {
        public AddSteelRollRequest? LastAddRequest { get; private set; }

        public SteelRollFilter? LastFilter { get; private set; }

        public SteelRoll? AddedRoll { get; init; }

        public Exception? DeleteException { get; init; }

        public Task<SteelRoll> Add(AddSteelRollRequest request)
        {
            LastAddRequest = request;
            return Task.FromResult(AddedRoll!);
        }

        public Task<SteelRoll> Delete(DeleteSteelRollRequest request)
        {
            if (DeleteException is not null)
                throw DeleteException;

            throw new NotSupportedException();
        }

        public IAsyncEnumerable<SteelRoll> GetFiltered(SteelRollFilter filter)
        {
            LastFilter = filter;
            return Empty();
        }

        private static async IAsyncEnumerable<SteelRoll> Empty()
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}