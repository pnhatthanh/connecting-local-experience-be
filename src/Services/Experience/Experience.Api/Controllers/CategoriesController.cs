using Experience.Application.Dtos;
using Experience.Application.Handlers.Queries.GetCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Experience.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            var query = new GetCategoriesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
