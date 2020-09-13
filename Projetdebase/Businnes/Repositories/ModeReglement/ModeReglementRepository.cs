using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjetBase.Businnes.Contexts;
using InoAuthentification.Entities;

namespace ProjetBase.Businnes.Repositories.ModeReglement
{
    public class ModeReglementRepository : EntityFrameworkRepository<Entities.ModeReglement, int>, IModeReglementRepository
    {
        public ModeReglementRepository(ProjetBaseContext dbContext) : base(dbContext)
        { }
    }
}







