using MySpot.Api.Services;

namespace MySpot.Tests.Unit.Shared;

internal sealed class TestClock : IClock
{
    public DateTime Current() => new(2022, 2, 26);
}