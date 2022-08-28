using System;
using Godot;

using GPos = Godot.Vector3;
using BPos = Godot.Vector3;
using CPos = Godot.Vector2;

namespace MC
{
    public static class GlobalVars
    {
        /// <summary>
        /// The size of a chunk in the X and Z axis.
        /// </summary>
        public const int ChunkSize = 16;
        /// <summary>
        /// The height of a chunk (Y axis).
        /// </summary>
        public const int ChunkHeight = 256;
        public const int SeaLevel = 60;
        
        public static BPos BlockForward = new BPos(0, 0, 1);
        public static BPos BlockBackward = new BPos(0, 0, -1);
        public static BPos BlockLeft = new BPos(1, 0, 0);
        public static BPos BlockRight = new BPos(-1, 0, 0);
        public static BPos BlockUp = new BPos(0, 1, 0);
        public static BPos BlockDown = new BPos(0, -1, 0);

        public static BPos VectorFromDirection(int i)
        {
            switch (i)
            {
                case 0:
                    return BlockForward;
                case 1:
                    return BlockBackward;
                case 2:
                    return BlockLeft;
                case 3:
                    return BlockRight;
                case 4:
                    return BlockUp;
                case 5:
                    return BlockDown;
                default:
                    throw new Exception("Invalid direction");
            }
        }

        public static int DirectionFromName(string name)
        {
            switch (name)
            {
                case "forward":
                    return 0;
                case "backward":
                    return 1;
                case "left":
                    return 2;
                case "right":
                    return 3;
                case "top":
                    return 4;
                case "bottom":
                    return 5;
                default:
                    throw new Exception("Invalid direction");
            }
        }

        /// <summary>
        /// Returns the translation of a chunk in Godot's coordinate space.
        /// </summary>
        public static GPos ChunkTranslation(CPos position)
        {
            return new GPos(-position.x * ChunkSize, 0, -position.y * ChunkSize);
        }

        /// <summary>
        /// Returns of an offset of a block in Godot's coordinate space.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static GPos BlockTranslation(BPos position)
        {
            return new GPos(-position.x, position.y, -position.z);
        }
    }
}