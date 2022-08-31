using static MC.GlobalVars;

using System;
using System.Collections.Generic;
using Godot;
using MC.World;

namespace MC.Graphics
{
    /// <summary>
    /// Contains an array of the vertex colors passed to the shader for each face of a block.
    /// </summary>
    public struct BlockTexture
    {
        public Color[] Colors;
        
        public BlockTexture(Color[] colors)
        {
            Colors = colors;
        }
    }
    
    public static class BlockTextureMapper
    {
        // TODO: We will want to have different atlas sizes in the future

        private static List<BlockTexture> _blocks;
        private static Dictionary<string, int> _blockIndices;

        public static void Initialize()
        {
            _blocks = new List<BlockTexture>();
            _blockIndices = new Dictionary<string, int>();
            
            // Load the atlas data
            using (XMLParser parser = new XMLParser())
            {
                parser.Open("res://textures/atlas/atlas_blocks_data.xml");
                int id = 0;
                string name = "";
                var colors = new Color[6];

                while (parser.Read() == Error.Ok)
                {
                    // Find a block tag
                    if (parser.GetNodeType() == XMLParser.NodeType.Element)
                    {
                        switch (parser.GetNodeName())
                        {
                            // A new block, reinitialize the data
                            case "block":
                                colors = new Color[6];
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
                                var pos = new Vector2(float.Parse(x), float.Parse(y));
                                
                                parser.Read();
                                var type = parser.GetNodeData();
                                
                                // We can have several different types of faces
                                switch (type)
                                {
                                    // All faces are the same
                                    case "all":
                                        for (int i = 0; i < 6; i++)
                                        {
                                            colors.SetValue(ColorFromPosition(pos), i);
                                        }
                                        break;
                                    // The front, back, left, and right faces are the same
                                    case "side":
                                        for (int i = 0; i < 4; i++)
                                        {
                                            colors.SetValue(ColorFromPosition(pos), i);
                                        }
                                        break;
                                    // Only the given face is set
                                    default:
                                        colors.SetValue(ColorFromPosition(pos), DirectionFromName(type));
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
                            _blocks.Insert(id, new BlockTexture(colors));
                            _blockIndices.Add(name, id);
                        }
                    }
                }
            }
        }

        public static Color ColorForBlockFace(Block block, int dir)
        {
            // Get the base color for texturing
            Color col = _blocks[block.Type].Colors[dir];
            
            // Set the blue channel based on the face's light factor
            col.b = block.LightFactors[dir];
            
            return col;
        }
        
        /// <summary>
        /// Returns the vertex color that should be used for the given part
        /// of the TextureArray.
        /// </summary>
        private static Color ColorFromPosition(Vector2 position)
        {
            return Color.Color8((byte) (position.x * 4), (byte) (position.y * 4), 0);
        }
    }
}