using QiQiaoBan.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QiQiaoBan.Design
{
    public class DesignDataService : IDataService
    {
        public async Task<IList<Puzzle>> GetPuzzlesLocal()
        {
            var result = new List<Puzzle>();

            result.Add(new Puzzle() { Name = "Coucou" });
            result.Add(new Puzzle() { Name = "Hello" });
            result.Add(new Puzzle() { Name = "Ni Hao" });

            return result;
        }
    }
}
