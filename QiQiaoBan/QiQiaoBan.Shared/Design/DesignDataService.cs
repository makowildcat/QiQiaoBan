using QiQiaoBan.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QiQiaoBan.Design
{
    /// <summary>
    /// This class is intended to show something in Blend or in VS 
    /// It makes designing easier without starting app
    /// </summary>
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
