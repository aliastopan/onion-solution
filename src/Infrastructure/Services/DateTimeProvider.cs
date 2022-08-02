using Onion.Application.Common.Interfaces;

namespace Onion.Infrastructure.Services;

public class DateTimeProvider : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}
