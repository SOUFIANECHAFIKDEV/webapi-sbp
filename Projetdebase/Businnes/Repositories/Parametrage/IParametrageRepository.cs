using ProjetBase.Businnes.Entities;
using ProjetBase.Businnes.Enum;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.Parametrage
{
    public interface IParametrageRepository : IRepository<Parametrages, int>
    {
        Task<string> GenerateParameter(ProjetBase.Businnes.Models.Parametrage numerotationInfos);
        Parametrages GetParametrageDocument();
        Parametrages GetParametrageMessagerie();
        Task<bool> Increment(TypeNumerotation type);
        Task<Parametrages> IncrementReference(Parametrages parametrage, TypeNumerotation type);
    }
}
