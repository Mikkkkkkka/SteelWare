using System.ComponentModel.DataAnnotations;
using SteelWare.Application.Models;

namespace SteelWare.Presentation.Http.Contracts;

public sealed record GetSteelRollsQuery : IValidatableObject
{
    public int? IdsFrom { get; init; }

    public int? IdsTo { get; init; }

    public float? WeightsFrom { get; init; }

    public float? WeightsTo { get; init; }

    public DateTime? AddedFrom { get; init; }

    public DateTime? AddedTo { get; init; }

    public DateTime? DeletedFrom { get; init; }

    public DateTime? DeletedTo { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IdsFrom > IdsTo)
            yield return new ValidationResult("IdsFrom must be less than or equal to IdsTo.",
                [nameof(IdsFrom), nameof(IdsTo)]);

        if (WeightsFrom > WeightsTo)
            yield return new ValidationResult("WeightsFrom must be less than or equal to WeightsTo.",
                [nameof(WeightsFrom), nameof(WeightsTo)]);

        if (AddedFrom > AddedTo)
            yield return new ValidationResult("AddedFrom must be less than or equal to AddedTo.",
                [nameof(AddedFrom), nameof(AddedTo)]);

        if (DeletedFrom > DeletedTo)
            yield return new ValidationResult("DeletedFrom must be less than or equal to DeletedTo.",
                [nameof(DeletedFrom), nameof(DeletedTo)]);
    }

    public SteelRollFilter ToFilter()
    {
        return new SteelRollFilter(
            IdsFrom,
            IdsTo,
            WeightsFrom,
            WeightsTo,
            AddedFrom,
            AddedTo,
            DeletedFrom,
            DeletedTo);
    }
}