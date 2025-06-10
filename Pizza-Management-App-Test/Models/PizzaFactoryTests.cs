using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContosoPizza.Models;
using ContosoPizza.Repositories.Mocks; // 既存のモックリポジトリの名前空間を使用
using System.Linq;
using System.Threading.Tasks;
using ContosoPizza.Factories;

namespace ContosoPizza.Tests
{
    [TestClass]
    public class PizzaFactoryTests
    {
        [TestMethod]
        public async Task CreateCheaperAlternativeAsync_WhenCheaperAlternativeExists_ReturnsCheaperPizza()
        {
            // Arrange
            var mockRepo = new MockBasePizzaRepository();

            var pizzaFactory = new PizzaFactory(mockRepo);

            // 「プレーン」に「モッツァレラ」と「バジル」を追加
            var plainPizzaBase = (await mockRepo.GetByIdAsync(new BasePizzaId(0)))!;
            var mozzarella = new Topping(new ToppingId(2), "モッツァレラチーズ", 300); ;
            var basil = new Topping(new ToppingId(5), "バジル", 100);

            var originalPizza = new Pizza(plainPizzaBase, new[] { mozzarella, basil });

            // 元のピザの情報を検証
            Assert.AreEqual(1600, originalPizza.TotalAmount, "元のピザの合計金額が前提と異なります。");

            // Act
            var resultPizza = await pizzaFactory.CreateCheaperAlternativeAsync(originalPizza);

            // Assert
            Assert.IsNotNull(resultPizza, "より安い代替案として「マルゲリータ」が見つかるはずです。");
            Assert.AreEqual(1, resultPizza.BasePizza.Id.Value, "ベースピザは「マルゲリータ」(ID:1)に変わっているはずです。");
            Assert.AreEqual(1500, resultPizza.TotalAmount, "合計金額は1500円になっているはずです。");
            Assert.IsTrue(resultPizza.TotalAmount < originalPizza.TotalAmount, "結果のピザは元のピザより安くなっているはずです。");
            Assert.AreEqual(0, resultPizza.OptionTopings.Count, "「マルゲリータ」は全てのトッピングをデフォルトで含むため、追加トッピングは0のはずです。");
        }

        [TestMethod]
        public async Task CreateCheaperAlternativeAsync_WhenAlreadyCheapest_ReturnsNull()
        {
            // Arrange
            var mockRepo = new MockBasePizzaRepository();
            var pizzaFactory = new PizzaFactory(mockRepo);

            // 元のピザ: 「マルゲリータ」この構成ではこれが最安のはず。
            var margheritaPizzaBase = (await mockRepo.GetByIdAsync(new BasePizzaId(1)))!;
            var originalPizza = new Pizza(margheritaPizzaBase, null);

            // 元のピザの情報を検証
            Assert.AreEqual(1500, originalPizza.TotalAmount);

            // Act
            var resultPizza = await pizzaFactory.CreateCheaperAlternativeAsync(originalPizza);

            // Assert
            Assert.IsNull(resultPizza, "元のピザが既に最安のため、結果はnullであるべきです。");
        }
    }
}