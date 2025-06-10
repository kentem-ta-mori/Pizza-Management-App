using ContosoPizza.DTOs;
using ContosoPizza.Models;

namespace ContosoPizza.Interfaces
{
    public interface IPizzaService
    {
        Task<IEnumerable<OrderedMenu>> GetAllOrdersAsync();
        Task<OrderedMenu?> GetOrderByIdAsync(int id);
        Task<OrderedMenu> ConvertToDomainModelAsync(OrderedMenueRequestDto orderRequestDto, int? existingId = null);
        Task<OrderProcessingResult> HandleOrderAsync(OrderedMenu orderedMenu, OrderedMenueRequestDto.OrderStatus orderStatus);
        Task AddOrderAsync(OrderedMenu order);
        Task UpdateOrderAsync(OrderedMenu order);
        Task DeleteOrderAsync(int id);
    }
}