using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetBase.Businnes.Models.Sorting
{
    public class SortingParams
    {
        public string SortDirection { get; set; } = "asc";
        public string OrderBy { get; set; } = "Id";
    }
}
