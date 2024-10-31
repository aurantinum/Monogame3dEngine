using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.GameEngine.AI
{
    public class SimpleEnemy
    {
        AStarSearch Search;
        public SimpleEnemy(int searchRadius) {
            Search = new AStarSearch(searchRadius, searchRadius);
        }
    }
}
