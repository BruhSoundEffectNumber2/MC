using static MC.GlobalVars;
using System;
using Godot;

using GArray = Godot.Collections.Array;
using GPos = Godot.Vector3;
using BPos = Godot.Vector3;
using CPos = Godot.Vector2;

namespace MC.World
{
    public static class ChunkGenerator
    {
        public static GArray GenerateSurface(ref Chunk[,] chunks, CPos position)
        {
            var chunk = chunks[(int)position.x, (int)position.y];

            var surface = new SurfaceTool();
            surface.Begin(Mesh.PrimitiveType.Triangles);

            for (int y = 0; y < ChunkHeight; y++)
            {
                for (int x = 0; x < ChunkSize; x++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        if (chunk[x, y, z].Solid == false) continue;
                        
                        BPos offset = new BPos(x, y, z);
                        
                        for (int i = 0; i < 6; i++)
                        {
                            BPos vector = VectorFromDirection(i);
                            
                            if (AdjacentSolid(ref chunks, position, offset, vector)) continue;
                            
                            AddFace(ref surface, chunk, BlockTranslation(offset), vector);
                        }
                    }
                }
            }

            surface.GenerateNormals();
            surface.GenerateTangents();
            return surface.CommitToArrays();
        }

        private static bool AdjacentSolid(ref Chunk[,] chunks, CPos position, BPos blockPosition, BPos direction)
        {
            var chunk = chunks[(int)position.x, (int)position.y];
            var checkPosition = blockPosition + direction;

            // We could be looking outside the chunk
            if (checkPosition.x < 0 || checkPosition.x >= ChunkSize ||
                checkPosition.y < 0 || checkPosition.y >= ChunkHeight ||
                checkPosition.z < 0 || checkPosition.z >= ChunkSize)
            {
                // Because there are no adjacent chunks above or below, always draw the face
                if (direction == BlockUp || direction == BlockDown)
                {
                    return false;
                }

                // Find the next chunk over in the direction we are looking
                CPos adjacentChunkPos;
                if (direction == BlockForward)
                {
                    adjacentChunkPos = position + new Vector2(0, 1);
                }
                else if (direction == BlockBackward)
                {
                    adjacentChunkPos = position + new Vector2(0, -1);
                }
                else if (direction == BlockLeft)
                {
                    adjacentChunkPos = position + new Vector2(1, 0);
                }
                else if (direction == BlockRight)
                {
                    adjacentChunkPos = position + new Vector2(-1, 0);
                }
                else
                {
                    throw new Exception("Invalid direction");
                }
                
                // We could be looking at a chunk outside of our render distance
                if (adjacentChunkPos.x < 0 || adjacentChunkPos.x >= GameWorld.RenderDistance * 2 ||
                    adjacentChunkPos.y < 0 || adjacentChunkPos.y >= GameWorld.RenderDistance * 2)
                {
                    // The player shouldn't be able to see the edge, so don't draw any geometry
                    return true;
                }
                
                var adjacentChunk = chunks[(int)adjacentChunkPos.x, (int)adjacentChunkPos.y];
                BPos adjacentCheck;
                
                // Update the check position to be relative to the adjacent chunk
                if (direction == BlockForward)
                {
                    adjacentCheck = new BPos(checkPosition.x, checkPosition.y, 0);
                }
                else if (direction == BlockBackward)
                {
                    adjacentCheck = new BPos(checkPosition.x, checkPosition.y, ChunkSize - 1);
                }
                else if (direction == BlockLeft)
                {
                    adjacentCheck = new BPos(0, checkPosition.y, checkPosition.z);
                }
                else if (direction == BlockRight)
                {
                    adjacentCheck = new BPos(ChunkSize - 1, checkPosition.y, checkPosition.z);
                }
                else
                {
                    throw new Exception("Invalid direction");
                }

                return adjacentChunk[adjacentCheck].Solid;
            }

            return chunk[checkPosition].Solid;
        }

        private static void AddFace(ref SurfaceTool surface, Chunk chunk, GPos offset, BPos direction)
        {
            if (direction == BlockForward)
            {
                surface.AddVertex(offset + new GPos(0, 1, -1));
                surface.AddVertex(offset + new GPos(-1, 0, -1));
                surface.AddVertex(offset + new GPos(0, 0, -1));
                              
                surface.AddVertex(offset + new GPos(0, 1, -1));
                surface.AddVertex(offset + new GPos(-1, 1, -1));
                surface.AddVertex(offset + new GPos(-1, 0, -1));
            } 
            else if (direction == BlockBackward)
            {
                surface.AddVertex(offset + new GPos(0, 1, 0));
                surface.AddVertex(offset + new GPos(0, 0, 0));
                surface.AddVertex(offset + new GPos(-1, 0, 0));

                surface.AddVertex(offset + new GPos(0, 1, 0));
                surface.AddVertex(offset + new GPos(-1, 0, 0));
                surface.AddVertex(offset + new GPos(-1, 1, 0));
            }
            else if (direction == BlockLeft)
            {
                surface.AddVertex(offset + new GPos(-1, 1, -1));
                surface.AddVertex(offset + new GPos(-1, 0, 0));
                surface.AddVertex(offset + new GPos(-1, 0, -1));
            
                surface.AddVertex(offset + new GPos(-1, 1, -1));
                surface.AddVertex(offset + new GPos(-1, 1, 0));
                surface.AddVertex(offset + new GPos(-1, 0, 0));
            }
            else if (direction == BlockRight)
            {
                surface.AddVertex(offset + new GPos(0, 1, -1));
                surface.AddVertex(offset + new GPos(0, 0, -1));
                surface.AddVertex(offset + new GPos(0, 0, 0));

                surface.AddVertex(offset + new GPos(0, 1, -1));
                surface.AddVertex(offset + new GPos(0, 0, 0));
                surface.AddVertex(offset + new GPos(0, 1, 0));
            }
            else if (direction == BlockUp)
            {
                surface.AddVertex(offset + new GPos(0, 1, 0));
                surface.AddVertex(offset + new GPos(-1, 1, -1));
                surface.AddVertex(offset + new GPos(0, 1, -1));
            
                surface.AddVertex(offset + new GPos(0, 1, 0));
                surface.AddVertex(offset + new GPos(-1, 1, 0));
                surface.AddVertex(offset + new GPos(-1, 1, -1));
            }
            else if (direction == BlockDown)
            {
                surface.AddVertex(offset + new GPos(0, 0, 0));
                surface.AddVertex(offset + new GPos(0, 0, -1));
                surface.AddVertex(offset + new GPos(-1, 0, -1));

                surface.AddVertex(offset + new GPos(0, 0, 0));
                surface.AddVertex(offset + new GPos(-1, 0, -1));
                surface.AddVertex(offset + new GPos(-1, 0, 0));
            }
            else
            {
                throw new Exception("Invalid direction");
            }
        }
    }
}