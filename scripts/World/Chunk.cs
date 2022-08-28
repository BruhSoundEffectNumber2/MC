using static MC.GlobalVars;
using Godot;
using MC.World.Terrain;

using GPos = Godot.Vector3;
using BPos = Godot.Vector3;
using CPos = Godot.Vector2;

namespace MC.World
{
    /// <summary>
    /// Represents a 1 cubic meter voxel of the world.
    /// Always stored as a part of a chunk.
    /// </summary>
    public struct Block
    {
        public bool Solid;
        public int Type;
        
        public Block(bool solid, int type)
        {
            Solid = solid;
            Type = type;
        }
    }
    
    /// <summary>
    /// 16x256x16 group of blocks in the world.
    /// </summary>
    public class Chunk
    {
        public readonly CPos Position;
        
        private readonly Block[,,] _blocks;
        
        public Chunk(CPos position)
        {
            Position = position;
            _blocks = new Block[16, 256, 16];

            for (int y = 0; y < ChunkHeight; y++)
            {
                for (int x = 0; x < ChunkSize; x++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        BPos pos = new BPos(x, y, z);
                        this[pos] = new Block(TerrainGenerator.BlockSolid(position, pos), 1);
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets/sets the block at the given position.
        /// </summary>
        public Block this[BPos position]
        {
            get => _blocks[(int) position.x, (int) position.y, (int) position.z];
            set => _blocks[(int) position.x, (int) position.y, (int) position.z] = value;
        }
        
        /// <summary>
        /// Gets/sets the block at the given position.
        /// </summary>
        public Block this[int x, int y, int z]
        {
            get => _blocks[x, y, z];
            set => _blocks[x, y, z] = value;
        }
    }
}