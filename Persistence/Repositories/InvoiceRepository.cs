using Application.Abstractions.Interfaces;
using Application.DTOs.InvoiceDtos;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.DbContexts;
using System.Linq;

namespace Persistence.Repositories
{
    public class InvoiceRepository(ReservationDbContext context)
        : GenericRepository<Invoice, int>(context), IInvoiceRepository
    {
        public async Task<List<InvoiceTableDataDto>> GetInvoiceTableDataAsync(
            CancellationToken cancellationToken = default)
        {
            return await context.Invoices
                .Include(i => i.InvoicePayments)
                    .ThenInclude(ip => ip.Payment)
                .Select(i => new InvoiceTableDataDto
                {
                    InvoiceID = i.InvoiceID,
                    AmountPaid = i.AmountPaid,
                    AmountDue = i.AmountDue,
                    IssuedDate = i.IssuedDate,
                    ReservationID = i.ReservationID,
                    // Get the first payment associated with the invoice
                    PaymentID = i.InvoicePayments
                        .Select(ip => ip.PaymentID)
                        .FirstOrDefault(),
                    PaymentMethod = i.InvoicePayments
                        .Select(ip => ip.Payment.Method)
                        .FirstOrDefault() ?? "N/A",
                    PaymentStatus = i.InvoicePayments
                        .Select(ip => ip.Payment.Status)
                        .FirstOrDefault() ?? "N/A"
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<InvoiceDetailsDto> GetInvoiceDetailsAsync(
            int invoiceId,
            CancellationToken cancellationToken = default)
        {
            var invoice = await context.Invoices
                .Where(i => i.InvoiceID == invoiceId)
                .Include(i => i.Reservation)
                    .ThenInclude(r => r.ReservationUserDetail)
                .Include(i => i.InvoicePayments)
                    .ThenInclude(ip => ip.Payment)
                // Include packages for facility name
                .Include(i => i.Reservation.ReservedPackages.Take(1))
                    .ThenInclude(rp => rp.Package)
                        .ThenInclude(p => p.Facility)
                // Include rooms as fallback for facility name
                .Include(i => i.Reservation.ReservedRooms.Take(1))
                    .ThenInclude(rr => rr.Room)
                        .ThenInclude(r => r.Facility)
                .FirstOrDefaultAsync(cancellationToken);

            if (invoice == null)
            {
                return null;
            }

            string facilityName = invoice.Reservation.ReservedPackages?.FirstOrDefault()?.Package?.Facility?.FacilityName ??
                                invoice.Reservation.ReservedRooms?.FirstOrDefault()?.Room?.Facility?.FacilityName ??
                                "Unknown";

            // Get payment information
            var paymentId = invoice.InvoicePayments
                .Select(ip => ip.PaymentID)
                .FirstOrDefault();

            // Create the DTO without booked items
            return new InvoiceDetailsDto
            {
                InvoiceId = invoice.InvoiceID,
                PaymentId = paymentId,
                ReservationId = invoice.ReservationID,
                FacilityName = facilityName,
                TotalAmount = invoice.Reservation.Total,
                AmountPaid = invoice.AmountPaid,
                AmountDue = invoice.AmountDue,
                InvoiceDate = invoice.IssuedDate,
                CustomerName = $"{invoice.Reservation.ReservationUserDetail.FirstName} {invoice.Reservation.ReservationUserDetail.LastName}",
                CustomerEmail = invoice.Reservation.ReservationUserDetail.Email,
                ReservationStatus = invoice.Reservation.Status.ToString(),
                ReservationStartDate = invoice.Reservation.StartDate,
                ReservationEndDate = invoice.Reservation.EndDate
            };
        }
    }
}