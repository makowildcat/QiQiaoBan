using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QiQiaoBan.Model
{
    public interface IDataService
    {
        Task<IList<Puzzle>> GetPuzzlesLocal();
    }
}
