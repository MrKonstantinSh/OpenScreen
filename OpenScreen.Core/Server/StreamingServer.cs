using OpenScreen.Core.Screenshot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using OpenScreen.Core.Mjpeg;

namespace OpenScreen.Core.Server
{
    public class StreamingServer
    {
        private readonly IEnumerable<Image> _images;
        private Thread _thread;

        public int Delay { get; }

        public int NumberOfClients { get; private set; }

        public bool IsRunning
        {
            get => _thread != null && _thread.IsAlive;
            // ReSharper disable once FunctionRecursiveOnAllPaths
            private set => IsRunning = value;
        }

        /// <summary>
        /// The constructor of the class that initializes the fields of the class.
        /// </summary>
        public StreamingServer(Resolution.Resolutions imageResolution, Fps fps, bool isDisplayCursor) 
            : this(Screenshot.Screenshot.TakeSeriesOfScreenshots(imageResolution, isDisplayCursor),
                fps)
        {

        }

        /// <summary>
        /// The constructor of the class that initializes the fields of the class.
        /// </summary>
        private StreamingServer(IEnumerable<Image> images, Fps fps)
        {
            _thread = null;
            _images = images;

            Delay = (int)fps;
        }

        /// <summary>
        /// Starts the server on the specified port.
        /// </summary>
        /// <param name="port">Port.</param>
        public void Start(int port)
        {
            lock (this)
            {
                _thread = new Thread(StartServerThread)
                {
                    IsBackground = true
                };

                _thread.Start(port);
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            try
            {
                _thread.Join();
                _thread.Abort();
            }
            finally
            {
                NumberOfClients = 0;
                IsRunning = false;

                _thread = null;
            }
        }

        /// <summary>
        /// Starts the server in a separate thread.
        /// </summary>
        /// <param name="state">Port.</param>
        private void StartServerThread(object state)
        {
            try
            {
                var server = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                server.Bind(new IPEndPoint(IPAddress.Parse("192.168.100.16"), (int)state));
                server.Listen(1);

                foreach (var client in server.GetIncomingConnections())
                {
                    ThreadPool.QueueUserWorkItem(StartClientThread, client);
                }
            }
            finally
            {
                Stop();
            }
        }

        /// <summary>
        /// Starts a thread to handle clients.
        /// </summary>
        /// <param name="client">Client socket.</param>
        private void StartClientThread(object client)
        {
            var socket = (Socket)client;

            NumberOfClients += 1;

            try
            {
                using (var mjpegWriter = new MjpegWriter(new NetworkStream(socket, true)))
                {

                    // Writes the response header to the client.
                    mjpegWriter.WriteHeaders();

                    // Streams the images from the source to the client.
                    foreach (var imgStream in _images.GetMjpegStream())
                    {
                        Thread.Sleep(Delay);

                        mjpegWriter.WriteImage(imgStream);
                    }
                }
            }
            catch (SocketException)
            {

            }
            catch (IOException)
            {

            }
            finally
            {
                NumberOfClients -= 2;

                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch (ObjectDisposedException)
                {
                    socket.Close();
                }
            }
        }
    }
}
