using System;

namespace MC.World
{
    /// <summary>
    /// Contains information about a voxel.
    /// </summary>
    public class VoxelType
    {
        /// <summary>
        /// The GUID of the voxel type.
        /// </summary>
        public uint Id;
        
        /// <summary>
        /// The display name of the voxel type.
        /// </summary>
        public string Name;
        
        /// <summary>
        /// Can we see through this voxel?
        /// </summary>
        public bool IsTransparent;

        public VoxelType(uint id, string name, bool isTransparent)
        {
            Id = id;
            Name = name;
            IsTransparent = isTransparent;
        }
    }

    /// <summary>
    /// Contains lighting, texturing, and other graphics related information about a voxel.
    /// </summary>
    public struct VoxelGraphics
    {
        /// <summary>
        /// A length 6 array of texture IDs for each face of the voxel. <br/>
        /// The value of each index is the index used for the texture array in the shader.
        /// </summary>
        public uint[] TextureIds;
        
        /// <summary>
        /// A length 6 array of light values for each face of the voxel. <br/>
        /// Each value is a uint between 0 and 15, where 0 is completely dark and 15 is completely bright.
        /// </summary>
        public uint[] LightLevels;
        
        /// <summary>
        /// A length 8 array, where each index represents a corner of the voxel. <br/>
        /// And the value determines if the corner should have ambient occlusion.
        /// </summary>
        public bool[] AmbientOcclusion;
        
        public VoxelGraphics(uint[] textureIds, uint[] lightLevels, bool[] ambientOcclusion)
        {
            // Make sure the arrays are the correct length.
            if (textureIds.Length != 6)
                throw new ArgumentException("TextureIds must be a length 6 array.");
            
            if (lightLevels.Length != 6)
                throw new ArgumentException("LightLevels must be a length 6 array.");
            
            if (ambientOcclusion.Length != 8)
                throw new ArgumentException("AmbientOcclusion must be a length 8 array.");
            
            TextureIds = textureIds;
            LightLevels = lightLevels;
            AmbientOcclusion = ambientOcclusion;
        }
    }
    
    /// <summary>
    /// A 1 cubic meter voxel.
    /// </summary>
    public struct Voxel
    {
        public WorldPosition Position;
        public VoxelType Type;
        public VoxelGraphics Graphics;
        
        public Voxel(WorldPosition position, VoxelType type, VoxelGraphics graphics)
        {
            Position = position;
            Type = type;
            Graphics = graphics;
        }
    }
}