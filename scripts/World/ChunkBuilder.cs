using System.Collections.Generic;
using System.Threading;

namespace MC.World
{
    /// <summary>
    /// This class is responsible for the entire process of generating a new chunk,
    /// including the generation of its terrain, lighting, and meshing.
    /// The process is multi-threaded, and the class may take some time before a new
    /// request is done.
    /// </summary>
    public static class ChunkBuilder
    {
        private static class Terrain
        {
            
        }
        
        private static class Lighting
        {
            
        }
        
        private static class Mesh
        {
            
        }

        /// <summary>
        /// Information about a chunk to be built.
        /// </summary>
        private struct Request
        {
            public WorldPosition Position;
            public WorldPosition Center;
            
            public Request(WorldPosition position, WorldPosition center)
            {
                Position = position;
                Center = center;
            }
        }

        /// <summary>
        /// A lock object to ensure that only one set of chunks is being generated at a time.
        /// </summary>
        private static readonly object Lock = new object();
        
        /// <summary>
        /// Requests that are still being queued.
        /// </summary>
        private static List<Request> _requests = new List<Request>();
        
        /// <summary>
        /// Are we adding new requests? If not, a new request will reset the queue.
        /// </summary>
        private static bool _isSendingRequests = false;

        /// <summary>
        /// Requests that are currently being processed.
        /// </summary>
        private static List<Request> _processing = new List<Request>();
        
        /// <summary>
        /// Are we currently building chunks?
        /// </summary>
        private static bool _building = false;

        /// <summary>
        /// Queues a chunk to be built.
        /// </summary>
        /// <param name="position">The chunks position.</param>
        /// <param name="center">The center of the world when queueing this chunk.</param>
        public static void QueueRequest(WorldPosition position, WorldPosition center)
        {
            lock (Lock)
            {
                if (_isSendingRequests == false)
                {
                    _isSendingRequests = true;
                    _requests.Clear();
                }
                
                _requests.Add(new Request(position, center));
            }
        }
        
        /// <summary>
        /// Sends the current block of requests to the sent requests list. <br/>
        /// This will overwrite the sent requests list.
        /// </summary>
        public static void SendRequests()
        {
            Godot.GD.Print($"Sent {_requests.Count} requests.");
            _isSendingRequests = false;
        }

        public static void Update()
        {
            // There might be no requests to build.
            if (_isSendingRequests == false && _requests.Count > 0)
            {
                // We can start building the requests.
                if (_building) return;

                _building = true;
                Thread thread = new Thread(StartBuilding);
                thread.Start();
            }
        }

        private static void StartBuilding()
        {
            // Remember that this is running on a different thread.
            Godot.GD.Print("Starting ChunkBuilder thread.");
            
            // Move the requests to the processing list.
            lock (Lock)
            {
                _processing = _requests;
                _requests = new List<Request>();
            }
            
            // TODO: Make this call a series of tasks, so they can be run in parallel.
            
            // First, we generate all the data for the terrain.
            // Go through the requests in reverse, so we can remove them from the list.
            foreach (var request in _processing)
            {
                GenerateChunk(request);
            }
            
            // Then, we generate the lighting and meshing.
            // The lighting and meshing require knowledge of their adjacent chunks, so we need to
            // wait until all the chunks are generated.
            
            // Go through the requests in reverse, so we can remove them from the list.
            for (int i = _processing.Count - 1; i >= 0; i--)
            {
                BuildChunk(_processing[i]);
                _processing.RemoveAt(i);
            }
        }

        /// <summary>
        /// Generates the terrain for a chunk.
        /// This does not include lighting or meshing. That has to be done after.
        /// </summary>
        private static void GenerateChunk(Request request)
        {
            Godot.GD.Print("Starting ChunkBuilder Generation Task.");
        }
        
        /// <summary>
        /// Builds the lighting and mesh for a chunk.
        /// All the terrain data must be generated before this is called.
        /// </summary>
        private static void BuildChunk(Request request)
        {
            Godot.GD.Print("Starting ChunkBuilder Building Task.");
        }
    }
}