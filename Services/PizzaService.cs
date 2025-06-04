using ContosoPizza.DTOs;
using ContosoPizza.Models;

namespace ContosoPizza.Services;

public static class PizzaService
{
    static List<OrderedMenue> OrderedMenues { get; }
    static int nextId = 3;

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

    public static OrderedMenue? Get(int id) => OrderedMenues.FirstOrDefault(o => o.Id == id);

    public static OrderedMenue ProcessOrder(OrderedMenueRequestDto orderRequestDto)
    {
        BasePizza selectedBasePizza = BasePizza.GetById(orderRequestDto.CustomedPiza.BasePizzaId);
        if (selectedBasePizza == null)
        {
            throw new ArgumentException($"Invalid BasePizzaId: {orderRequestDto.CustomedPiza.BasePizzaId}");
        }

        // オプショントッピングのリストを取得
        var selectedOptionToppings = new List<Topping>();
        if (orderRequestDto.CustomedPiza.OptionToppingIds != null)
        {
            foreach (int toppingId in orderRequestDto.CustomedPiza.OptionToppingIds)
            {
                Topping selectedTopping = Topping.GetById(toppingId);
                if (selectedTopping == null)
                {
                    throw new ArgumentException($"Invalid ToppingId: {toppingId}");
                }
                selectedOptionToppings.Add(selectedTopping);
            }
        }

        Pizza pizzaDomainModel = new Pizza(
            selectedBasePizza,
            selectedOptionToppings.ToArray()
        );

        OrderedMenue orderedMenueDomainModel = new OrderedMenue
        {
            Id = 0,// 採番についてはいったん考慮しない（後ほどstaticな値をインクリメントする）
            CustomedPiza = pizzaDomainModel
        };

        return orderedMenueDomainModel;
    }

    public static bool NewOrderCompleted(OrderedMenue newOrder)
    {
        // より安い選択肢が存在する場合は、一度ユーザに確認するため、登録しない
        if (PizzaSuggester.CheaperAlternativeAvailable(newOrder.CustomedPiza)) return false;

        Add(newOrder);
        return true;

    }

    public static void Add(OrderedMenue order)
    {
        order.Id = nextId++;
        OrderedMenues.Add(order);
    }

    public static OrderedMenue AddRecommended(OrderedMenue order)
    {
        Pizza recommended = PizzaSuggester.GetCheaperAlternative(order.CustomedPiza);
        OrderedMenue cheaperMenue = new OrderedMenue { Id = nextId++, CustomedPiza = recommended };
        OrderedMenues.Add(cheaperMenue);
        return cheaperMenue;
    }

    public static void Delete(int id)
    {
        var order = Get(id);
        if (order is null)
            return;

        OrderedMenues.Remove(order);
    }

    // public static void Update(Pizza pizza)
    // {
    //     var index = Pizzas.FindIndex(p => p.Id == pizza.Id);
    //     if(index == -1)
    //         return;

    //     Pizzas[index] = pizza;
    // }
}