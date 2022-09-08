using static MC.GlobalVars;

namespace MC.World
{
    public static class GameWorld
    {
        private static WorldPosition _center;
        private static Chunk[,] _localChunks;
        private static Player _player;
        
        public static void Initialize(Player player)
        {
            _player = player;

            _center = WorldPosition.Zero;
            _localChunks = new Chunk[RenderDistance * 2, RenderDistance * 2];
            
            ChunkBuilder.QueueRequest(new WorldPosition(0, 0), _center);
            ChunkBuilder.SendRequests();
        }

        public static void Update()
        {
            ChunkBuilder.Update();
        }
    }
}