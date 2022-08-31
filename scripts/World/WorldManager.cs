using static MC.GlobalVars;
using Godot;

using GArray = Godot.Collections.Array;
using GPos = Godot.Vector3;
using BPos = Godot.Vector3;
using CPos = Godot.Vector2;

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
            VisualServer.SetDebugGenerateWireframes(true);

            GameWorld.Initialize();
        }

        public override void _Process(float delta)
        {
            if (Input.IsActionJustPressed("ui_accept"))
            {
                // Cycle through all the debug drawing modes
                if ((int) GetViewport().DebugDraw == 3)
                {
                    GetViewport().DebugDraw = 0;
                }
                else
                {
                    GetViewport().DebugDraw++;
                }
            }
        }

        public static void AddChunk(CPos position, GArray surface)
        {
            var mesh = new ArrayMesh();
            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surface);
            mesh.SurfaceSetMaterial(0, _instance.BlocksMaterial);

            var node = new MeshInstance();
            node.Mesh = mesh;
            node.Translation = ChunkTranslation(position);
            
            _instance.AddChild(node);
        }
    }
}