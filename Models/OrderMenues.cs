using Microsoft.OpenApi.Extensions;

namespace ContosoPizza.Models;

public class OrderedMenu
{
    public int Id { get; set; }

    public required Pizza CustomedPiza { get; init; }

    // ピザが複数にならない限り不要
    // public int TotalAmount { get; set; }
}

public class Pizza
{
    public BasePizza BasePizza { get; }

    public IReadOnlyList<Topping>? OptionTopings { get; }

    public int TotalAmount { get; }
    public Pizza( BasePizza basePizza, Topping[]? optionTopings)
    {
        
        this.BasePizza = basePizza;
        this.OptionTopings = optionTopings;

        int calculatedTotalAmount = this.BasePizza.BasePrice;
        if (this.OptionTopings != null)
        {
            foreach (Topping topping in this.OptionTopings)
            {
                calculatedTotalAmount += topping.Price;
            }
        }
        this.TotalAmount = calculatedTotalAmount;
    }
}

