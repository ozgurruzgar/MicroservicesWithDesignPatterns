using MediatR;

namespace EventSourcing.API.Commands
{
    public class DeleteProductCommand:IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
