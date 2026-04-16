using System.ComponentModel.DataAnnotations;
using SteelWare.Application.Models;

namespace SteelWare.Presentation.Http.Contracts;

public sealed record TimePeriodQuery : IValidatableObject
{
    [Required] public DateTime? From { get; init; }

    [Required] public DateTime? To { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From > To)
            yield return new ValidationResult("From must be less than or equal to To.", [nameof(From), nameof(To)]);
    }

    public TimePeriod ToTimePeriod()
    {
        return new TimePeriod(From!.Value, To!.Value);
    }
}