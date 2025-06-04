namespace ContosoPizza.DTOs
{
    public class OrderedMenueRequestDto
    {
        public int Id { get; set; }
        public PizzaRequestDto CustomedPiza { get; set; }
    }
}