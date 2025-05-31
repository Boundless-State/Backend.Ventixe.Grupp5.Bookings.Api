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
        //Arrange
        var context = GetInMemoryDbContext();
        var service = new BookingService(context);
        var booking = new BookingEntity { InvoiceId = "Z", BookingDate = DateTime.UtcNow, UserId = "u", CustomerName = "C", EventId = "E", EventName = "E", CategoryId = "C", CategoryName = "C", TicketCategoryId = "T", TicketCategoryName = "T", Price = 99, Quantity = 2 };
        var created = await service.CreateAsync(booking);

        //Act
        var result = await service.DeleteAsync(created.BookingId);
        var deleted = await service.GetByIdAsync(created.BookingId);

        //Assert
        Assert.True(result);
        Assert.Null(deleted);
    }
    [Fact]
    public async Task UpdateAsync_IfBookingExists_ReturnsTruePlusUpdates()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookingService(context);
        var booking = new BookingEntity
        {
            InvoiceId = "INV-002",
            BookingDate = DateTime.UtcNow,
            UserId = "user789",
            CustomerName = "Update Test",
            EventId = "event123",
            EventName = "Update Event",
            CategoryId = "cat002",
            CategoryName = "UpdateCat",
            TicketCategoryId = "tc002",
            TicketCategoryName = "Standard",
            Price = 100,
            Quantity = 1
        };
        var created = await service.CreateAsync(booking);

        // Act
        created.Quantity = 5;
        created.Status = BookingStatus.Confirmed;
        var success = await service.UpdateAsync(created);
        var updated = await service.GetByIdAsync(created.BookingId);

        // Assert
        Assert.True(success);
        Assert.Equal(5, updated?.Quantity);
        Assert.Equal(BookingStatus.Confirmed, updated?.Status);
    }
    [Fact]
    public async Task GetAllBookingsAsync_ReturnAllBookingsWhenOK()
    {
        var context = GetInMemoryDbContext();
        var service = new BookingService(context);

        var b1 = new BookingEntity { InvoiceId = "A", BookingDate = DateTime.UtcNow, UserId = "u1", CustomerName = "Test1", EventId = "e1", EventName = "E1", CategoryId = "c", CategoryName = "Cat", TicketCategoryId = "t", TicketCategoryName = "T", Price = 50, Quantity = 1 };
        var b2 = new BookingEntity { InvoiceId = "B", BookingDate = DateTime.UtcNow, UserId = "u2", CustomerName = "Test2", EventId = "e2", EventName = "E2", CategoryId = "c", CategoryName = "Cat", TicketCategoryId = "t", TicketCategoryName = "T", Price = 60, Quantity = 2 };

        await service.CreateAsync(b1);
        await service.CreateAsync(b2);

        var all = await service.GetAllBookingsAsync();

        Assert.Equal(2, all.Count());
    }


}