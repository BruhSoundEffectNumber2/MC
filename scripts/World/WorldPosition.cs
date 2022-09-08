using System;
using static MC.GlobalVars;
using Godot;

namespace MC.World
{
    public readonly struct WorldPosition
    {
        public static WorldPosition Zero => new WorldPosition(0, 0, 0, 0, 0, true);

        /// <summary>
        /// Is this position referring to a block?
        /// Values will be rounded to the nearest block if so,
        /// and some extra operations can be performed on block positions.
        /// </summary>
        public bool IsBlock { get; }

        private readonly int _cx, _cz;
        private readonly float _y;
        private readonly float _bx, _bz;

        /// <summary>
        /// Creates a new world position vector from the given coordinates.
        /// </summary>
        /// <param name="cx">The chunk position on the X axis.</param>
        /// <param name="bx">The position within the chunk on the X axis.</param>
        /// <param name="y">The height within the chunk.</param>
        /// <param name="cz">The chunk position on the Z axis.</param>
        /// <param name="bz">The position within the chunk on the Z axis.</param>
        /// <param name="isBlock">Should be true if the position is used to
        /// represent the position of a block. It unlocks some additional functionality.</param>
        public WorldPosition(int cx, float bx, float y, int cz, float bz, bool isBlock)
        {
            // TODO: Do we want a separate struct/constructor for block positions?
            // Our block position has to always be within the chunk.
            if (bx < 0 || bx >= ChunkSize || bz < 0 || bz >= ChunkSize)
            {
                throw new ArgumentException("Block position must be within the chunk.");
            }

            IsBlock = isBlock;
            _cx = cx;
            _cz = cz;
            
            if (IsBlock)
            {
                // These should all be integers anyway, but we round them just in case.
                _y = (int)y;
                _bx = (int)bx;
                _bz = (int)bz;
            }
            else
            {
                _y = y;
                _bx = bx;
                _bz = bz;
            }
        }

        /// <summary>
        /// Creates a new world position vector, only specifying the chunk position.
        /// This position will have block positions of 0, and will have the IsBlock flag set to false.
        /// </summary>
        /// <param name="cx">The chunk position on the X axis.</param>
        /// <param name="cz">The chunk position on the Z axis.</param>
        public WorldPosition(int cx, int cz)
        {
            IsBlock = false;
            _cx = cx;
            _cz = cz;

            _bx = _y = _bz = 0;
        }

        /// <summary>
        /// Returns the chunk that this world position is in.
        /// </summary>
        /// <param name="center">If set, the returned value will be
        /// the difference between the center and the actual world position.</param>
        public Vector3 ChunkPosition(WorldPosition center)
        {
            // To prevent an infinite loop, check if the center is zero.
            if (center == Zero)
            {
                return new Vector3(_cx, 0, _cz);
            }
            
            return new Vector3(_cx, 0, _cz) - center.ChunkPosition(Zero);
        }

        /// <summary>
        /// Returns the position in the chunk as a Godot Vector3.
        /// </summary>
        public Vector3 BlockPositionVector()
        {
            return new Vector3(_bx, _y, _bz);
        }

        /// <summary>
        /// Returns the position in the chunk as a WorldPosition.
        /// With the chunk position set to zero, and all other values staying the same.
        /// </summary>
        public WorldPosition BlockPosition()
        {
            return new WorldPosition(0, _bx, _y, 0, _bz, IsBlock);
        }
        
        /// <summary>
        /// Returns the translation of the block in Godot's coordinate system.
        /// Translation being the combined chunk and block positions.
        /// </summary>
        /// <param name="center">If set, the returned value will be
        /// the difference between the center and the actual world position.</param>
        public Vector3 Translation(WorldPosition center)
        {
            // We need to multiply the chunk position by the chunk size to get the real position.
            var chunkPos = ChunkPosition(center) * ChunkSize;
            var blockPos = BlockPositionVector();
            var combinedPos = chunkPos + blockPos;
            
            // Godot's coordinate system is different from the worlds,
            // so we have to invert the X and Z axes.
            return new Vector3(-combinedPos.x, combinedPos.y, -combinedPos.z);
        }

        /// <summary>
        /// Gets the position of the block that is adjacent to this block.
        /// </summary>
        /// <param name="direction">Which block to get.</param>
        /// <param name="result">The resulting position.</param>
        /// <returns>Is the new position valid and within bounds?</returns>
        public bool AdjacentBlock(WorldDirection direction, out WorldPosition result)
        {
            // TODO: If we add math operations to WorldPosition, we can simplify this.
            
            // We can't get an adjacent block if we're not a block position.
            if (!IsBlock)
            {
                throw new InvalidOperationException("Cannot get an adjacent block from a non-block position.");
            }
            
            float bx = _bx, y = _y, bz = _bz;

            switch (direction)
            {
                case WorldDirection.Forward:
                    bz++;
                    break;
                case WorldDirection.Backward:
                    bz--;
                    break;
                case WorldDirection.Left:
                    bx++;
                    break;
                case WorldDirection.Right:
                    bx--;
                    break;
                case WorldDirection.Up:
                    y++;
                    break;
                case WorldDirection.Down:
                    y--;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
            
            int cx = _cx, cz = _cz;
            
            // The block position has to be within the chunk, so if it's not,
            // then we have to adjust the chunk position.
            if (bx < 0)
            {
                cx--;
                bx += ChunkSize;
            }
            else if (bx >= ChunkSize)
            {
                cx++;
                bx -= ChunkSize;
            }
            
            if (bz < 0)
            {
                cz--;
                bz += ChunkSize;
            }
            else if (bz >= ChunkSize)
            {
                cz++;
                bz -= ChunkSize;
            }
            
            result = new WorldPosition(cx, bx, y, cz, bz, true);
            
            return y < ChunkHeight && y >= 0;
        }

        #region Operators

        public override bool Equals(object obj) => obj is WorldPosition other && this == other;

        public override int GetHashCode() => (_cx, _bx, _y, _cz, _bz).GetHashCode();

        public static bool operator ==(WorldPosition a, WorldPosition b)
        {
            var cx = a._cx == b._cx;
            var bx = Math.Abs(a._bx - b._bx) < float.Epsilon;
            
            var y = Math.Abs(a._y - b._y) < float.Epsilon;
            
            var cz = a._cz == b._cz;
            var bz = Math.Abs(a._bz - b._bz) < float.Epsilon;
            
            return cx && bx && y && cz && bz;
        }

        public static bool operator !=(WorldPosition a, WorldPosition b) => !(a == b);

        // TODO: Do we want to have math operators?
        /*public static WorldPosition operator +(WorldPosition a) => a;

        public static WorldPosition operator + (WorldPosition a, WorldPosition b)
        {
            var cx = a._cx + b._cx;
            var cz = a._cz + b._cz;
            var y = a._y + b._y;
            var bx = a._bx + b._bx;
            var bz = a._bz + b._bz;
            
            // The block position has to be within the chunk, so if it's not,
            // then we have to adjust the chunk position.
            if (bx < 0)
            {
                cx--;
                bx += ChunkSize;
            }
            else if (bx >= ChunkSize)
            {
                cx++;
                bx -= ChunkSize;
            }
            
            if (bz < 0)
            {
                cz--;
                bz += ChunkSize;
            }
            else if (bz >= ChunkSize)
            {
                cz++;
                bz -= ChunkSize;
            }
            
            return new WorldPosition(cx, bx, y, cz, bz);
        }
        
        public static WorldPosition operator -(WorldPosition a)
        {
            // The chunk and y coordinates can be inverted directly.
            var cx = -a._cx;
            var cz = -a._cz;
            var y = -a._y;
            
            // Because the block position is always going in the positive direction,
            // we have to invert it by subtracting it from the max block position value
            // and decrementing the chunk position. If the block position is 0, don't change it.
            float bx;
            
        }

        public static WorldPosition operator -(WorldPosition a, WorldPosition b) => a + -b;*/

        #endregion
    }
}