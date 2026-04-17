namespace SteelWare.Application.Models;

public record struct SteelRollFilter(
    int? IdsFrom,
    int? IdsTo,
    float? LengthsFrom,
    float? LengthsTo,
    float? WeightsFrom,
    float? WeightsTo,
    DateTime? AddedFrom,
    DateTime? AddedTo,
    DateTime? DeletedFrom,
    DateTime? DeletedTo);