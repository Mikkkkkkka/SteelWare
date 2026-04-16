using System.ComponentModel.DataAnnotations;

namespace SteelWare.Presentation.Http.Contracts;

public sealed record AddSteelRollHttpRequest(
    [property: Range(0.000_001d, double.MaxValue)]
    float Length,
    [property: Range(0.000_001d, double.MaxValue)]
    float Weight);