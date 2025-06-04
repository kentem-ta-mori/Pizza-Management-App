using ContosoPizza.Models;

public static class PizzaSuggester
{
    public static bool CheaperAlternativeAvailable(Pizza oridinal)
    {
        // もともとのピザと最安のピザのベースが同じかの判定を返す
        return !oridinal.BasePizza.Equals(GetCheaperAlternative(oridinal).BasePizza);
    }

    public static Pizza GetCheaperAlternative(Pizza oridinal)
    {
        // より安いピザの選択肢があるかチェックする
        // すべてのトッピングを抽出する
        Topping[] AllToppings = GetALLTopingsList(oridinal);
        // 同じトッピングを持つ構成をチェックする
        Pizza[] similarPizzas = GetSimilerPizzas(AllToppings);
        // すべての選択肢の中で、最も安いものを選択する
        return SelectCheapestPizza(similarPizzas) ?? oridinal;
    }

    private static Topping[] GetALLTopingsList(Pizza oridinal)
    {
        // 元のピザのデフォルトトッピングとオプショントッピングの配列を作成
        var allToppings = new List<Topping>();

        // デフォルトトッピングを追加
        if (oridinal.BasePizza?.DefaultToppings != null)
        {
            allToppings.AddRange(oridinal.BasePizza.DefaultToppings);
        }

        // オプショントッピングを追加
        if (oridinal.OptionTopings != null)
        {
            allToppings.AddRange(oridinal.OptionTopings);
        }

        // Distinct() でトッピングIDに基づいた重複が排除される（基本画面側で排他になる想定だが）
        return allToppings.Distinct().ToArray();
    }
    private static Pizza[] GetSimilerPizzas(Topping[] allToppings)
    {
        // トッピングのリストに含まれるデフォルトトッピングを持つベースピザを抽出
        BasePizza[] bases = QueryBasePizza(allToppings);
        // ベースピザのデフォルトトッピングとallToppingと比較して不足しているトッピングを追加
        return SetSameTopping(bases, allToppings);
    }

    private static BasePizza[] QueryBasePizza(Topping[] allToppings)
    {
        // ベースピザの中から、allToppings がそのベースピザのデフォルトトッピングを
        // すべて含んでいるようなベースピザを取得します。
        var candidateBases = new List<BasePizza>();
        var allToppingsSet = new HashSet<Topping>(allToppings);

        foreach (BasePizza basePizza in BasePizza.GetAll())
        {
            if (basePizza.DefaultToppings.All(defaultTopping => allToppingsSet.Contains(defaultTopping)))
            {
                // このベースピザの全てのデフォルトトッピングが、元のピザの全トッピングリストに含まれている
                candidateBases.Add(basePizza);
            }
        }
        return candidateBases.ToArray();
    }

    private static Pizza[] SetSameTopping(BasePizza[] bases, Topping[] neededToppings)
    {
        // ベースピザに、neededToppings と同じ全体トッピング構成になるように
        // オプショントッピングを追加したPizzaオブジェクトのリストを返却
        var similarPizzas = new List<Pizza>();
        var neededToppingsSet = new HashSet<Topping>(neededToppings);

        foreach (BasePizza basePizza in bases)
        {
            // このベースピザを選んだ場合のオプショントッピングを決定
            // neededToppingsSet から、この basePizza の DefaultToppings を除いたもの
            var optionToppingsForThisBase = new List<Topping>();
            var baseDefaultToppingsSet = new HashSet<Topping>(basePizza.DefaultToppings);

            foreach (Topping neededTopping in neededToppingsSet)
            {
                // neededTopping がこの basePizza のデフォルトトッピングに含まれていなければ、
                // オプションとして追加する必要がある。
                if (!baseDefaultToppingsSet.Contains(neededTopping))
                {
                    optionToppingsForThisBase.Add(neededTopping);
                }
            }

            // Pizzaクラスのコンストラクタで価格は計算される
            similarPizzas.Add(new Pizza(basePizza, optionToppingsForThisBase.ToArray()));
        }
        return similarPizzas.ToArray();
    }

    private static Pizza? SelectCheapestPizza(Pizza[] similarPizzas)
    {
        // TotalAmount で昇順にソートし、最初の要素を取得
        return similarPizzas.OrderBy(p => p.TotalAmount).FirstOrDefault();
    }
}