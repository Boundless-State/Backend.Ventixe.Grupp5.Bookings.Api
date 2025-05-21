using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ventixe.Grupp5.Bookings.Api.DTOs;
using Ventixe.Grupp5.Bookings.Api.Entities;
using Ventixe.Grupp5.Bookings.Api.Services;

namespace Ventixe.Grupp5.Bookings.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly BookingService _service;

    public BookingsController(BookingService service)
    {
        _service = service;
    }

    /// <summary>
    /// Hämtar alla bokningar i systemet (endast Admin).
    /// </summary>
    /// <param name="filter">Filter för sökning, sortering och paging.</param>
    [Authorize(Roles = "Admin")]
    [HttpGet("admin-bookings")]
    public async Task<IActionResult> GetAllBookings([FromQuery] BookingFilterDto filter)
    {
        var result = await _service.GetAllAsync(filter, true, null);
        return Ok(result);
    }

    /// <summary>
    /// Hämtar alla bokningar för den inloggade medlemmen.
    /// </summary>
    /// <param name="filter">Filter för sökning, sortering och paging.</param>
    [Authorize(Roles = "Member")]
    [HttpGet("user-bookings")]
    public async Task<IActionResult> GetUserBookings([FromQuery] BookingFilterDto filter)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId is null
            ? Unauthorized()
            : Ok(await _service.GetAllAsync(filter, false, userId));
    }

    /// <summary>
    /// Hämtar en bokning med angivet ID (Admin eller ägare).
    /// </summary>
    /// <param name="id">BookingId för bokningen.</param>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var booking = await _service.GetByIdAsync(id);
        return booking is not null ? Ok(booking) : NotFound();
    }

    /// <summary>
    /// Skapar en ny bokning (endast för inloggade medlemmar).
    /// </summary>
    /// <param name="dto">Information om bokningen.</param>
    [Authorize(Roles = "Member")]
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        dto.UserId = userId;
        dto.CustomerName = User.Identity?.Name ?? "Member";

        var booking = new BookingEntity
        {
            InvoiceId = dto.InvoiceId,
            BookingDate = dto.BookingDate,
            UserId = dto.UserId,
            CustomerName = dto.CustomerName,
            EventId = dto.EventId,
            EventName = dto.EventName,
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            TicketCategoryId = dto.TicketCategoryId,
            TicketCategoryName = dto.TicketCategoryName,
            Price = dto.Price,
            Quantity = dto.Quantity,
            EVoucher = dto.EVoucher,
            Status = BookingStatus.Pending
        };

        var created = await _service.CreateAsync(booking);
        return CreatedAtAction(nameof(GetById), new { id = created.BookingId }, created);
    }

    /// <summary>
    /// Uppdaterar en bokning (status, antal, e-voucher).
    /// </summary>
    /// <param name="id">BookingId att uppdatera.</param>
    /// <param name="dto">Uppdaterad information.</param>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBooking(string id, [FromBody] UpdateBookingDto dto)
    {
        var updated = new BookingEntity
        {
            BookingId = id,
            Status = dto.Status,
            Quantity = dto.Quantity,
            EVoucher = dto.EVoucher
        };

        return await _service.UpdateAsync(updated) 
            ? NoContent() 
            : NotFound();
    }

    /// <summary>
    /// Tar bort en bokning (endast för Admin).
    /// </summary>
    /// <param name="id">BookingId att ta bort.</param>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        return await _service.DeleteAsync(id)
            ? NoContent()
            : NotFound();
    }

    /// <summary>
    /// Avbokar en bokning för inloggad medlem (ändrar status till Cancelled).
    /// </summary>
    /// <param name="id">BookingId att avboka.</param>
    [Authorize(Roles = "Member")]
    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> MemberCancelBooking(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var booking = await _service.GetByIdAsync(id);
        if (booking == null || booking.UserId != userId)
            return NotFound();

        booking.Status = BookingStatus.Cancelled;
        var result = await _service.UpdateAsync(booking);

        return result ? Ok() : BadRequest();
    }

    /// <summary>
    /// Hämtar statistik över bokningar (antal, biljetter och intäkter) = (Endast Admin).
    /// </summary>
    /// <returns>
    /// Ett <see cref="BookingStatsDto"/>-objekt som hämtar all bokningsstatistik.
    /// </returns>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _service.GetStatisticsAsync();
        return Ok(stats);
    }

}
