using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Application.EventHandlers
{
    public class ExpireReservationTask(
        int reservationId,
        IServiceScopeFactory serviceScopeFactory,
        ILogger logger)
    {
        public async Task Execute(CancellationToken cancellationToken)
        {
            try
            {
                // Create a new scope to get fresh instances of repositories
                using var scope = serviceScopeFactory.CreateScope();
                var reservationRepository = scope.ServiceProvider
                    .GetRequiredService<IGenericRepository<Reservation, int>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // Fetch the reservation by ID
                var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);

                if (reservation == null)
                {
                    logger.Warning("Reservation with ID {ReservationId} not found for expiration.", reservationId);
                    return;
                }

                // Only expire if still in PendingPayment status
                if (reservation.Status == ReservationStatus.PendingPayment)
                {
                    await unitOfWork.BeginTransactionAsync(cancellationToken);

                    try
                    {
                        reservation.Status = ReservationStatus.Expired;
                        reservation.UpdatedDate = DateTime.UtcNow;

                        await reservationRepository.UpdateAsync(reservation, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);
                        await unitOfWork.CommitTransactionAsync(cancellationToken);

                        logger.Information("Reservation {ReservationId} marked as expired after payment timeout", reservationId);
                    }
                    catch (Exception ex)
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);
                        logger.Error(ex, "Failed to expire reservation {ReservationId}", reservationId);
                        throw;
                    }
                }
                else
                {
                    logger.Information("Reservation {ReservationId} not expired, current status is {Status}",
                        reservationId, reservation.Status);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error processing reservation expiration for {ReservationId}", reservationId);
                throw;
            }
        }
    }
}
