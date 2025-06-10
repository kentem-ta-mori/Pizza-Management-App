using System.Text.Json.Serialization;

namespace ContosoPizza.Models
{
    public record ToppingId( int Value);

    public class Topping
    {
        public ToppingId Id { get; init; }
        public string Name { get; init; }
        public int Price { get; init; }
        
        public Topping(ToppingId id, string name, int price)
        {
            Id = id;
            Name = name;
            Price = price;
        }
        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }
            return Id == ((Topping)obj).Id;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}