using ContosoPizza.Interfaces;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToppingsController : ControllerBase
    {
        private readonly IToppingRepository _toppingRepository;

        public ToppingsController(IToppingRepository toppingRepository)
        {
            _toppingRepository = toppingRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Topping>>> GetToppings()
        {
            var toppings = await _toppingRepository.GetAllAsync();
            return Ok(toppings);
        }
    }
}
