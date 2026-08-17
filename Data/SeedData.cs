using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicRoomBooking.Models;

namespace MusicRoomBooking.Data;

// Creates the two roles, a demo admin account and a set of sample rooms on startup, so the app is usable right after cloning.
public static class SeedData
{
    private const string AdminEmail = "admin@musicrooms.local";
    private const string AdminPassword = "Admin123!";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var db = services.GetRequiredService<ApplicationDbContext>();

        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (await userManager.FindByEmailAsync(AdminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User"
            };

            var result = await userManager.CreateAsync(admin, AdminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        await SeedRoomsAsync(db);
    }

    // Adds a few sample rooms with equipment the first time the app runs (only when there are no rooms yet).
    private static async Task SeedRoomsAsync(ApplicationDbContext db)
    {
        if (await db.Rooms.AnyAsync())
        {
            return;
        }

        var rooms = new List<Room>
        {
            new Room
            {
                Name = "Studio A – Rock Room",
                Description = "Sound-treated rehearsal room built for rock and metal bands, with a full backline and PA.",
                Capacity = 6,
                IsActive = true,
                ImageUrl = "https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?w=800&q=70&fm=jpg&fit=crop",
                EquipmentItems = new List<Equipment>
                {
                    new() { Name = "Drum kit", Description = "5-piece acoustic drum kit with hi-hat, crash and ride cymbals." },
                    new() { Name = "Guitar amplifier", Description = "100W tube guitar amplifier with a 4x12 cabinet." },
                    new() { Name = "Bass amplifier", Description = "300W bass amplifier with an 8x10 cabinet." },
                    new() { Name = "PA system", Description = "1000W PA with mixer and three vocal microphones." }
                }
            },
            new Room
            {
                Name = "Studio B – Recording Room",
                Description = "Quiet, acoustically treated room for recording vocals and single instruments.",
                Capacity = 4,
                IsActive = true,
                ImageUrl = "https://images.unsplash.com/photo-1598488035139-bdbb2231ce04?w=800&q=70&fm=jpg&fit=crop",
                EquipmentItems = new List<Equipment>
                {
                    new() { Name = "Condenser microphone", Description = "Large-diaphragm condenser microphone with pop filter and shock mount." },
                    new() { Name = "Audio interface", Description = "8-channel USB audio interface for multi-track recording." },
                    new() { Name = "Studio monitors", Description = "Pair of active near-field studio monitors." },
                    new() { Name = "Headphones", Description = "Four sets of closed-back monitoring headphones." }
                }
            },
            new Room
            {
                Name = "Rehearsal Hall",
                Description = "Large space for full-band rehearsals and small performances, with stage lighting.",
                Capacity = 10,
                IsActive = true,
                ImageUrl = "https://images.unsplash.com/photo-1471478331149-c72f17e33c73?w=800&q=70&fm=jpg&fit=crop",
                EquipmentItems = new List<Equipment>
                {
                    new() { Name = "Drum kit", Description = "7-piece acoustic drum kit with double bass pedal." },
                    new() { Name = "Guitar amplifiers", Description = "Two 100W guitar amplifiers with 4x12 cabinets." },
                    new() { Name = "Bass amplifier", Description = "500W bass amplifier with an 8x10 cabinet." },
                    new() { Name = "PA system", Description = "2000W PA with a 12-channel mixer, monitors and four microphones." },
                    new() { Name = "Stage lighting", Description = "Basic LED stage lighting rig." }
                }
            },
            new Room
            {
                Name = "Piano Room",
                Description = "Calm room with an acoustic grand piano, suitable for practice and lessons.",
                Capacity = 3,
                IsActive = true,
                ImageUrl = "https://images.unsplash.com/photo-1520523839897-bd0b52f945a0?w=800&q=70&fm=jpg&fit=crop",
                EquipmentItems = new List<Equipment>
                {
                    new() { Name = "Grand piano", Description = "Acoustic grand piano, tuned regularly." },
                    new() { Name = "Piano bench", Description = "Adjustable-height piano bench." },
                    new() { Name = "Metronome", Description = "Digital metronome with adjustable tempo and time signatures." }
                }
            }
        };

        db.Rooms.AddRange(rooms);
        await db.SaveChangesAsync();
    }
}
