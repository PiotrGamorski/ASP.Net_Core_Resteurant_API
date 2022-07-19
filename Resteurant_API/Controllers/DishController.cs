using Microsoft.AspNetCore.Mvc;
using Resteurant_API.Dtos;
using Resteurant_API.Entities;
using Resteurant_API.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resteurant_API.Controllers
{
    [Route("api/resteurant/{resteurantId}/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _service;

        public DishController(IDishService service)
        {
            _service = service;
        }

        [HttpPut("{dishId}")]
        public async Task<ActionResult> Update([FromRoute] int resteurantId, [FromRoute] int dishId, [FromBody] UpdateDishDto dto)
        {
            await _service.Update(resteurantId, dishId, dto);
            return Ok();
        }


        [HttpDelete("{dishId}")]
        public async Task<ActionResult> Delete([FromRoute] int resteurantId, [FromRoute] int dishId)
        {
            await _service.Delete(resteurantId, dishId);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromRoute] int resteurantId, [FromBody] CreateDishDto dto)
        {
            var dish = await _service.Create(resteurantId, dto);

            return Created($"/api/resteurant/{resteurantId}/dish/{dish.Id}", null);
        }

        [HttpGet("{dishId}")]
        public async Task<ActionResult> GetById([FromRoute] int resteurantId, [FromRoute] int dishId)
        {
            var dishDto = await _service.GetById(resteurantId, dishId);
            return Ok(dishDto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dish>>> GetAll([FromRoute] int resteurantId)
        {
            var dishes = await _service.GetAll(resteurantId);
            return Ok(dishes);
        }
    }
}
