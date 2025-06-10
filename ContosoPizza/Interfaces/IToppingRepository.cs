using ContosoPizza.Models;

namespace ContosoPizza.Interfaces
{
    public interface IToppingRepository
    {
        Task<IEnumerable<Topping>> GetAllAsync();
        Task<Topping?> GetByIdAsync(ToppingId id);
    }
}