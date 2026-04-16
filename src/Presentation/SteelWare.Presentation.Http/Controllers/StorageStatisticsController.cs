using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SteelWare.Application.Contracts;
using SteelWare.Application.Models;
using SteelWare.Presentation.Http.Contracts;

namespace SteelWare.Presentation.Http.Controllers;

[ApiController]
[Route("api/storage-statistics")]
public sealed class StorageStatisticsController(IStorageStatisticsService service) : ControllerBase
{
    [HttpGet("count-added")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CountAdded(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Ok(await service.CountAdded());
    }

    [HttpGet("count-deleted")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> CountDeleted(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Ok(await service.CountDeleted());
    }

    [HttpGet("average-length")]
    [ProducesResponseType<float>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<float>> AverageLength([FromQuery] TimePeriodQuery query,
        CancellationToken cancellationToken)
    {
        return Execute(query, service.AverageLength, cancellationToken);
    }

    [HttpGet("average-weight")]
    [ProducesResponseType<float>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<float>> AverageWeight([FromQuery] TimePeriodQuery query,
        CancellationToken cancellationToken)
    {
        return Execute(query, service.AverageWeight, cancellationToken);
    }

    [HttpGet("max-length")]
    [ProducesResponseType<float>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<float>> MaxLength([FromQuery] TimePeriodQuery query, CancellationToken cancellationToken)
    {
        return Execute(query, service.MaxLength, cancellationToken);
    }

    [HttpGet("min-length")]
    [ProducesResponseType<float>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<float>> MinLength([FromQuery] TimePeriodQuery query, CancellationToken cancellationToken)
    {
        return Execute(query, service.MinLength, cancellationToken);
    }

    [HttpGet("max-weight")]
    [ProducesResponseType<float>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<float>> MaxWeight([FromQuery] TimePeriodQuery query, CancellationToken cancellationToken)
    {
        return Execute(query, service.MaxWeight, cancellationToken);
    }

    [HttpGet("min-weight")]
    [ProducesResponseType<float>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<float>> MinWeight([FromQuery] TimePeriodQuery query, CancellationToken cancellationToken)
    {
        return Execute(query, service.MinWeight, cancellationToken);
    }

    [HttpGet("total-weight")]
    [ProducesResponseType<float>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<float>> TotalWeight([FromQuery] TimePeriodQuery query, CancellationToken cancellationToken)
    {
        return Execute(query, service.TotalWeight, cancellationToken);
    }

    [HttpGet("min-roll-count-day")]
    [ProducesResponseType<DateTime>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<DateTime>> DayWithMinRollCount(
        [FromQuery] TimePeriodQuery query,
        CancellationToken cancellationToken)
    {
        return Execute(query, service.GetDayWithMinRollCount, cancellationToken);
    }

    [HttpGet("max-roll-count-day")]
    [ProducesResponseType<DateTime>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<DateTime>> DayWithMaxRollCount(
        [FromQuery] TimePeriodQuery query,
        CancellationToken cancellationToken)
    {
        return Execute(query, service.GetDayWithMaxRollCount, cancellationToken);
    }

    [HttpGet("min-total-weight-day")]
    [ProducesResponseType<DateTime>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<DateTime>> DayWithMinTotalWeight(
        [FromQuery] TimePeriodQuery query,
        CancellationToken cancellationToken)
    {
        return Execute(query, service.GetDayWithMinTotalWeight, cancellationToken);
    }

    [HttpGet("max-total-weight-day")]
    [ProducesResponseType<DateTime>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<ActionResult<DateTime>> DayWithMaxTotalWeight(
        [FromQuery] TimePeriodQuery query,
        CancellationToken cancellationToken)
    {
        return Execute(query, service.GetDayWithMaxTotalWeight, cancellationToken);
    }

    private async Task<ActionResult<float>> Execute(
        TimePeriodQuery query,
        Func<TimePeriod, Task<float>> action,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Ok(await action(query.ToTimePeriod()));
    }

    private async Task<ActionResult<DateTime>> Execute(
        TimePeriodQuery query,
        Func<TimePeriod, Task<DateTime>> action,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Ok(await action(query.ToTimePeriod()));
    }
}
