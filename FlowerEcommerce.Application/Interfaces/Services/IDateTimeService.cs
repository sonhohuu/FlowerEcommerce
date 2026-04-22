namespace FlowerEcommerce.Application.Interfaces.Services;
public interface IDateTimeService
{
    public DateTime UtcNow { get; }
    public DateTimeOffset OffsetUtcNow { get; }
    public DateTime SeaNow { get; }
    public DateTimeOffset OffsetSeaNow();
    public DateTime ConvertUtcToSea(DateTime utcDateTime);
    public (DateTime, DateTime) GetFirstAndLastDateInCurrentMonth();
    public (DateTime, DateTime) GetFirstAndLastDateOfCurrentWeek();
    public (DateTime, DateTime, DateTime) GetCurrentAndPreviousNextMonth();
}
