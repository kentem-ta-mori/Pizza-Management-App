// (例えば DTOs フォルダなどに作成)
using ContosoPizza.Models;

public class OrderProcessingResult
{
    public OrderedMenue? ProcessedOrder { get; set; }
    public bool SuggestsAlternative { get; set; }
    public Pizza? SuggestedPizza { get; set; }
    public string? ErrorMessage { get; set; }
}