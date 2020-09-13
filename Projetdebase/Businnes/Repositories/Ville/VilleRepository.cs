using ProjetBase.Businnes.Contexts;

namespace ProjetBase.Businnes.Repositories.Ville
{
    public class VilleRepository : EntityFrameworkRepository<Entities.Ville, int>, IVilleRepository
    {

        public VilleRepository(ProjetBaseContext dbContext) : base(dbContext)
        { }
    }
}
