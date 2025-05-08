using Domain.Common;
using MediatR;

namespace Application.Features.ManageFacility.AddRoomTypes
{
    public class AddRoomTypeCommand : IRequest<Result<int>>
    {
      public string TypeName { get; set; }
    }
  
}
