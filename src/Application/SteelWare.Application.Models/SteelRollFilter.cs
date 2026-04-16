namespace SteelWare.Application.Models;

public record struct SteelRollFilter(
    int? IdsFrom,
    int? IdsTo,
    float? WeightsFrom,
    float? WeightsTo,
    DateTime? AddedFrom,
    DateTime? AddedTo,
    DateTime? DeletedFrom,
    DateTime? DeletedTo);