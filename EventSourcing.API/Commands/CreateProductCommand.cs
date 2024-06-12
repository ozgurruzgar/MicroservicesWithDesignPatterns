using EventSourcing.API.Dtos;
using MediatR;

namespace EventSourcing.API.Commands
{
    public class CreateProductCommand:IRequest<Unit>
    {
        public CreateProductDto CreateProductDto { get; set; }
    }
}
