using Domain.Common;
using MediatR;

namespace Application.Features.ManagePackages.DeletePackages
{
    public class DeletePackageCommand : IRequest<Result<int>>
    {
        public int PackageId { get; set; }
    }
}
