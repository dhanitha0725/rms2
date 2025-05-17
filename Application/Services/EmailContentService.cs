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

            // Fetch reserved packages
            var reservedPackages = (await reservedPackageRepository.GetAllAsync(cancellationToken))
                .Where(rp => rp.ReservationID == reservationId)
                .ToList();

            // Fetch reserved rooms
            var reservedRooms = (await reservedRoomRepository.GetAllAsync(cancellationToken))
                .Where(rr => rr.ReservationID == reservationId)
                .ToList();

            // Build packages HTML
            var packagesHtml = new StringBuilder();
            foreach (var rp in reservedPackages)
            {
                var package = await packageRepository.GetByIdAsync(rp.PackageID, cancellationToken);
                var facility = package != null
                    ? await facilityRepository.GetByIdAsync(package.FacilityID, cancellationToken)
                    : null;

                packagesHtml.Append($@"
                    <div class=""item"">
                        <div class=""item-title"">Package: {package?.PackageName ?? "N/A"}</div>
                        <div class=""item-facility"">Facility: {facility?.FacilityName ?? "N/A"}</div>
                    </div>");
            }

            // Build rooms HTML
            var roomsHtml = new StringBuilder();
            foreach (var rr in reservedRooms)
            {
                var room = await roomRepository.GetByIdAsync(rr.RoomID, cancellationToken);
                var facility = room != null
                    ? await facilityRepository.GetByIdAsync(room.FacilityID, cancellationToken)
                    : null;
                var roomType = room != null
                    ? await roomTypeRepository.GetByIdAsync(room.RoomTypeID, cancellationToken)
                    : null;

                roomsHtml.Append($@"
                    <div class=""item"">
                        <div class=""item-title"">Room: {roomType?.TypeName ?? "N/A"}</div>
                        <div class=""item-facility"">Facility: {facility?.FacilityName ?? "N/A"}</div>
                    </div>");
            }

            // Build email HTML
            var emailHtml = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Reservation Confirmation</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            margin: 0;
            padding: 0;
            background-color: #f9f9f9;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
        }}
        .header {{
            background: linear-gradient(135deg, #1e5799 0%, #207cca 100%);
            color: white;
            padding: 30px 20px;
            text-align: center;
        }}
        .logo {{
            margin-bottom: 15px;
            font-size: 24px;
            font-weight: bold;
        }}
        .confirmation-text {{
            font-size: 22px;
            margin: 0;
        }}
        .content {{
            padding: 30px 25px;
        }}
        .reservation-id {{
            background-color: #f5f9ff;
            border-left: 4px solid #207cca;
            padding: 10px 15px;
            margin-bottom: 25px;
            font-size: 16px;
        }}
        .reservation-id span {{
            font-weight: bold;
            color: #207cca;
        }}
        .reservation-details {{
            margin-bottom: 25px;
        }}
        .section-title {{
            color: #207cca;
            border-bottom: 1px solid #eaeaea;
            padding-bottom: 8px;
            margin-top: 25px;
            margin-bottom: 15px;
            font-size: 18px;
        }}
        .detail-row {{
            display: flex;
            margin-bottom: 12px;
            flex-wrap: wrap;
        }}
        .detail-label {{
            flex: 0 0 130px;
            font-weight: bold;
            color: #555;
        }}
        .detail-value {{
            flex: 1;
            color: #333;
        }}
        .items-container {{
            margin-bottom: 20px;
        }}
        .item {{
            background-color: #f9f9f9;
            border-radius: 6px;
            padding: 15px;
            margin-bottom: 10px;
            border-left: 3px solid #207cca;
        }}
        .item-title {{
            font-weight: bold;
            margin-bottom: 5px;
            color: #207cca;
        }}
        .item-facility {{
            color: #666;
            font-size: 14px;
        }}
        .total-section {{
            background-color: #f5f9ff;
            padding: 15px;
            border-radius: 6px;
            text-align: right;
            margin-top: 20px;
        }}
        .total-value {{
            font-size: 20px;
            font-weight: bold;
            color: #207cca;
        }}
        .footer {{
            background-color: #f5f5f5;
            padding: 20px;
            text-align: center;
            font-size: 14px;
            color: #777;
        }}
        .contact-info {{
            margin-top: 10px;
        }}
        .social-links {{
            margin-top: 15px;
        }}
        .social-links a {{
            margin: 0 5px;
            display: inline-block;
            color: #207cca;
            text-decoration: none;
        }}
        .thank-you {{
            margin-top: 25px;
            font-style: italic;
            text-align: center;
            color: #666;
        }}
        .support-note {{
            background-color: #fffcf5;
            border-left: 4px solid #ffcc66;
            padding: 10px 15px;
            margin-top: 25px;
            font-size: 14px;
            color: #776633;
        }}
        @media screen and (max-width: 600px) {{
            .detail-row {{
                flex-direction: column;
            }}
            .detail-label {{
                margin-bottom: 4px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""header"">
            <div class=""logo"">VENUE BOOKINGS</div>
            <h1 class=""confirmation-text"">Reservation Under Review</h1>
        </div>
        
        <div class=""content"">
            <div class=""reservation-id"">
                Reservation ID: <span>{reservation.ReservationID}</span>
            </div>
            
            <p>Dear Customer,</p>
            
            <p>Your reservation is under review. We will notify you once it is approved, along with payment instructions.</p>
            
            <div class=""reservation-details"">
                <h2 class=""section-title"">Reservation Details</h2>
                
                <div class=""detail-row"">
                    <div class=""detail-label"">Status:</div>
                    <div class=""detail-value"">{reservation.Status}</div>
                </div>
                
                <div class=""detail-row"">
                    <div class=""detail-label"">Start Date:</div>
                    <div class=""detail-value"">{reservation.StartDate:yyyy-MM-dd HH:mm}</div>
                </div>
                
                <div class=""detail-row"">
                    <div class=""detail-label"">End Date:</div>
                    <div class=""detail-value"">{reservation.EndDate:yyyy-MM-dd HH:mm}</div>
                </div>
            </div>
            
            <h2 class=""section-title"">Reserved Items</h2>
            
            <div class=""items-container"">
                {packagesHtml}
                {roomsHtml}
            </div>
            
            <div class=""total-section"">
                <div>Total Amount</div>
                <div class=""total-value"">{reservation.Total:C}</div>
            </div>
           
            <div class=""thank-you"">
                Thank you for choosing National Institute of Co-operative Development.
            </div>
            
            <div class=""support-note"">
                If you have any questions about your reservation, please contact our support team.
            </div>
        </div>
        
        <div class=""footer"">
            <div>National Institute of Co-operative Development</div>
            <div class=""contact-info"">Email: support@nicd.lk | Phone: +94 11 123 4567</div>
        </div>
    </div>
</body>
</html>";

            return emailHtml;
        }
    }
}
