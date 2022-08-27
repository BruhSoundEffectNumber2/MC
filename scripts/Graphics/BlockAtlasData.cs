using Godot;
using GDictionary = Godot.Collections.Dictionary;

namespace MC.Graphics
{
    public class BlockAtlasData
    {
        [Export()]
        public GDictionary Blocks;
        
        public BlockAtlasData(GDictionary blocks = null)
        {
            Blocks = blocks ?? new GDictionary();
        }
    }
}