using EventSourcing.API.Dtos;
using EventSourcing.API.Models;
using EventSourcing.API.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.API.Handlers
{
    public class GetProductAllListByUserIdHandler : IRequestHandler<GetProductAllListByUserId, List<ProductDto>>
    {
        private readonly AppDbContext _dbContext;

        public GetProductAllListByUserIdHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductDto>> Handle(GetProductAllListByUserId request, CancellationToken cancellationToken)
        {
            var products = await _dbContext.Products.Where(x => x.UserId == request.UserId).ToListAsync();

            return products.Select(x => new ProductDto { Id = x.Id, Name = x.Name, Price = x.Price, Stock = x.Stock, UserId = x.UserId }).ToList();
        }
    }
}
