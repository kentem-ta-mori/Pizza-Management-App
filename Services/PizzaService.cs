using ContosoPizza.Models;

namespace ContosoPizza.Services;

public static class PizzaService
{
    static List<OrderedMenue> OrderedMenues { get; }

    static PizzaService()
    {
        OrderedMenues = new List<OrderedMenue>
        {

            new OrderedMenue {
                Id=1,
                CustomedPiza=new Pizza(BasePizza.Plain, null)
            },
            new OrderedMenue {
                Id=2,
                CustomedPiza=new Pizza(BasePizza.Bambino, [Topping.Cheese])
            }
        };
    }

    public static List<OrderedMenue> GetAll() => OrderedMenues;

    // public static Pizza? Get(int id) => Pizzas.FirstOrDefault(p => p.Id == id);

    // public static void Add(Pizza pizza)
    // {
    //     pizza.Id = nextId++;
    //     Pizzas.Add(pizza);
    // }

    // public static void Delete(int id)
    // {
    //     var pizza = Get(id);
    //     if(pizza is null)
    //         return;

    //     Pizzas.Remove(pizza);
    // }

    // public static void Update(Pizza pizza)
    // {
    //     var index = Pizzas.FindIndex(p => p.Id == pizza.Id);
    //     if(index == -1)
    //         return;

    //     Pizzas[index] = pizza;
    // }
}