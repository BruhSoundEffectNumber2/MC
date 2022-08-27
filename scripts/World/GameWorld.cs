﻿using static MC.GlobalVars;
using System.Threading;
using MC.World.Terrain;
using GPos = Godot.Vector3;
using BPos = Godot.Vector3;
using CPos = Godot.Vector2;

namespace MC.World
{
    public static class GameWorld
    {
        public static int RenderDistance => 8;

        private static CPos _localChunksCenter;
        private static Chunk[,] _localChunks;
        
        private static readonly object GenerationLock = new object();
        
        public static void Initialize()
        {
            TerrainGenerator.Initialize();
            _localChunksCenter = CPos.Zero;
            _localChunks = new Chunk[RenderDistance * 2, RenderDistance * 2];
            UpdateChunks();
        }

        private static void UpdateChunks()
        {
            Thread thread = new Thread(() =>
            {
                Monitor.Enter(GenerationLock);
                GenerateChunks();
                MeshChunks();
                Monitor.Exit(GenerationLock);
            });
            
            thread.Start();
        }

        private static void GenerateChunks()
        {
            for (int x = 0; x < RenderDistance * 2; x++)
            {
                for (int y = 0; y < RenderDistance * 2; y++)
                {
                    // We might not have generated the chunk before
                    if (_localChunks[x, y] == null)
                    {
                        var worldPosition = new CPos(x - RenderDistance, y - RenderDistance);
                        _localChunks[x, y] = new Chunk(worldPosition);
                    }
                }
            }
        }
        
        private static void MeshChunks()
        {
            for (int x = 0; x < RenderDistance * 2; x++)
            {
                for (int y = 0; y < RenderDistance * 2; y++)
                {
                    var surface = ChunkGenerator.GenerateSurface(ref _localChunks, new CPos(x, y));
                    
                    var worldPosition = new CPos(x - RenderDistance, y - RenderDistance);
                    WorldManager.AddChunk(worldPosition, surface);
                }
            }
        }
    }
}