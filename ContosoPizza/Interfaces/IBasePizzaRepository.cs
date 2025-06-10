using ContosoPizza.Models;

namespace ContosoPizza.Interfaces
{
    public interface IBasePizzaRepository
    {
        Task<IEnumerable<BasePizza>> GetAllAsync();
        Task<BasePizza?> GetByIdAsync(BasePizzaId id);
    }
}