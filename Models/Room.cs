using System.ComponentModel.DataAnnotations;

namespace MusicRoomBooking.Models;

public class Room
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 100)]
    public int Capacity { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public List<Equipment> EquipmentItems { get; set; } = new();
    public List<Reservation> Reservations { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}
