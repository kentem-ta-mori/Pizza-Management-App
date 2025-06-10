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
    public BasePizza BasePizza { get; init; }

    public IReadOnlyList<Topping>? OptionTopings { get; init; }

    public int TotalAmount { get; init; }

    public IReadOnlyList<Topping> AllToppings
    {
        get
        {
            var allToppings = new List<Topping>();
            if (BasePizza?.DefaultToppings != null)
            {
                allToppings.AddRange(BasePizza.DefaultToppings);
            }
            if (OptionTopings != null)
            {
                allToppings.AddRange(OptionTopings);
            }
            // 重複を除外して返す
            return allToppings.Distinct().ToList().AsReadOnly();
        }
    }

    public Pizza() { }

    public Pizza(BasePizza basePizza, Topping[]? optionTopings)
    {
        this.BasePizza = basePizza;
        this.OptionTopings = optionTopings;

        // 合計金額の計算ロジックはそのまま
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

    /// <summary>
    /// 指定されたベースピザと目標のトッピングリストから、代替となるPizzaインスタンスを生成します。
    /// </summary>
    /// <param name="basePizza">新しいベースピザ</param>
    /// <param name="neededToppings">最終的に含めたい全てのトッピングのリスト</param>
    /// <returns>新しいPizzaインスタンス</returns>
    public static Pizza CreateAlternative(BasePizza basePizza, IEnumerable<Topping> neededToppings)
    {
        var neededToppingsSet = new HashSet<Topping>(neededToppings);

        // 新しいベースピザのデフォルトトッピングを差し引いて、必要なオプショントッピングを決定
        var optionToppingsForThisBase = neededToppingsSet.Except(basePizza.DefaultToppings).ToArray();

        return new Pizza(basePizza, optionToppingsForThisBase);
    }
}

