using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ventixe.Grupp5.Bookings.Api.Data;
using Ventixe.Grupp5.Bookings.Api.Entities;
using Ventixe.Grupp5.Bookings.Api.Services;
using Xunit;

namespace BookingApi_Tests;

public class BookingServiceTests
{
    private BookingDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<BookingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new BookingDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_IfBookingIsOkReturnOK()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookingService(context);

        var booking = new BookingEntity
        {
            InvoiceId = "INV-001",
            BookingDate = DateTime.UtcNow,
            UserId = "user123",
            CustomerName = "Test User",
            EventId = "event001",
            EventName = "Sample Event",
            CategoryId = "cat001",
            CategoryName = "Music",
            TicketCategoryId = "tc001",
            TicketCategoryName = "VIP",
            Price = 299.99M,
            Quantity = 2
        };

        // Act
        var result = await service.CreateAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("INV-001", result.InvoiceId);
        Assert.Equal("Test User", result.CustomerName);
        Assert.Equal(2, result.Quantity);
        Assert.Equal(299.99M, result.Price);
        Assert.Equal(599.98M, result.TotalAmount);
    }
    [Fact]
    public async Task GetByIdAsync_ShowExistingBookings_IfCorrectReturnOK()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookingService(context);
        var booking = new BookingEntity { InvoiceId = "123", BookingDate = DateTime.UtcNow, UserId = "u1", CustomerName = "C", EventId = "E", EventName = "Ev", CategoryId = "C1", CategoryName = "Cat", TicketCategoryId = "T1", TicketCategoryName = "VIP", Price = 100, Quantity = 1 };
        await service.CreateAsync(booking);

        // Act
        var result = await service.GetByIdAsync(booking.BookingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(booking.BookingId, result.BookingId);
    }
    [Fact]
    public async Task DeleteAsync_IfBookingExistsInDbDeleteAndReturnOK()
    {
        var context = GetInMemoryDbContext();
        var service = new BookingService(context);

        var booking = new BookingEntity { InvoiceId = "Z", BookingDate = DateTime.UtcNow, UserId = "u", CustomerName = "C", EventId = "E", EventName = "E", CategoryId = "C", CategoryName = "C", TicketCategoryId = "T", TicketCategoryName = "T", Price = 99, Quantity = 2 };
        var created = await service.CreateAsync(booking);

        var result = await service.DeleteAsync(created.BookingId);
        var deleted = await service.GetByIdAsync(created.BookingId);

        Assert.True(result);
        Assert.Null(deleted);
    }
}