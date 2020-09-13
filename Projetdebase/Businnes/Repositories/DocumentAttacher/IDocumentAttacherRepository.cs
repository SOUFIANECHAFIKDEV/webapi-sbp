using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Repositories.DocumentAttacher
{
    interface IDocumentAttacherRepository : IRepository<Entities.DocumentAttacher, int>
    {
        Task<bool> Create(Entities.DocumentAttacher documentAttacher);
        Task<bool> Delete(Entities.DocumentAttacher documentAttacher);
        Task<bool> Update(Entities.DocumentAttacher documentAttacher);

        
    }
}
