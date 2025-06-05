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

    public static OrderedMenue ConvertToDomainModel(OrderedMenueRequestDto orderRequestDto, int? existingId = null)
    {
        BasePizza? selectedBasePizza = BasePizza.GetById(orderRequestDto.CustomedPiza.BasePizzaId);
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
                Topping? selectedTopping = Topping.GetById(toppingId);
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
            Id = existingId ?? 0,
            CustomedPiza = pizzaDomainModel
        };

        return orderedMenueDomainModel;
    }

    public static OrderProcessingResult HandleOrder(
        OrderedMenue orderedMenue,
        OrderedMenueRequestDto.OrderStatus orderStatus,
        Action<OrderedMenue> persistOperation
    )
    {
        switch (orderStatus)
        {
            case OrderedMenueRequestDto.OrderStatus.firstTime:
                bool hasCheaperAlternative = PizzaSuggester.CheaperAlternativeAvailable(orderedMenue.CustomedPiza);
                if (hasCheaperAlternative)
                {
                    Pizza? cheaperPizza = PizzaSuggester.GetCheaperAlternative(orderedMenue.CustomedPiza);
                    return new OrderProcessingResult
                    {
                        ProcessedOrder = orderedMenue,
                        SuggestsAlternative = true,
                        SuggestedPizza = cheaperPizza,
                    };
                }
                else
                {
                    // 最安の選択であったため、永続化処理を実行
                    persistOperation(orderedMenue);
                    return new OrderProcessingResult
                    {
                        ProcessedOrder = orderedMenue,
                    };
                }

            case OrderedMenueRequestDto.OrderStatus.recommended:
                Pizza? recommendedPizza = PizzaSuggester.GetCheaperAlternative(orderedMenue.CustomedPiza);
                if (recommendedPizza == null)
                {
                    // 推奨が見つからない場合のエラーハンドリング
                    return new OrderProcessingResult { ErrorMessage = "推奨されるピザ構成が見つかりませんでした。" };
                }
                orderedMenue.CustomedPiza = recommendedPizza;

                persistOperation(orderedMenue); // 永続化処理を実行
                return new OrderProcessingResult
                {
                    ProcessedOrder = orderedMenue,
                };

            case OrderedMenueRequestDto.OrderStatus.original:
                persistOperation(orderedMenue); // 永続化処理を実行
                return new OrderProcessingResult
                {
                    ProcessedOrder = orderedMenue,
                };

            default:
                return new OrderProcessingResult { ErrorMessage = "不明な注文ステータスです。" };
        }

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

    public static void Update(OrderedMenue updateOrderedMenue)
    {
        var index = OrderedMenues.FindIndex(p => p.Id == updateOrderedMenue.Id);
        if (index == -1)
            return;

        OrderedMenues[index] = updateOrderedMenue;
    }

    public static void Delete(int id)
    {
        var order = Get(id);
        if (order is null)
            return;

        OrderedMenues.Remove(order);
    }

}