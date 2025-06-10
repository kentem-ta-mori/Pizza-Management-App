using ContosoPizza.Interfaces;
using ContosoPizza.Models;

namespace ContosoPizza.Repositories.Mocks
{
    public class MockToppingRepository : IToppingRepository
    {
        // 以前のTypeSafe Enumのデータをモックとして保持
        private static readonly List<Topping> _toppings = new()
        {
            new Topping(new ToppingId(0), "チーズ", 100),
            new Topping(new ToppingId(1), "フライドガーリック", 150),
            new Topping(new ToppingId(2), "モッツァレラチーズ", 300),
            new Topping(new ToppingId(3), "シーフードミックス", 500),
            new Topping(new ToppingId(4), "ホタテ", 500),
            new Topping(new ToppingId(5), "バジル", 100),
            new Topping(new ToppingId(6), "トマト", 250),
            new Topping(new ToppingId(7), "ツナ", 250),
            new Topping(new ToppingId(8), "コーン", 250),
            new Topping(new ToppingId(9), "ベーコン", 250)
        };

        public Task<IEnumerable<Topping>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Topping>>(_toppings);
        }

        public Task<Topping?> GetByIdAsync(ToppingId id)
        {
            return Task.FromResult(_toppings.FirstOrDefault(t => t.Id == id));
        }
    }
}