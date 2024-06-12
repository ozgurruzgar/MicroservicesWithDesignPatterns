using EventSourcing.API.Dtos;
using MediatR;

namespace EventSourcing.API.Commands
{
    public class ChangeProductPriceCommand : IRequest<Unit>
    {
        public ChangeProductPriceDto ChangeProductPriceDto { get; set; }
    }
}
