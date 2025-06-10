using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContosoPizza.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using ContosoPizza.Models;
using System.Net.Http.Json;
using System.Net;
using ContosoPizza.DTOs;
using System.Text.Json;
using ContosoPizza.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoPizza.Controllers.Tests
{
    [TestClass()]
    public class OrdersControllerTests
    {
        private static WebApplicationFactory<Program> _factory = null!;
        private static HttpClient _client = null!;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>()
                            .WithWebHostBuilder(builder =>
                            {
                                // テスト実行時だけDIコンテナの登録を上書きする
                                builder.ConfigureServices(services =>
                                {
                                    // Program.csで登録されているある既存の登録を探して削除
                                    var descriptor = services.SingleOrDefault(
                                        d => d.ServiceType == typeof(IOrderedMenuRepository));

                                    if (descriptor != null)
                                    {
                                        services.Remove(descriptor);
                                    }
                                    // MockOrderedMenuRepositoryを「Singleton」として登録
                                    // これにより、テスト実行中は常に同じインスタンスが使われる
                                    services.AddSingleton<IOrderedMenuRepository, MockOrderedMenuRepository>();
                                });
                            });
            _client = _factory.CreateClient();
        }
        [ClassCleanup]
        public static void ClassCleanup()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [TestMethod()]
        public async Task GetAllTest()
        {
            // Act
            var response = await _client.GetAsync("api/orders");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var orders = await response.Content.ReadFromJsonAsync<List<OrderedMenu>>();
            Assert.IsNotNull(orders);
        }

        [TestMethod()]
        public async Task GetbyIdTest()
        {
            // Arrange
            var newOrderDto = new OrderedMenueRequestDto
            {
                CustomedPiza = new PizzaRequestDto
                {
                    BasePizzaId = 1,
                    OptionToppingIds = new List<int>()
                },
                orderStatus = OrderedMenueRequestDto.OrderStatus.firstTime
            };
            var createResponse = await _client.PostAsJsonAsync("api/orders", newOrderDto);
            Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode, "テストデータの作成に失敗しました。");

            // 作成された注文のIDを取得
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderedMenu>(options);
            Assert.IsNotNull(createdOrder, "作成された注文をデシリアライズできませんでした。");
            var orderIdToGet = createdOrder.Id;

            // Act
            var response = await _client.GetAsync($"api/orders/{orderIdToGet}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var fetchedOrder = await response.Content.ReadFromJsonAsync<OrderedMenu>();
            Assert.IsNotNull(fetchedOrder);
            Assert.AreEqual(orderIdToGet, fetchedOrder.Id);
            Assert.AreEqual("マルゲリータ", fetchedOrder.CustomedPiza.BasePizza.Name);
        }

        [TestMethod()]
        public async Task CreateOrderTest()
        {
            // Arrange
            var newOrderDto = new OrderedMenueRequestDto
            {
                CustomedPiza = new PizzaRequestDto
                {
                    BasePizzaId = 0,
                    OptionToppingIds = new List<int>()
                },
                orderStatus = OrderedMenueRequestDto.OrderStatus.firstTime
            };
            // Act
            var response = await _client.PostAsJsonAsync("api/orders", newOrderDto);
            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNotNull(response.Headers.Location);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var createdOrder = await response.Content.ReadFromJsonAsync<OrderedMenu>(options);
            Assert.IsNotNull(createdOrder);
            Assert.IsTrue(createdOrder.Id > 0);
            Assert.AreEqual("プレーン", createdOrder.CustomedPiza.BasePizza.Name);
        }

        [TestMethod()]
        public async Task UpdateOrderTest()
        {
            // Arrange
            var newOrderDto = new OrderedMenueRequestDto
            {
                CustomedPiza = new PizzaRequestDto
                {
                    BasePizzaId = 2,
                    OptionToppingIds = new List<int>()
                },
                orderStatus = OrderedMenueRequestDto.OrderStatus.firstTime
            };
            var createResponse = await _client.PostAsJsonAsync("api/orders", newOrderDto);
            Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode, "テストデータの作成に失敗しました。");

            // 作成された注文のIDを取得
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderedMenu>(options);
            Assert.IsNotNull(createdOrder, "作成された注文をデシリアライズできませんでした。");
            var orderIdToGet = createdOrder.Id;

            var updateOrderDto = new OrderedMenueRequestDto
            {
                CustomedPiza = new PizzaRequestDto
                {
                    BasePizzaId = 0,
                    OptionToppingIds = new List<int>()
                },
                orderStatus = OrderedMenueRequestDto.OrderStatus.original
            };
            // Act
            var updateResponse = await _client.PutAsJsonAsync($"api/orders/{orderIdToGet}", updateOrderDto);
            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, updateResponse.StatusCode);
            // 更新した注文の確認
            var updatedOrderResponce = await _client.GetAsync($"api/orders/{orderIdToGet}");
            var updatedOrder = await updatedOrderResponce.Content.ReadFromJsonAsync<OrderedMenu>();
            Assert.IsNotNull(updatedOrder);
            Assert.AreEqual("プレーン", updatedOrder.CustomedPiza.BasePizza.Name);
        }

        [TestMethod()]
        public async Task DeleteTest()
        {
            // Arrange
            var newOrderDto = new OrderedMenueRequestDto
            {
                CustomedPiza = new PizzaRequestDto
                {
                    BasePizzaId = 2,
                    OptionToppingIds = new List<int>()
                },
                orderStatus = OrderedMenueRequestDto.OrderStatus.firstTime
            };
            var createResponse = await _client.PostAsJsonAsync("api/orders", newOrderDto);
            Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode, "テストデータの作成に失敗しました。");

            // 作成された注文のIDを取得
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderedMenu>(options);
            Assert.IsNotNull(createdOrder, "作成された注文をデシリアライズできませんでした。");
            var orderIdToGet = createdOrder.Id;

            // Act
            var updateResponse = await _client.DeleteAsync($"api/orders/{orderIdToGet}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, updateResponse.StatusCode);
            // 更新した注文の確認
            var deletedOrderResponce = await _client.GetAsync($"api/orders/{orderIdToGet}");
            Assert.AreEqual(HttpStatusCode.NotFound, deletedOrderResponce.StatusCode);
        }
    }
}