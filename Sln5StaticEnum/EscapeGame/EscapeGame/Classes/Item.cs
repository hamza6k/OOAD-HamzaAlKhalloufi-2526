using System.Windows.Resources;

namespace EscapeGame.Classes
{
    class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsLocked { get; set; } = false;
        public bool IsPortable { get; set; } = true;
        public Item Key { get; set; }
        public Item HiddenItem { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}