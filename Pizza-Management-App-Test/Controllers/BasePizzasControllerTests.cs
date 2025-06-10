using ContosoPizza.Interfaces;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContosoPizza.Tests
{
    [TestClass]
    public class BasePizzasControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;
        private Mock<IBasePizzaRepository> _mockBasePizzaRepo = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockBasePizzaRepo = new Mock<IBasePizzaRepository>();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        // DIコンテナのIBasePizzaRepositoryをテスト用のモックに差し替える
                        services.AddScoped<IBasePizzaRepository>(sp => _mockBasePizzaRepo.Object);
                    });
                });

            _client = _factory.CreateClient();
        }

        [TestMethod]
        public async Task GetBasePizzas_ReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            _mockBasePizzaRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<BasePizza>());

            // Act
            var response = await _client.GetAsync("/api/basepizzas");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }

        [TestMethod]
        public async Task GetBasePizzas_ReturnsListOfBasePizzas()
        {
            // Arrange
            // BasePizzaが依存するToppingのモックデータを作成
            var mockTomato = new Topping(new ToppingId(6), "トマト", 250);
            var mockCheese = new Topping(new ToppingId(0), "チーズ", 100);
            var mockSeafoodMix = new Topping(new ToppingId(3), "シーフードミックス", 500);

            // 返却するBasePizzaのモックデータを作成
            var mockPizzas = new List<BasePizza>
            {
                new BasePizza(new BasePizzaId(0), "プレーン", 1200, [mockTomato, mockCheese]),
                new BasePizza(new BasePizzaId(2), "シーフード", 1400, [mockCheese, mockSeafoodMix])
            };

            _mockBasePizzaRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(mockPizzas);

            // Act
            var response = await _client.GetAsync("/api/basepizzas");

            // Assert
            response.EnsureSuccessStatusCode();
            var returnedPizzas = await response.Content.ReadFromJsonAsync<List<BasePizza>>();

            Assert.IsNotNull(returnedPizzas);
            Assert.AreEqual(2, returnedPizzas.Count);
            // 1件目のピザの情報を検証
            Assert.AreEqual("プレーン", returnedPizzas[0].Name);
            Assert.AreEqual(2, returnedPizzas[0].DefaultToppings.Count);
            // 2件目のピザの情報を検証
            Assert.AreEqual(1400, returnedPizzas[1].BasePrice);
        }
    }
}