using ContosoPizza.Interfaces;
using ContosoPizza.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoPizza.Services
{
    public class PizzaSuggester : IPizzaSuggester
    {
        private readonly IBasePizzaRepository _basePizzaRepository;

        // コンストラクタでBasePizzaリポジトリを注入
        public PizzaSuggester(IBasePizzaRepository basePizzaRepository)
        {
            _basePizzaRepository = basePizzaRepository;
        }

        public async Task<bool> CheaperAlternativeAvailableAsync(Pizza original)
        {
            var cheaperAlternative = await GetCheaperAlternativeAsync(original);

            // 代替案が存在し、かつ元のピザとベースが異なる場合 true
            if (cheaperAlternative == null)
            {
                return false;
            }
            // 元のピザ自身が最安の場合は false になる
            return !original.BasePizza.Equals(cheaperAlternative.BasePizza);
        }

        public async Task<Pizza?> GetCheaperAlternativeAsync(Pizza original)
        {
            // すべてのトッピングを抽出する
            Topping[] allToppings = GetAllToppingsList(original);

            // 同じトッピングを持つ構成をチェックする
            Pizza[] similarPizzas = await GetSimilarPizzasAsync(allToppings);

            // すべての選択肢の中で、最も安いものを選択する
            var cheapestPizza = SelectCheapestPizza(similarPizzas);

            // 最安のピザと元のピザの価格を比較
            if (cheapestPizza != null && cheapestPizza.TotalAmount < original.TotalAmount)
            {
                return cheapestPizza;
            }

            // より安いものがなければ、元のピザを返す（あるいはnullを返しても良い）
            return original;
        }

        private Topping[] GetAllToppingsList(Pizza original)
        {
            var allToppings = new List<Topping>();
            if (original.BasePizza?.DefaultToppings != null)
            {
                allToppings.AddRange(original.BasePizza.DefaultToppings);
            }
            if (original.OptionTopings != null)
            {
                allToppings.AddRange(original.OptionTopings);
            }
            return allToppings.Distinct().ToArray();
        }

        private async Task<Pizza[]> GetSimilarPizzasAsync(Topping[] allToppings)
        {
            // トッピングのリストに含まれるデフォルトトッピングを持つベースピザを抽出
            BasePizza[] bases = await QueryBasePizzaAsync(allToppings);

            // ベースピザのデフォルトトッピングとallToppingを比較して不足しているトッピングを追加
            return SetSameTopping(bases, allToppings);
        }

        private async Task<BasePizza[]> QueryBasePizzaAsync(Topping[] allToppings)
        {
            var allAvailableBasePizzas = await _basePizzaRepository.GetAllAsync();

            var candidateBases = new List<BasePizza>();
            var allToppingsSet = new HashSet<Topping>(allToppings);

            foreach (BasePizza basePizza in allAvailableBasePizzas)
            {
                // このベースピザの全てのデフォルトトッピングが、元のピザの全トッピングリストに含まれているか
                if (basePizza.DefaultToppings.All(defaultTopping => allToppingsSet.Contains(defaultTopping)))
                {
                    candidateBases.Add(basePizza);
                }
            }
            return candidateBases.ToArray();
        }

        private Pizza[] SetSameTopping(BasePizza[] bases, Topping[] neededToppings)
        {
            var similarPizzas = new List<Pizza>();
            var neededToppingsSet = new HashSet<Topping>(neededToppings);

            foreach (BasePizza basePizza in bases)
            {
                // このベースピザを選んだ場合のオプショントッピングを決定
                var optionToppingsForThisBase = neededToppingsSet.Except(basePizza.DefaultToppings).ToArray();
                similarPizzas.Add(new Pizza(basePizza, optionToppingsForThisBase));
            }
            return similarPizzas.ToArray();
        }

        private Pizza? SelectCheapestPizza(Pizza[] similarPizzas)
        {
            return similarPizzas.OrderBy(p => p.TotalAmount).FirstOrDefault();
        }
    }
}