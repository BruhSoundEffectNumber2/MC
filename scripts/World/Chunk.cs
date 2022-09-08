using static MC.GlobalVars;
using Godot;

namespace MC.World
{
    /// <summary>
    /// 16x256x16 group of blocks in the world.
    /// </summary>
    public class Chunk
    {
        public readonly WorldPosition Position;
        
        /// <summary>
        /// All voxels in the chunk, in the order of YXZ.
        /// </summary>
        private readonly Voxel[,,] _voxels;
        
        public Chunk(WorldPosition position)
        {
            Position = position;
            _voxels = new Voxel[ChunkHeight, ChunkSize, ChunkSize];
        }

        #region Operators

        public Voxel this[WorldPosition position]
        {
            get => this[position.BlockPositionVector()];
            set => this[position.BlockPositionVector()] = value;
        }

        public Voxel this[Vector3 position]
        {
            get => _voxels[(int) position.y, (int) position.x, (int) position.z];
            set => _voxels[(int) position.y, (int) position.x, (int) position.z] = value;
        }
        
        public Voxel this[int x, int y, int z]
        {
            get => _voxels[y, x, z];
            set => _voxels[y, x, z] = value;
        }

        #endregion
    }
}