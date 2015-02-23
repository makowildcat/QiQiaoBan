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

            result.Add(new Puzzle() { Name = "Coucou", BestTime = 17 });
            result.Add(new Puzzle() { Name = "Hello", BestTime = 84 });
            result.Add(new Puzzle() { Name = "Ni Hao", BestTime = 5 });
            result.Add(new Puzzle() { Name = "Hallo", BestTime = 148 });

            return result;
        }
    }
}
