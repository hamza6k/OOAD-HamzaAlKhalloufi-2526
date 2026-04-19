using System.Collections.Generic;

namespace EscapeGame.Classes
{
    class Room
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Item> Items { get; set; } = new List<Item>();
    }
}