using Onion.Application.Common.Interfaces;

namespace Onion.Infrastructure.Services;

internal sealed class DateTimeProvider : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}
