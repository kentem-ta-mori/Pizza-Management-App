using ContosoPizza.DTOs;
using ContosoPizza.Interfaces;
using ContosoPizza.Models;

namespace ContosoPizza.Services
{
    public class PizzaService : IPizzaService
    {
        private readonly IToppingRepository _toppingRepository;
        private readonly IBasePizzaRepository _basePizzaRepository;
        private readonly IOrderedMenuRepository _orderedMenuRepository;
        private readonly IPizzaSuggester _pizzaSuggester;

        public PizzaService(
            IToppingRepository toppingRepository,
            IBasePizzaRepository basePizzaRepository,
            IOrderedMenuRepository orderedMenuRepository,
            IPizzaSuggester pizzaSuggester)
        {
            _toppingRepository = toppingRepository;
            _basePizzaRepository = basePizzaRepository;
            _orderedMenuRepository = orderedMenuRepository;
            _pizzaSuggester = pizzaSuggester;
        }

        // DTOをドメインモデルに変換する非同期メソッド
        public async Task<OrderedMenu> ConvertToDomainModelAsync(OrderedMenueRequestDto orderRequestDto, int? existingId = null)
        {
            // リポジトリからBasePizzaを取得
            var basePizzaId = new BasePizzaId(orderRequestDto.CustomedPiza.BasePizzaId);
            BasePizza? selectedBasePizza = await _basePizzaRepository.GetByIdAsync(basePizzaId);
            if (selectedBasePizza == null)
            {
                throw new ArgumentException($"Invalid BasePizzaId: {orderRequestDto.CustomedPiza.BasePizzaId}");
            }

            // リポジトリからToppingを取得
            var selectedOptionToppings = new List<Topping>();
            if (orderRequestDto.CustomedPiza.OptionToppingIds != null)
            {
                foreach (int toppingIdValue in orderRequestDto.CustomedPiza.OptionToppingIds)
                {
                    var toppingId = new ToppingId(toppingIdValue);
                    Topping? selectedTopping = await _toppingRepository.GetByIdAsync(toppingId);
                    if (selectedTopping == null)
                    {
                        throw new ArgumentException($"Invalid ToppingId: {toppingIdValue}");
                    }
                    selectedOptionToppings.Add(selectedTopping);
                }
            }

            var pizzaDomainModel = new Pizza(selectedBasePizza, selectedOptionToppings.ToArray());
            var orderedMenueDomainModel = new OrderedMenu
            {
                Id = existingId ?? 0,
                CustomedPiza = pizzaDomainModel
            };

            return orderedMenueDomainModel;
        }

        public async Task<OrderProcessingResult> HandleOrderAsync(OrderedMenu orderedMenu, OrderedMenueRequestDto.OrderStatus orderStatus)
        {
            switch (orderStatus)
            {
                case OrderedMenueRequestDto.OrderStatus.firstTime:
                    bool hasCheaperAlternative = await _pizzaSuggester.CheaperAlternativeAvailableAsync(orderedMenu.CustomedPiza);
                    if (hasCheaperAlternative)
                    {
                        Pizza? cheaperPizza = await _pizzaSuggester.GetCheaperAlternativeAsync(orderedMenu.CustomedPiza);
                        return new OrderProcessingResult
                        {
                            ProcessedOrder = orderedMenu,
                            SuggestsAlternative = true,
                            SuggestedPizza = cheaperPizza,
                        };
                    }
                    else
                    {
                        if (orderedMenu.Id == 0)
                        {
                            await _orderedMenuRepository.AddAsync(orderedMenu);
                        } else
                        {
                            await _orderedMenuRepository.UpdateAsync(orderedMenu);
                        }
                        return new OrderProcessingResult { ProcessedOrder = orderedMenu };
                    }

                case OrderedMenueRequestDto.OrderStatus.recommended:
                    Pizza? recommendedPizza = await _pizzaSuggester.GetCheaperAlternativeAsync(orderedMenu.CustomedPiza);
                    if (recommendedPizza == null)
                    {
                        return new OrderProcessingResult { ErrorMessage = "推奨されるピザ構成が見つかりませんでした。" };
                    }
                    var recommendedMenu = new OrderedMenu { Id = orderedMenu.Id, CustomedPiza = recommendedPizza };
                    if (recommendedMenu.Id == 0)
                    {
                        await _orderedMenuRepository.AddAsync(recommendedMenu);
                    } else
                    {
                        await _orderedMenuRepository.UpdateAsync(recommendedMenu);
                    }
                        
                    return new OrderProcessingResult { ProcessedOrder = recommendedMenu };

                case OrderedMenueRequestDto.OrderStatus.original:
                    if (orderedMenu.Id == 0)
                    {
                        await _orderedMenuRepository.AddAsync(orderedMenu);
                    } else
                    {
                        await _orderedMenuRepository.UpdateAsync(orderedMenu);
                    }
                        return new OrderProcessingResult { ProcessedOrder = orderedMenu };

                default:
                    return new OrderProcessingResult { ErrorMessage = "不明な注文ステータスです。" };
            }
        }

        #region CRUDラッパー
        public Task<IEnumerable<OrderedMenu>> GetAllOrdersAsync() => _orderedMenuRepository.GetAllAsync();
        public Task<OrderedMenu?> GetOrderByIdAsync(int id) => _orderedMenuRepository.GetByIdAsync(id);
        public Task AddOrderAsync(OrderedMenu order) => _orderedMenuRepository.AddAsync(order);
        public Task UpdateOrderAsync(OrderedMenu order) => _orderedMenuRepository.UpdateAsync(order);
        public Task DeleteOrderAsync(int id) => _orderedMenuRepository.DeleteAsync(id);
        #endregion
    }
}