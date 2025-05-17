using Application.Abstractions.Interfaces;
using Domain.Entities;
using System.Text;

namespace Application.Services
{
    public class EmailContentService(
        IGenericRepository<Reservation, int> reservationRepository,
        IGenericRepository<ReservedPackage, int> reservedPackageRepository,
        IGenericRepository<ReservedRoom, int> reservedRoomRepository,
        IGenericRepository<Package, int> packageRepository,
        IGenericRepository<Room, int> roomRepository,
        IGenericRepository<Facility, int> facilityRepository,
        IGenericRepository<RoomType, int> roomTypeRepository) 
        : IEmailContentService
    {
        public async Task<string> GeneratePendingApprovalEmailBodyAsync(int reservationId, CancellationToken cancellationToken)
        {
            // Fetch reservation
            var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);
            if (reservation == null)
            {
                throw new InvalidOperationException($"Reservation not found for ID: {reservationId}");
            }

            // Build summary
            var summary = new StringBuilder();
            summary.AppendLine("Reservation Summary");
            summary.AppendLine("-------------------");
            summary.AppendLine($"Reservation ID: {reservation.ReservationID}");
            summary.AppendLine($"Status: {reservation.Status}");
            summary.AppendLine($"Start Date: {reservation.StartDate:yyyy-MM-dd HH:mm}");
            summary.AppendLine($"End Date: {reservation.EndDate:yyyy-MM-dd HH:mm}");
            summary.AppendLine($"Total: {reservation.Total:C}");
            summary.AppendLine();
            summary.AppendLine("Reserved Items:");
            summary.AppendLine();

            // Reserved Packages
            var reservedPackages = (await reservedPackageRepository.GetAllAsync(cancellationToken))
                .Where(rp => rp.ReservationID == reservationId)
                .ToList();

            foreach (var rp in reservedPackages)
            {
                var package = await packageRepository.GetByIdAsync(rp.PackageID, cancellationToken);
                var facility = package != null
                    ? await facilityRepository.GetByIdAsync(package.FacilityID, cancellationToken)
                    : null;
                summary.AppendLine($"- Package: {package?.PackageName ?? "N/A"} (Facility: {facility?.FacilityName ?? "N/A"})");
            }

            // Reserved Rooms
            var reservedRooms = (await reservedRoomRepository.GetAllAsync(cancellationToken))
                .Where(rr => rr.ReservationID == reservationId)
                .ToList();

            foreach (var rr in reservedRooms)
            {
                var room = await roomRepository.GetByIdAsync(rr.RoomID, cancellationToken);
                var facility = room != null
                    ? await facilityRepository.GetByIdAsync(room.FacilityID, cancellationToken)
                    : null;
                var roomType = room != null
                    ? await roomTypeRepository.GetByIdAsync(room.RoomTypeID, cancellationToken)
                    : null;
                summary.AppendLine($"- Room: {roomType?.TypeName ?? "N/A"} (Facility: {facility?.FacilityName ?? "N/A"})");
            }

            // Build email body
            var body = $"""
                        Dear Customer,

                        Your reservation (ID: {reservationId}) is under review. We will notify you once it is approved, along with payment instructions.

                        {summary}

                        Thank you,
                        National Institute of Co-operative Development
                        """;

            return body;
        }
    }
}
