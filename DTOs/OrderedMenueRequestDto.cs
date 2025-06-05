namespace ContosoPizza.DTOs
{
    public record OrderedMenueRequestDto
    {
        public enum OrderStatus {
            firstTime, recommended, original
        }
        public int Id { get; init; }
        public required PizzaRequestDto CustomedPiza { get; init; }
        public OrderStatus orderStatus { get; init; } 
    }
}