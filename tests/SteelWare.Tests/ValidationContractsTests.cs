using System.ComponentModel.DataAnnotations;
using SteelWare.Presentation.Http.Contracts;
using Xunit;

namespace SteelWare.Tests;

public sealed class ValidationContractsTests
{
    [Fact]
    public void AddSteelRollHttpRequest_WithNonPositiveValues_IsInvalid()
    {
        var request = new AddSteelRollHttpRequest
        {
            Length = 0,
            Weight = -1
        };

        var results = Validate(request);

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void GetSteelRollsQuery_WithInvalidRanges_IsInvalid()
    {
        var query = new GetSteelRollsQuery
        {
            IdsFrom = 10,
            IdsTo = 1,
            LengthsFrom = 20,
            LengthsTo = 10,
            WeightsFrom = 100,
            WeightsTo = 10,
            AddedFrom = new DateTime(2026, 4, 10),
            AddedTo = new DateTime(2026, 4, 1),
            DeletedFrom = new DateTime(2026, 4, 10),
            DeletedTo = new DateTime(2026, 4, 1)
        };

        var results = Validate(query);

        Assert.Equal(5, results.Count);
    }

    [Fact]
    public void TimePeriodQuery_WithFromGreaterThanTo_IsInvalid()
    {
        var query = new TimePeriodQuery
        {
            From = new DateTime(2026, 4, 2),
            To = new DateTime(2026, 4, 1)
        };

        var results = Validate(query);

        Assert.Single(results);
    }

    private static List<ValidationResult> Validate(object instance)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(instance);

        Validator.TryValidateObject(instance, context, results, true);

        return results;
    }
}