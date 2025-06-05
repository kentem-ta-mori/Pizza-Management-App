// (例えば DTOs フォルダなどに作成)
using ContosoPizza.Models;

public record OrderProcessingResult
{
    public OrderedMenue? ProcessedOrder { get; init; }
    public bool SuggestsAlternative { get; init; }
    public Pizza? SuggestedPizza { get; init; }
    public string? ErrorMessage { get; init; }
}