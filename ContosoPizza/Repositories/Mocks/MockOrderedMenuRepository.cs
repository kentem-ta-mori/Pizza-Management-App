using ContosoPizza.Interfaces;
using ContosoPizza.Models;

public class MockOrderedMenuRepository : IOrderedMenuRepository
{
    private readonly List<OrderedMenu> _orderedMenus = new();
    private int _nextId = 1;

    public Task AddAsync(OrderedMenu order)
    {
        order.Id = _nextId++;
        _orderedMenus.Add(order);
        return Task.CompletedTask;
    }



    public Task<IEnumerable<OrderedMenu>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<OrderedMenu>>(_orderedMenus);
    }

    public Task<OrderedMenu?> GetByIdAsync(int id)
    {
        return Task.FromResult(_orderedMenus.FirstOrDefault(o => o.Id == id));
    }

    public Task UpdateAsync(OrderedMenu order)
    {
        var index = _orderedMenus.FindIndex(o => o.Id == order.Id);
        if (index != -1)
        {
            _orderedMenus[index] = order;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _orderedMenus.RemoveAll(o => o.Id == id);
        return Task.CompletedTask;
    }

}