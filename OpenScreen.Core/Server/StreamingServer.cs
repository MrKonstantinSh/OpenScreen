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
        private readonly List<Socket> _clients;
        private readonly IEnumerable<Image> _images;
        private Thread _thread;

        public int Delay { get; }

        public IEnumerable<Socket> Clients
        {
            get
            {
                lock (_clients)
                {
                    return _clients ?? null;
                }
            }
        }

        public bool IsRunning => _thread != null && _thread.IsAlive;

        /// <summary>
        /// The constructor of the class that initializes the fields of the class.
        /// </summary>
        public StreamingServer() : this(Screenshot.Screenshot.TakeSeriesOfScreenshots())
        {

        }

        /// <summary>
        /// The constructor of the class that initializes the fields of the class.
        /// </summary>
        private StreamingServer(IEnumerable<Image> images)
        {
            _clients = new List<Socket>();
            _thread = null;
            _images = images;

            Delay = (int) Fps.Sixty;
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
                lock (_clients)
                {
                    foreach (var socket in _clients)
                    {
                        try
                        {
                            socket.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    _clients.Clear();
                }

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
                server.Listen(10);

                foreach (var client in ServerSocketExtension.GetIncomingConnections(server))
                    ThreadPool.QueueUserWorkItem(StartClientThread, client);
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

            lock (_clients)
            {
                _clients.Add(socket);
            }

            try
            {
                using (var wr = new MjpegWriter(new NetworkStream(socket)))
                {

                    // Writes the response header to the client.
                    wr.WriteHeaders();

                    // Streams the images from the source to the client.
                    foreach (var imgStream in _images.GetMjpegStream())
                    {
                        Thread.Sleep(Delay);

                        wr.WriteImage(imgStream);
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
                lock (_clients)
                {
                    _clients?.Remove(socket);
                }
            }
        }
    }
}
