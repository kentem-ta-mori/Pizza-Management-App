using ContosoPizza.Interfaces;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasePizzasController : ControllerBase
    {
        private readonly IBasePizzaRepository _basePizzaRepository;

        public BasePizzasController(IBasePizzaRepository basePizzaRepository)
        {
            _basePizzaRepository = basePizzaRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BasePizza>>> GetBasePizzas()
        {
            var pizzas = await _basePizzaRepository.GetAllAsync();
            return Ok(pizzas);
        }
    }
}