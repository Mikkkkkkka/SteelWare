using SteelWare.Application.Models;

namespace SteelWare.Application.Contracts;

public interface IStorageStatisticsService
{
    public Task<int> CountAdded();

    public Task<int> CountDeleted();

    public Task<float> AverageLength(TimePeriod period);

    public Task<float> AverageWeight(TimePeriod period);

    public Task<float> MaxLength(TimePeriod period);

    public Task<float> MinLength(TimePeriod period);

    public Task<float> MaxWeight(TimePeriod period);

    public Task<float> MinWeight(TimePeriod period);

    public Task<float> TotalWeight(TimePeriod period);

    public Task<DateTime> GetDayWithMinRollCount(TimePeriod period);

    public Task<DateTime> GetDayWithMaxRollCount(TimePeriod period);

    public Task<DateTime> GetDayWithMinTotalWeight(TimePeriod period);

    public Task<DateTime> GetDayWithMaxTotalWeight(TimePeriod period);
}
