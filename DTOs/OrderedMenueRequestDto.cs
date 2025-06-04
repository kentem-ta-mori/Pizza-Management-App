namespace ContosoPizza.DTOs
{
    public class OrderedMenueRequestDto
    {
        public enum OrderStatus {
            firstTime, recommended, original
        }
        public int Id { get; set; }
        public PizzaRequestDto CustomedPiza { get; set; }
        public OrderStatus orderStatus { get; set; } 
    }
}