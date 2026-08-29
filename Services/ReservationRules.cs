namespace MusicRoomBooking.Services;

// Pure reservation-time helpers, kept separate from the UI so they can be unit tested.
public static class ReservationRules
{
    // Two time ranges overlap when each one starts before the other ends.
    // Adjacent ranges (e.g. 10:00-12:00 and 12:00-14:00) do not overlap.
    public static bool Overlaps(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
        => startA < endB && startB < endA;

    // Builds the free, hour-aligned slots for a day: fixed blocks of the chosen duration within
    // opening hours, skipping slots that are in the past or overlap an existing reservation.
    public static List<(DateTime Start, DateTime End)> BuildAvailableSlots(
        DateTime day, int durationHours, int openingHour, int closingHour,
        DateTime nowUtc, IEnumerable<(DateTime Start, DateTime End)> existing)
    {
        var dayStart = day.Date;
        var existingList = existing.ToList();
        var slots = new List<(DateTime Start, DateTime End)>();

        for (var hour = openingHour; hour + durationHours <= closingHour; hour += durationHours)
        {
            var start = dayStart.AddHours(hour);
            var end = start.AddHours(durationHours);

            if (start < nowUtc)
            {
                continue;
            }
            if (existingList.Any(e => Overlaps(start, end, e.Start, e.End)))
            {
                continue;
            }
            slots.Add((start, end));
        }

        return slots;
    }
}
