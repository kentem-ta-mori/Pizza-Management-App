namespace ContosoPizza.DTOs
{
    using System.Collections.Generic;

    public class PizzaRequestDto
    {
        public int BasePizzaId { get; set; }
        public List<int>? OptionToppingIds { get; set; } 
    }
}
