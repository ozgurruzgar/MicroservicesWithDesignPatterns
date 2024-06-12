using EventSourcing.API.Dtos;
using MediatR;

namespace EventSourcing.API.Queries
{
    public class GetProductAllListByUserId : IRequest<List<ProductDto>>
    {
        public int UserId { get; set; }
    }
}
