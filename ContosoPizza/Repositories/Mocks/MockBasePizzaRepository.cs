using ContosoPizza.Interfaces;
using ContosoPizza.Models;

namespace ContosoPizza.Repositories.Mocks
{
    public class MockBasePizzaRepository : IBasePizzaRepository
    {
        // Toppingのモックデータ（本来はToppingリポジトリから取得すべきだが、ここでは簡略化のため定義）
        private static readonly Topping Cheese = new(new ToppingId(0), "チーズ", 100);
        private static readonly Topping FriedGarlic = new(new ToppingId(1), "フライドガーリック", 150);
        private static readonly Topping Mozzarella = new(new ToppingId(2), "モッツァレラチーズ", 300);
        private static readonly Topping SeafoodMix = new(new ToppingId(3), "シーフードミックス", 500);
        private static readonly Topping Scallops = new(new ToppingId(4), "ホタテ", 500);
        private static readonly Topping Basil = new(new ToppingId(5), "バジル", 100);
        private static readonly Topping Tomato = new(new ToppingId(6), "トマト", 250);
        private static readonly Topping Tuna = new(new ToppingId(7), "ツナ", 250);
        private static readonly Topping Corn = new(new ToppingId(8), "コーン", 250);
        private static readonly Topping Bacon = new(new ToppingId(9), "ベーコン", 250);

        private static readonly List<BasePizza> _pizzas = new()
        {
            new BasePizza(new BasePizzaId(0), "プレーン", 1200, new[] { Tomato, Cheese }),
            new BasePizza(new BasePizzaId(1), "マルゲリータ", 1500, new[] { Cheese, Tomato, Mozzarella, Basil }),
            new BasePizza(new BasePizzaId(2), "シーフード", 1400, new[] { Cheese, SeafoodMix }),
            new BasePizza(new BasePizzaId(3), "ペスカトーレ", 1800, new[] { Cheese, SeafoodMix, Scallops }),
            new BasePizza(new BasePizzaId(4), "バンビーノ", 1600, new[] { Cheese, Tomato, Tuna, Corn, Bacon })
        };

        public Task<IEnumerable<BasePizza>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<BasePizza>>(_pizzas);
        }

        public Task<BasePizza?> GetByIdAsync(BasePizzaId id)
        {
            return Task.FromResult(_pizzas.FirstOrDefault(p => p.Id == id));
        }
    }
}