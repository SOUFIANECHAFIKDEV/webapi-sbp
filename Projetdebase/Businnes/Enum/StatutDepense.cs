using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Enum
{
    public enum StatutDepense
    {
        Brouillon = 1,
        Encours = 2,
        Enretard = 3,
        Cloture = 4,
        Annule = 5
    }

    public enum categorieDepense
    {
        achat = 1,
        soustraitance = 2,
    }
}
