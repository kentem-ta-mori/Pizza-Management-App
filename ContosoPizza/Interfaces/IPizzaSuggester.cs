using ContosoPizza.Models;
using System.Threading.Tasks;

namespace ContosoPizza.Interfaces
{
    public interface IPizzaSuggester
    {
        /// <summary>
        /// 指定されたピザ構成より安価な代替構成が存在するかどうかを判断します。
        /// </summary>
        /// <param name="original">元のピザ構成。</param>
        /// <returns>安価な代替案があればtrue、なければfalse。</returns>
        Task<bool> CheaperAlternativeAvailableAsync(Pizza original);

        /// <summary>
        /// 指定されたピザ構成と同じトッピングを持つ、最も安価なピザ構成を取得します。
        /// </summary>
        /// <param name="original">元のピザ構成。</param>
        /// <returns>最も安価なピザ構成。見つからない場合はnullを返すことも考慮。</returns>
        Task<Pizza?> GetCheaperAlternativeAsync(Pizza original);
    }
}