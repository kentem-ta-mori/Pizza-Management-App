namespace ContosoPizza.Models
{
    public class Topping
    {
        public string Id { get; }
        public string Name { get; }
        public int Price { get; }

        private Topping(string id, string name, int price)
        {
            Id = id;
            Name = name;
            Price = price;
        }

        public static readonly Topping Cheese = new("Cheese", "チーズ", 100);
        public static readonly Topping FriedGarlic = new("FriedGarlic", "フライドガーリック", 150);
        public static readonly Topping Mozzarella = new("Mozzarella", "モッツァレラチーズ", 300);
        public static readonly Topping SeafoodMix = new("SeafoodMix", "シーフードミックス", 500);
        public static readonly Topping Scallops = new("Scallops", "ホタテ", 500);
        public static readonly Topping Basil = new("Basil", "バジル", 100);
        public static readonly Topping Tomato = new("Tomato", "トマト", 250);
        public static readonly Topping Tuna = new("Tuna", "ツナ", 250);
        public static readonly Topping Corn = new("Corn", "コーン", 250);
        public static readonly Topping Bacon = new("Bacon", "ベーコン", 250);

        private static readonly List<Topping> _allToppings = new List<Topping>
        {
            Cheese, FriedGarlic, Mozzarella, SeafoodMix, Scallops,
            Basil, Tomato, Tuna, Corn, Bacon
        };
        public static IReadOnlyList<Topping> GetAll() => _allToppings.AsReadOnly();

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }
            return Id == ((Topping)obj).Id;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(Topping left, Topping right)
        {
            if (left is null)
            {
                return right is null;
            }
            return left.Equals(right);
        }
        public static bool operator !=(Topping left, Topping right) => !(left == right);
    }

    public class BasePizza
    {
        public string Id { get; }
        public string Name { get; }
        public int BasePrice { get; }
        public IReadOnlyList<Topping> DefaultToppings { get; }

        private BasePizza(string id, string name, int basePrice, IEnumerable<Topping> defaultToppings)
        {
            Id = id;
            Name = name;
            BasePrice = basePrice;
            DefaultToppings = new List<Topping>(defaultToppings ?? Enumerable.Empty<Topping>()).AsReadOnly();
        }

        public static readonly BasePizza Plain = new("Plain", "プレーン", 1200, new[] { Topping.Tomato });
        public static readonly BasePizza Margherita = new("Margherita", "マルゲリータ", 1500, new[] { Topping.Cheese, Topping.Tomato, Topping.Mozzarella, Topping.Basil });
        public static readonly BasePizza Seafood = new("Seafood", "シーフード", 1400, new[] { Topping.Cheese, Topping.SeafoodMix });
        public static readonly BasePizza Pescatore = new("Pescatore", "ペスカトーレ", 1800, new[] { Topping.Cheese, Topping.SeafoodMix, Topping.Scallops });
        public static readonly BasePizza Bambino = new("Bambino", "バンビーノ", 1600, new[] { Topping.Cheese, Topping.Tomato, Topping.Tuna, Topping.Corn, Topping.Bacon });

        private static readonly List<BasePizza> _allPizzas = new List<BasePizza>
        {
            Plain, Margherita, Seafood, Pescatore, Bambino
        };
        public static IReadOnlyList<BasePizza> GetAll() => _allPizzas.AsReadOnly();

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }
            return Id == ((BasePizza)obj).Id;
        }
        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(BasePizza left, BasePizza right)
        {
            if (left is null)
            {
                return right is null;
            }
            return left.Equals(right);
        }
        public static bool operator !=(BasePizza left, BasePizza right) => !(left == right);
    }

    public static class MenuCatalog
    {
        public static readonly IReadOnlyList<BasePizza> PizzaTypes = BasePizza.GetAll();
        public static readonly IReadOnlyList<Topping> ToppingTypes = Topping.GetAll();
    }
}