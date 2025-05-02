using Microsoft.EntityFrameworkCore;
using Ventixe.Grupp5.Bookings.Api.Entities;

namespace Ventixe.Grupp5.Bookings.Api.Data;

public class BookingDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<BookingEntity> Bookings { get; set; }
}
