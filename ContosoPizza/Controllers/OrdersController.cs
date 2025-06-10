using ContosoPizza.DTOs;
using ContosoPizza.Interfaces;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IPizzaService _pizzaService;

        public OrdersController(IPizzaService pizzaService)
        {
            _pizzaService = pizzaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderedMenu>>> GetAll()
        {
            // サービスを非同期で呼び出し
            var orders = await _pizzaService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderedMenu>> Get(int id)
        {
            var order = await _pizzaService.GetOrderByIdAsync(id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderedMenueRequestDto orderRequestDto)
        {
            try
            {
                var orderedMenu = await _pizzaService.ConvertToDomainModelAsync(orderRequestDto);
                var result = await _pizzaService.HandleOrderAsync(orderedMenu, orderRequestDto.orderStatus);

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }

                if (result.SuggestsAlternative)
                {
                    return Ok(new
                    {
                        message = "より経済的な選択肢があります。ご確認ください。",
                        suggestedPizza = result.SuggestedPizza,
                        originalSelection = result.ProcessedOrder
                    });
                }

                if (result.ProcessedOrder != null)
                {
                    return CreatedAtAction(nameof(Get), new { id = result.ProcessedOrder.Id }, result.ProcessedOrder);
                }

                // 予期せぬ状態
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "注文処理中に予期せぬエラーが発生しました。" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "サーバー内部でエラーが発生しました。" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderedMenueRequestDto orderRequestDto)
        {
            try
            {
                var existingOrder = await _pizzaService.GetOrderByIdAsync(id);
                if (existingOrder == null)
                {
                    return NotFound(new { message = $"ID {id} の注文は見つかりません。" });
                }

                var orderedMenuToUpdate = await _pizzaService.ConvertToDomainModelAsync(orderRequestDto, id);
                var result = await _pizzaService.HandleOrderAsync(orderedMenuToUpdate, orderRequestDto.orderStatus);

                if (!string.IsNullOrEmpty(result.ErrorMessage))
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "サーバー内部でエラーが発生しました。" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _pizzaService.GetOrderByIdAsync(id);
            if (order is null)
                return NotFound();

            await _pizzaService.DeleteOrderAsync(id);

            return NoContent();
        }
    }
}