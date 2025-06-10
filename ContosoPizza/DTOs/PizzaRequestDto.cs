namespace ContosoPizza.DTOs
{
    using System.Collections.Generic;

    public record PizzaRequestDto
    {
        public int BasePizzaId { get; init; }
        public List<int>? OptionToppingIds { get; init; } 
    }
}
