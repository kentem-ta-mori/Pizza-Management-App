using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContosoPizza.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.Net;
using System.Net.Http.Json;
using ContosoPizza.Interfaces;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ContosoPizza.Controllers.Tests
{
    [TestClass()]
    public class ToppingsControllerTests
    {
        private WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;
        private Mock<IToppingRepository> _mockToppingRepo = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockToppingRepo = new Mock<IToppingRepository>();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddScoped<IToppingRepository>(sp => _mockToppingRepo.Object);
                    });
                });

            _client = _factory.CreateClient();
        }

        [TestMethod]
        public async Task GetToppings_ReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            // モックリポジトリが空のリストを返すように設定
            _mockToppingRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Topping>());

            // Act
            var response = await _client.GetAsync("/api/toppings");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            // Content-Typeが期待通りであることを確認
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        }

        [TestMethod]
        public async Task GetToppings_ReturnsListOfToppings()
        {
            // Arrange (準備)
            var mockToppings = new List<Topping>
            {
                new Topping(new ToppingId(0), "チーズ", 100),
                new Topping(new ToppingId(1), "フライドガーリック", 150)
            };

            // モックリポジトリが特定のデータを返すように設定
            _mockToppingRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(mockToppings);

            // Act
            var response = await _client.GetAsync("api/toppings");

            // Assert
            response.EnsureSuccessStatusCode();

            // レスポンスボディをデシリアライズして検証
            var returnedToppings = await response.Content.ReadFromJsonAsync<List<Topping>>();

            Assert.IsNotNull(returnedToppings);
            Assert.AreEqual(2, returnedToppings.Count);
            Assert.AreEqual("チーズ", returnedToppings[0].Name);
            Assert.AreEqual(150, returnedToppings[1].Price);
        }
    }
}