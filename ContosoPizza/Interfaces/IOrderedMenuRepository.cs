using ContosoPizza.Models;

namespace ContosoPizza.Interfaces
{
    public interface IOrderedMenuRepository
    {
        Task<IEnumerable<OrderedMenu>> GetAllAsync();
        Task<OrderedMenu?> GetByIdAsync(int id);
        Task AddAsync(OrderedMenu order);
        Task UpdateAsync(OrderedMenu order);
        Task DeleteAsync(int id);
    }
}