using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SteelWare.Application.Contracts;
using SteelWare.Application.Models;
using SteelWare.Presentation.Http.Contracts;

namespace SteelWare.Presentation.Http.Controllers;

[ApiController]
[Route("api/steel-rolls")]
public sealed class SteelRollsController(ISteelRollPersistenceService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<SteelRollResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<SteelRollResponse>>> GetFiltered(
        [FromQuery] GetSteelRollsQuery query,
        CancellationToken cancellationToken)
    {
        var items = new List<SteelRollResponse>();

        await foreach (var roll in service.GetFiltered(query.ToFilter()).WithCancellation(cancellationToken))
            items.Add(SteelRollResponse.FromDomain(roll));

        return Ok(items);
    }

    [HttpPost]
    [ProducesResponseType<SteelRollResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SteelRollResponse>> Add(
        [FromBody] AddSteelRollHttpRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var roll = await service.Add(new AddSteelRollRequest(request.Length, request.Weight));
        var response = SteelRollResponse.FromDomain(roll);

        return Created($"/api/steel-rolls/{response.Id}", response);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType<SteelRollResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SteelRollResponse>> Delete(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var deleted = await service.Delete(new DeleteSteelRollRequest(id));
            return Ok(SteelRollResponse.FromDomain(deleted));
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Steel roll was not found.",
                Detail = exception.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }
}