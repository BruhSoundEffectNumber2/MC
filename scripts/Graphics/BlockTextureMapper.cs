using static MC.GlobalVars;

using System;
using System.Collections.Generic;
using Godot;

namespace MC.Graphics
{
    public struct FaceUv
    {
        // UVs for each corner of the face, starting at the top left corner and moving clockwise
        public Vector2[] Corners;
        
        public FaceUv(Vector2[] corners)
        {
            Corners = corners;
        }
    }

    public struct BlockUv
    {
        public FaceUv[] Faces;

        /// <summary>
        /// Creates a new BlockUV, assigning each face a UV for each corner.
        /// </summary>
        /// <param name="faceTexCoords">
        /// A 6 long set of vectors that correspond to the textures
        /// place within the atlas.
        /// </param>
        public BlockUv(Vector2[] faceTexCoords)
        {
            Faces = new FaceUv[6];
            
            for (int i = 0; i < 6; i++)
            {
                var corners = new Vector2[4];

                for (int j = 0; j < 4; j++)
                {
                    corners[j] = BlockTextureMapper.AtlasFaceToUv(faceTexCoords[i], j);
                }
                
                Faces[i] = new FaceUv(corners);
            }
        }
    }
    
    public static class BlockTextureMapper
    {
        // TODO: We will want to have different atlas sizes in the future
        
        /// <summary>
        /// The number of pixels in each direction for a texture in the atlas. <br/>
        /// This includes the margin pixels, so the actual size is this - 2 * Margin.
        /// </summary>
        public const int GridSize = 20;

        /// <summary>
        /// The number of pixels in each direction for the entire atlas.
        /// </summary>
        public const int AtlasSize = 1280;

        public const int Margin = 2;

        private static List<BlockUv> _blocks;
        private static Dictionary<string, int> _blockIndices;

        public static void Initialize()
        {
            _blocks = new List<BlockUv>();
            _blockIndices = new Dictionary<string, int>();
            
            // Load the atlas data
            using (XMLParser parser = new XMLParser())
            {
                parser.Open("res://textures/atlas/atlas_blocks_data.xml");
                int id = 0;
                string name = "";
                Vector2[] faces = new Vector2[6];

                while (parser.Read() == Error.Ok)
                {
                    // Find a block tag
                    if (parser.GetNodeType() == XMLParser.NodeType.Element)
                    {
                        switch (parser.GetNodeName())
                        {
                            // A new block, reinitialize the data
                            case "block":
                                faces = new Vector2[6];
                                id = int.Parse(parser.GetAttributeValue(0));
                                break;
                            // The name of the block
                            case "name":
                                parser.Read();
                                name = parser.GetNodeData();
                                break;
                            // A face of the block
                            case "face":
                                var x = parser.GetAttributeValue(0);
                                var y = parser.GetAttributeValue(1);
                                
                                parser.Read();
                                var type = parser.GetNodeData();
                                
                                // We can have several different types of faces
                                switch (type)
                                {
                                    // All faces are the same
                                    case "all":
                                        faces[0] = new Vector2(float.Parse(x), float.Parse(y));
                                        faces[1] = faces[2] = faces[3] = faces[4] = faces[5] = faces[0];
                                        break;
                                    // The front, back, left, and right faces are the same
                                    case "side":
                                        faces[0] = new Vector2(float.Parse(x), float.Parse(y));
                                        faces[1] = faces[2] = faces[3] = faces[0];
                                        break;
                                    // Only the given face is set
                                    default:
                                        faces[DirectionFromName(type)] = new Vector2(float.Parse(x), float.Parse(y));
                                        break;
                                }
                                break;
                        }
                    }
                    
                    // Find the end of the block tag
                    if (parser.GetNodeType() == XMLParser.NodeType.ElementEnd)
                    {
                        if (parser.GetNodeName() == "block")
                        {
                            // Create the BlockUv
                            _blocks.Insert(id, new BlockUv(faces));
                            _blockIndices.Add(name, id);
                        }
                    }
                }
            }
        }

        public static Vector2 GetUvForFace(string blockName, int dir, int corner)
        {
            // Gets index of the block from the name, then looks at the given face and corner
            return _blocks[
                _blockIndices[blockName]
            ].Faces[dir].Corners[corner];
        }
        
        public static Vector2 GetUvForFace(int blockId, int dir, int corner)
        {
            return _blocks[blockId].Faces[dir].Corners[corner];
        }

        /// <summary>
        /// Gets the UV coordinates for a face of a block.
        /// </summary>
        /// <param name="pos">The position within the grid of the atlas.</param>
        /// <param name="corner">What corner of the texture, following the FaceUv convention.</param>
        public static Vector2 AtlasFaceToUv(Vector2 pos, int corner)
        {
            switch (corner)
            {
                case 0:
                    return (pos * GridSize + new Vector2(Margin, Margin)) / AtlasSize;
                case 1:
                    return (pos * GridSize + new Vector2(GridSize - Margin, Margin)) / AtlasSize;
                case 2:
                    return (pos * GridSize + new Vector2(GridSize - Margin, GridSize - Margin)) / AtlasSize;
                case 3:
                    return (pos * GridSize + new Vector2(Margin, GridSize - Margin)) / AtlasSize;
            }
            
            throw new Exception("Invalid corner");
        }
    }
}