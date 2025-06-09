namespace ContosoPizza.DTOs
{
    public record OrderedMenueRequestDto
    {
        public enum OrderStatus {
            firstTime, recommended, original
        }
        public required PizzaRequestDto CustomedPiza { get; init; }
        public OrderStatus orderStatus { get; init; } 
    }
}