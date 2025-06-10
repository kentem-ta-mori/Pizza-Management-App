using System.Text.Json.Serialization;

namespace ContosoPizza.Models
{
    public record BasePizzaId(int Value);

    public class BasePizza
    {
        public BasePizzaId Id { get; init; }
        public string Name { get; init; }
        public int BasePrice { get; init; }
        public IReadOnlyList<Topping> DefaultToppings { get; init; }

        public BasePizza()
        {
            Name = string.Empty;
            DefaultToppings = new List<Topping>().AsReadOnly();
        }

        public BasePizza(BasePizzaId id, string name, int basePrice, IEnumerable<Topping> defaultToppings)
        {
            Id = id;
            Name = name;
            BasePrice = basePrice;
            DefaultToppings = new List<Topping>(defaultToppings ?? Enumerable.Empty<Topping>()).AsReadOnly();
        }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }
            return Id == ((BasePizza)obj).Id;
        }
        public override int GetHashCode() => Id.GetHashCode();
    }
}