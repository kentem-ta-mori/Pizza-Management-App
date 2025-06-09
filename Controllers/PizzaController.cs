using ContosoPizza.DTOs;
using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("[controller]")]
public class PizzaController : ControllerBase
{
    // public PizzaController()
    // {
    // }

    [HttpGet]
    public ActionResult<List<OrderedMenu>> GetAll() =>
    PizzaService.GetAll();

    [HttpGet("{id}")]
    public ActionResult<OrderedMenu> Get(int id)
    {
        var order = PizzaService.Get(id);

        if (order == null)
            return NotFound();

        return order;
    }

    [HttpPost]
    public ActionResult CreateOrder([FromBody] OrderedMenueRequestDto orderRequestDto)
    {
        try
        {
            OrderedMenu orderedMenue = PizzaService.ConvertToDomainModel(orderRequestDto, null);
            Action<OrderedMenu> persistAction = PizzaService.Add;
            OrderProcessingResult result = PizzaService.HandleOrder(orderedMenue, orderRequestDto.orderStatus, persistAction);

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
            else if (result.ProcessedOrder != null)
            {
                return CreatedAtAction(nameof(Get), new { id = result.ProcessedOrder.Id }, result.ProcessedOrder);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "注文処理中に予期せぬエラーが発生しました。" });
            }
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "サーバー内部でエラーが発生しました: " + ex });
        }
    }

    [HttpPut("{id}")]
    public ActionResult UpdateOrder(int id, [FromBody] OrderedMenueRequestDto orderRequestDto)
    {
        try
        {
            var existingOrder = PizzaService.Get(id);
            if (existingOrder == null)
            {
                return NotFound(new { message = $"ID {id} の注文は見つかりません。" });
            }

            OrderedMenu orderedMenueToUpdate = PizzaService.ConvertToDomainModel(orderRequestDto, id);
            Action<OrderedMenu> persistAction = PizzaService.Update;

            OrderProcessingResult result = PizzaService.HandleOrder(orderedMenueToUpdate, orderRequestDto.orderStatus, persistAction);

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
            else if (result.ProcessedOrder != null)
            {
                return NoContent();
            }
            else
            {
                return BadRequest( new { message = "注文処理中に予期せぬエラーが発生しました。" });
            }
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "サーバー内部でエラーが発生しました: " + ex });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var pizza = PizzaService.Get(id);

        if (pizza is null)
            return NotFound();

        PizzaService.Delete(id);

        return NoContent();
    }
}