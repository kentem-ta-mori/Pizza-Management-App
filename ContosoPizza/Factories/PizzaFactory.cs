using ContosoPizza.Interfaces;
using ContosoPizza.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoPizza.Factories
{
    /// <summary>
    /// Pizzaオブジェクトの生成に関する責務を持つファクトリクラス。
    /// </summary>
    public class PizzaFactory
    {
        private readonly IBasePizzaRepository _basePizzaRepository;

        /// <summary>
        /// コンストラクタで、必要なリポジトリを注入
        /// </summary>
        /// <param name="basePizzaRepository">ベースピザのデータソース</param>
        public PizzaFactory(IBasePizzaRepository basePizzaRepository)
        {
            _basePizzaRepository = basePizzaRepository;
        }

        /// <summary>
        /// 指定されたピザと同じトッピング構成を持つ、より安価な代替案のピザを生成
        /// </summary>
        /// <param name="originalPizza">比較元となるピザ</param>
        /// <returns>より安い代替案が見つかった場合はそのPizzaインスタンス、見つからなければnull</returns>
        public async Task<Pizza?> CreateCheaperAlternativeAsync(Pizza originalPizza)
        {
            // 代替案となるピザのリストを取得
            var alternatives = await FindAlternativePizzasAsync(originalPizza);

            // 代替案の中から最も安いものを選択
            var cheapestAlternative = SelectCheapestPizza(alternatives);

            // 最安の代替案が元のピザより安いなら、それを返す
            if (cheapestAlternative != null && cheapestAlternative.TotalAmount < originalPizza.TotalAmount)
            {
                return cheapestAlternative;
            }
            // 元のピザが最安の選択肢なら、nullを返却
            return null;
        }

        /// <summary>
        /// 代替案となるピザのリストを検索・生成する
        /// </summary>
        private async Task<IEnumerable<Pizza>> FindAlternativePizzasAsync(Pizza originalPizza)
        {
            var neededToppings = originalPizza.AllToppings;
            var neededToppingsSet = new HashSet<Topping>(neededToppings);

            var allAvailableBasePizzas = await _basePizzaRepository.GetAllAsync();

            var candidateBases = allAvailableBasePizzas.Where(basePizza =>
                basePizza.Id != originalPizza.BasePizza.Id &&
                basePizza.DefaultToppings.All(defaultTopping => neededToppingsSet.Contains(defaultTopping))
            );

            return candidateBases.Select(basePizza => Pizza.CreateAlternative(basePizza, neededToppings));
        }

        /// <summary>
        /// ピザのシーケンスから最も合計金額が安いものを選択する
        /// </summary>
        private Pizza? SelectCheapestPizza(IEnumerable<Pizza> pizzas)
        {
            return pizzas.OrderBy(p => p.TotalAmount).FirstOrDefault();
        }
    }
}