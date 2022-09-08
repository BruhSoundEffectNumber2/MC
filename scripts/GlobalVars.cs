using System;
using Godot;

namespace MC
{
    public enum WorldDirection
    {
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down
    }

    public static class Extensions
    {
        public static Vector3 ToVector(this WorldDirection direction)
        {
            switch (direction)
            {
                // Godot's coordinate system has the X and Z axes inverted from ours.
                case WorldDirection.Forward:
                    return Vector3.Back;
                case WorldDirection.Backward:
                    return Vector3.Forward;
                case WorldDirection.Left:
                    return Vector3.Right;
                case WorldDirection.Right:
                    return Vector3.Left;
                case WorldDirection.Up:
                    return Vector3.Up;
                case WorldDirection.Down:
                    return Vector3.Down;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
    
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
        
        /// <summary>
        /// How many chunks in each direction are rendered. <br/>
        /// Must be greater than 0.
        /// </summary>
        public static uint RenderDistance { get; private set; } = 12;
        
        /// <summary>
        /// How many chunks in each direction are simulated.
        /// (i.e. things can happen in these chunks.) <br/>
        /// This must be less than or equal to <see cref="RenderDistance"/>.
        /// </summary>
        public static uint SimDistance { get; private set; } = 6;
        
        // TODO: Event system for when these change.
        // TODO: Setup a system for actual settings.

        public static void SetRenderDistance(uint distance)
        {
            if (distance == 0)
            {
                return;
            }

            RenderDistance = distance;
            
            // The SimDistance cannot be greater than the RenderDistance.
            if (SimDistance > RenderDistance)
            {
                SimDistance = RenderDistance;
            }
        }
    }
}