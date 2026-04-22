namespace FlowerEcommerce.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;

    DateTime IDateTimeService.SeaNow => ConvertUtcToSea(DateTime.UtcNow);

    public DateTime SeaNow()
    {
        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var convertedTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);
        return convertedTime;
    }

    public DateTimeOffset OffsetSeaNow()
    {
        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tz);
    }

    public DateTime ConvertUtcToSea(DateTime utcDateTime)
    {
        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        DateTime convertedTime = TimeZoneInfo.ConvertTime(utcDateTime, tz);
        return convertedTime;
    }

    public (DateTime, DateTime) GetFirstAndLastDateInCurrentMonth()
    {
        var now = UtcNow;
        var first = new DateTime(now.Year, now.Month, 1);
        var last = first.AddMonths(1).AddDays(-1);
        return (first, last);
    }

    public (DateTime, DateTime) GetFirstAndLastDateOfCurrentWeek()
    {
        var now = UtcNow;
        // Lấy ngày đầu tiên của tuần hiện tại (tính từ thứ Hai)
        var startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
        // Ngày cuối cùng của tuần hiện tại
        var endOfWeek = startOfWeek.AddDays(6);

        return (startOfWeek, endOfWeek);
    }

    public (DateTime, DateTime, DateTime) GetCurrentAndPreviousNextMonth()
    {
        var now = UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1);
        var previousMonthStart = currentMonthStart.AddMonths(-1);
        var nextMonthStart = currentMonthStart.AddMonths(1);
        return (currentMonthStart, previousMonthStart, nextMonthStart);
    }
}
