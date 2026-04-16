using SteelWare.Domain;

namespace SteelWare.Presentation.Http.Contracts;

public sealed record SteelRollResponse(
    int Id,
    float Length,
    float Weight,
    DateTime AddedAt,
    DateTime? DeletedAt)
{
    public static SteelRollResponse FromDomain(SteelRoll roll)
    {
        return new SteelRollResponse(
            roll.Id,
            roll.Length,
            roll.Weight,
            roll.AddedAt,
            roll.DeletedAt);
    }
}