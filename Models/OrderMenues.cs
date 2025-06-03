using Microsoft.OpenApi.Extensions;

namespace ContosoPizza.Models;

public class OrderedMenue
{
    public int Id { get; set; }

    public required Pizza CustomedPiza { get; set; }

    // ピザが複数にならない限り不要
    // public int TotalAmount { get; set; }
}

public class Pizza
{
    public BasePizza Base { get; }

    public IReadOnlyList<Topping>? OptionTopings { get; }

    public int TotalAmount { get; }
    public Pizza( BasePizza type, Topping[]? topings)
    {
        
        this.Base = type;
        this.OptionTopings = topings;

        int calculatedTotalAmount = this.Base.BasePrice;
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

