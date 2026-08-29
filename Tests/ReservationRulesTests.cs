using MusicRoomBooking.Services;
using Xunit;

namespace MusicRoomBooking.Tests;

public class ReservationRulesTests
{
    private static readonly DateTime Day = new(2030, 1, 7); // a Monday well in the future
    private static readonly DateTime LongAgo = new(2000, 1, 1);

    [Fact]
    public void Overlaps_WhenRangesIntersect_ReturnsTrue()
    {
        var a = new DateTime(2030, 1, 7, 10, 0, 0);
        var b = new DateTime(2030, 1, 7, 12, 0, 0);
        Assert.True(ReservationRules.Overlaps(a, b,
            new DateTime(2030, 1, 7, 11, 0, 0), new DateTime(2030, 1, 7, 13, 0, 0)));
    }

    [Fact]
    public void Overlaps_WhenAdjacent_ReturnsFalse()
    {
        // 10:00-12:00 and 12:00-14:00 are back-to-back, not overlapping.
        Assert.False(ReservationRules.Overlaps(
            new DateTime(2030, 1, 7, 10, 0, 0), new DateTime(2030, 1, 7, 12, 0, 0),
            new DateTime(2030, 1, 7, 12, 0, 0), new DateTime(2030, 1, 7, 14, 0, 0)));
    }

    [Fact]
    public void Overlaps_WhenCompletelySeparate_ReturnsFalse()
    {
        Assert.False(ReservationRules.Overlaps(
            new DateTime(2030, 1, 7, 8, 0, 0), new DateTime(2030, 1, 7, 9, 0, 0),
            new DateTime(2030, 1, 7, 14, 0, 0), new DateTime(2030, 1, 7, 15, 0, 0)));
    }

    [Fact]
    public void Overlaps_WhenIdentical_ReturnsTrue()
    {
        var s = new DateTime(2030, 1, 7, 10, 0, 0);
        var e = new DateTime(2030, 1, 7, 12, 0, 0);
        Assert.True(ReservationRules.Overlaps(s, e, s, e));
    }

    [Fact]
    public void Overlaps_WhenOneContainsOther_ReturnsTrue()
    {
        Assert.True(ReservationRules.Overlaps(
            new DateTime(2030, 1, 7, 9, 0, 0), new DateTime(2030, 1, 7, 15, 0, 0),
            new DateTime(2030, 1, 7, 11, 0, 0), new DateTime(2030, 1, 7, 12, 0, 0)));
    }

    [Fact]
    public void BuildAvailableSlots_WithNoBookings_ReturnsAllBlocks()
    {
        var slots = ReservationRules.BuildAvailableSlots(
            Day, durationHours: 2, openingHour: 8, closingHour: 22, nowUtc: LongAgo,
            existing: Enumerable.Empty<(DateTime, DateTime)>());

        // 08-10, 10-12, 12-14, 14-16, 16-18, 18-20, 20-22
        Assert.Equal(7, slots.Count);
        Assert.Equal(Day.AddHours(8), slots.First().Start);
        Assert.Equal(Day.AddHours(22), slots.Last().End);
    }

    [Fact]
    public void BuildAvailableSlots_SkipsSlotOverlappingExistingReservation()
    {
        var existing = new[] { (Day.AddHours(10), Day.AddHours(12)) };

        var slots = ReservationRules.BuildAvailableSlots(
            Day, durationHours: 2, openingHour: 8, closingHour: 22, nowUtc: LongAgo, existing);

        Assert.Equal(6, slots.Count);
        Assert.DoesNotContain(slots, s => s.Start == Day.AddHours(10));
        Assert.Contains(slots, s => s.Start == Day.AddHours(8));  // adjacent block still offered
        Assert.Contains(slots, s => s.Start == Day.AddHours(12)); // adjacent block still offered
    }

    [Fact]
    public void BuildAvailableSlots_SkipsSlotsInThePast()
    {
        var now = Day.AddHours(13); // "now" is 13:00 on that day

        var slots = ReservationRules.BuildAvailableSlots(
            Day, durationHours: 2, openingHour: 8, closingHour: 22, nowUtc: now,
            existing: Enumerable.Empty<(DateTime, DateTime)>());

        // Only 14-16, 16-18, 18-20, 20-22 start at or after 13:00.
        Assert.Equal(4, slots.Count);
        Assert.All(slots, s => Assert.True(s.Start >= now));
    }
}
