using Godot;
using static MC.GlobalVars;

using GPos = Godot.Vector3;
using BPos = Godot.Vector3;
using CPos = Godot.Vector2;

namespace MC.World.Terrain
{
    public static class TerrainGenerator
    {
        private static float _period = 20;
        private static int _seed = 0;
        private static int _octaves = 4;
        private static float _lacunarity = 0.5f;
        private static float _persistence = 0.8f;
        
        private static OpenSimplexNoise _noise;

        public static void Initialize()
        {
            _noise = new OpenSimplexNoise();
            _noise.Seed = _seed;
            _noise.Octaves = _octaves;
            _noise.Period = _period;
            _noise.Lacunarity = _lacunarity;
            _noise.Persistence = _persistence;
        }

        public static bool BlockSolid(CPos chunkPosition, BPos blockPosition)
        {
            var translation = ChunkTranslation(chunkPosition) + BlockTranslation(blockPosition);

            var noise = _noise.GetNoise2d(translation.x, translation.z);

            var height = SeaLevel + noise * 40;
            
            return blockPosition.y <= height;
        }

        public static int BlockType(ref Chunk chunk, BPos blockPosition)
        {
            // If the block above is solid, it is dirt. If not, it is grass
            
            BPos above = blockPosition + new BPos(0, 1, 0);
            if (above.y >= ChunkHeight)
                return 1;

            return chunk[above].Solid ? 0 : 1;
        }
    }
}