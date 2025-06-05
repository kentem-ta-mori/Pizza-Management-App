namespace ContosoPizza.DTOs
{
    public class OrderedMenueRequestDto
    {
        public enum OrderStatus {
            firstTime, recommended, original
        }
        public int Id { get; set; }
        public required PizzaRequestDto CustomedPiza { get; set; }
        public OrderStatus orderStatus { get; set; } 
    }
}