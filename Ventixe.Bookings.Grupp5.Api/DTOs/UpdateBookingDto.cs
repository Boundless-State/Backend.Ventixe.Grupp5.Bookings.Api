using Ventixe.Grupp5.Bookings.Api.Entities;

namespace Ventixe.Grupp5.Bookings.Api.DTOs;

public class UpdateBookingDto
{
    public BookingStatus Status { get; set; }
    public int Quantity { get; set; }
    public string? EVoucher { get; set; }
}
