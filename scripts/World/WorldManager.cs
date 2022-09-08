using static MC.GlobalVars;
using Godot;

namespace MC.World
{
    public class WorldManager : Spatial
    {
        [Export()]
        public Material BlocksMaterial;
        
        private static WorldManager _instance;
        
        public override void _Ready()
        {
            _instance = this;

            GameWorld.Initialize(GetNode<Player>("/root/Level/Player"));
        }

        public override void _Process(float delta)
        {
            GameWorld.Update();
        }
    }
}