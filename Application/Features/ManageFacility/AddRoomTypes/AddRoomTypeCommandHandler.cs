using Application.Abstractions.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Serilog;

namespace Application.Features.ManageFacility.AddRoomTypes
{
    public class AddRoomTypeCommandHandler (
        IGenericRepository<RoomType, int> roomTypeRepository,
        IUnitOfWork unitOfWork,
        ILogger logger): IRequestHandler<AddRoomTypeCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(
            AddRoomTypeCommand request,
            CancellationToken cancellationToken)
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // Fetch all room types
                var roomTypes = await roomTypeRepository.GetAllAsync(cancellationToken);

                // Check if room type already exists
                var existingRoomType = roomTypes.FirstOrDefault(rt => rt.TypeName == request.TypeName);
                if (existingRoomType != null)
                {
                    return Result<int>.Failure(new Error("Room type already exists"));
                }

                // Create new room type
                var roomType = new RoomType
                {
                    TypeName = request.TypeName,
                };

                await roomTypeRepository.AddAsync(roomType, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return Result<int>.Success(roomType.RoomTypeID);

            }
            catch (Exception e)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.Error(e, "Error adding room type");
                throw;
            }
        }
    }
}
