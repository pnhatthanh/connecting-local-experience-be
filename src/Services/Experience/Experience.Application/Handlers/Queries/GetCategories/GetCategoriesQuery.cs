using BuildingBlocks.Application.CQRS.Query;
using Experience.Application.Dtos;

namespace Experience.Application.Handlers.Queries.GetCategories
{
    public class GetCategoriesQuery : IQuery<List<CategoryDto>>
    {
    }
}
