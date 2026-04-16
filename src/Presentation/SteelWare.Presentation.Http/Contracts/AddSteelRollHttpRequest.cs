using System.ComponentModel.DataAnnotations;

namespace SteelWare.Presentation.Http.Contracts;

public sealed class AddSteelRollHttpRequest
{
    [Range(0.000_001d, double.MaxValue)] public float Length { get; init; }

    [Range(0.000_001d, double.MaxValue)] public float Weight { get; init; }
}