using Microsoft.EntityFrameworkCore;
using Ventixe.Grupp5.Bookings.Api.Data;
using Ventixe.Grupp5.Bookings.Api.DTOs;
using Ventixe.Grupp5.Bookings.Api.Entities;

namespace Ventixe.Grupp5.Bookings.Api.Services;

public class BookingService
{
    private readonly BookingDbContext _context;

    public BookingService(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookingEntity>> GetAllAsync(BookingFilterDto filter, bool isAdmin, string? userId)
    {
        var query = _context.Bookings.AsQueryable();

        if (!isAdmin &&userId != null)
        {
            query = query.Where(q => q.UserId == userId);
        }

        if (filter.Statuses?.Any() == true)
        {
            query = query.Where(q => filter.Statuses.Contains(q.Status.ToString()));
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(b => b.BookingDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(b => b.BookingDate <= filter.ToDate.Value);
        }


        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(q =>
                q.CustomerName.Contains(filter.Search) ||
                q.EventName.Contains(filter.Search) ||
                q.InvoiceId.Contains(filter.Search));
        }

        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            if (filter.SortBy == "BookingDate")
                query = filter.SortDesc
                    ? query.OrderByDescending(q => q.BookingDate)
                    : query.OrderBy(q => q.BookingDate);

            else if (filter.SortBy == "Price")
                query = filter.SortDesc
                    ? query.OrderByDescending(q => q.Price)
                    : query.OrderBy(q => q.Price);

            else
                query = query.OrderBy(q => q.CreatedAt);
        }
        else
        {
            query = query.OrderByDescending(q => q.CreatedAt);
        }

        query = query.Skip(filter.Page * filter.PageSize)
            .Take(filter.PageSize);

        return await query.ToListAsync();

    }

    public async Task<BookingEntity?> GetByIdAsync(string bookingId)
    {
        return await _context.Bookings.FindAsync(bookingId);
    }

    public async Task<BookingEntity> CreateAsync(BookingEntity booking)
    {
        booking.BookingId = Guid.NewGuid().ToString();
        booking.CreatedAt = DateTime.UtcNow;

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        return booking;
    }

    public async Task<bool> UpdateAsync(BookingEntity updatedBooking)
    {
        var existing = await _context.Bookings.FindAsync(updatedBooking.BookingId);
        if (existing == null)
            return false;

        existing.Status = updatedBooking.Status;
        existing.Quantity = updatedBooking.Quantity;
        existing.EVoucher = updatedBooking.EVoucher;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null)
            return false;

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return true;
    }
}


