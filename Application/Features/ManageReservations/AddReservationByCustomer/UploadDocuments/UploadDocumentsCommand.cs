using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ManageReservations.AddReservationByCustomer.UploadDocuments
{
    public class UploadDocumentsCommand : IRequest<Result>
    {
        public int ReservationID { get; set; }
        public int UserID { get; set; }
        public List<IFormFile> Documents { get; set; }
    }
}
