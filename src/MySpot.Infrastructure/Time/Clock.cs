using MySpot.Application.Services;
using MySpot.Core.Abstractions;

namespace MySpot.Infrastructure.Time;

internal sealed class Clock : IClock
{
    public DateTime Current() => DateTime.UtcNow;
}