using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Enum
{
    public enum ChantierChangeStatusResponse
    {
        has_bills_not_paid = 1,
        changed_successfully = 2,
        server_error = 3,
    }
}
