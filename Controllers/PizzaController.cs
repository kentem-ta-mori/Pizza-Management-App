using ContosoPizza.DTOs;
using ContosoPizza.Models;
using ContosoPizza.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers;

[ApiController]
[Route("[controller]")]
public class PizzaController : ControllerBase
{
    public PizzaController()
    {
    }

    [HttpGet]
    public ActionResult<List<OrderedMenue>> GetAll() =>
    PizzaService.GetAll();

    [HttpGet("{id}")]
    public ActionResult<OrderedMenue> Get(int id)
    {
        var order = PizzaService.Get(id);

        if (order == null)
            return NotFound();

        return order;
    }

    [HttpPost]
    public ActionResult CreateOrder([FromBody] OrderedMenueRequestDto orderRequestDto)
    {
        try {
            // リクエストDTOを永続化用モデルにコンバート
            OrderedMenue orderedMenue = PizzaService.ProcessOrder(orderRequestDto);
            // 永続化（現在はstaticなListに追加するだけ）
            PizzaService.Add(orderedMenue);

            // 暫定で作成したオブジェクトを返却する
            return CreatedAtAction(nameof(Get), new { id = orderedMenue.Id }, orderedMenue);
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, ex);
        }
    }

    // [HttpPut("{id}")]
    // public IActionResult Update(int id, Pizza pizza)
    // {
    //     if (id != pizza.Id)
    //         return BadRequest();

    //     var existingPizza = PizzaService.Get(id);
    //     if (existingPizza is null)
    //         return NotFound();

    //     PizzaService.Update(pizza);

    //     return NoContent();
    // }

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