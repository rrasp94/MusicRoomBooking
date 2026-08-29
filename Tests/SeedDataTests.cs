using Microsoft.EntityFrameworkCore;
using MusicRoomBooking.Data;
using MusicRoomBooking.Models;
using Xunit;

namespace MusicRoomBooking.Tests;

public class SeedDataTests
{
    private static ApplicationDbContext NewInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task SeedRoomsAsync_OnEmptyDatabase_CreatesRoomsWithEquipment()
    {
        using var db = NewInMemoryContext();

        await SeedData.SeedRoomsAsync(db);

        Assert.Equal(4, await db.Rooms.CountAsync());
        Assert.True(await db.Equipment.AnyAsync());
        Assert.All(await db.Rooms.ToListAsync(), r => Assert.True(r.IsActive));
    }

    [Fact]
    public async Task SeedRoomsAsync_WhenRoomsAlreadyExist_DoesNotAddMore()
    {
        using var db = NewInMemoryContext();
        db.Rooms.Add(new Room { Name = "Existing", Description = "x", Capacity = 1, IsActive = true });
        await db.SaveChangesAsync();

        await SeedData.SeedRoomsAsync(db);

        Assert.Equal(1, await db.Rooms.CountAsync());
    }

    [Fact]
    public async Task SeedSampleBookingsAsync_GivesDemoUserOnePastAndOneFutureReservation()
    {
        using var db = NewInMemoryContext();
        await SeedData.SeedRoomsAsync(db);
        var demo = new ApplicationUser { Id = "demo-id", UserName = "demo@x", Email = "demo@x" };
        db.Users.Add(demo);
        await db.SaveChangesAsync();

        await SeedData.SeedSampleBookingsAsync(db, demo);

        var reservations = await db.Reservations.ToListAsync();
        Assert.Equal(2, reservations.Count);
        Assert.Contains(reservations, r => r.StartTime < DateTime.UtcNow);
        Assert.Contains(reservations, r => r.StartTime > DateTime.UtcNow);
        Assert.All(reservations, r => Assert.Equal(demo.Id, r.UserId));
    }

    [Fact]
    public async Task SeedSampleBookingsAsync_WhenReservationsExist_DoesNothing()
    {
        using var db = NewInMemoryContext();
        await SeedData.SeedRoomsAsync(db);
        var roomId = (await db.Rooms.FirstAsync()).Id;
        db.Reservations.Add(new Reservation
        {
            RoomId = roomId,
            UserId = "someone",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(2)
        });
        await db.SaveChangesAsync();

        await SeedData.SeedSampleBookingsAsync(db, new ApplicationUser { Id = "demo-id" });

        Assert.Equal(1, await db.Reservations.CountAsync());
    }
}
