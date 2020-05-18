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
        private static readonly object s_syncRoot = new object();
        private static StreamingServer s_serverInstance;

        private IEnumerable<Image> _images;
        private Socket _serverSocket;
        private Thread _thread;

        public int Delay { get; }

        public List<Socket> Clients { get; }

        public bool IsRunning => _thread != null && _thread.IsAlive;

        /// <summary>
        /// Initializes the fields and properties of the class for the screen stream.
        /// </summary>
        private StreamingServer(Resolution.Resolutions imageResolution, Fps fps, bool isDisplayCursor)
            : this(Screenshot.Screenshot.TakeSeriesOfScreenshots(imageResolution, isDisplayCursor), fps)
        {

        }

        /// <summary>
        /// Initializes the fields and properties of the class to stream a specific application window.
        /// </summary>
        private StreamingServer(string applicationName, Fps fps, bool isDisplayCursor)
            : this(Screenshot.Screenshot.TakeSeriesOfScreenshotsAppWindow(applicationName, isDisplayCursor), fps)
        {

        }

        /// <summary>
        /// The constructor of the class that initializes the fields of the class.
        /// </summary>
        private StreamingServer(IEnumerable<Image> images, Fps fps)
        {
            _thread = null;
            _images = images;

            Clients = new List<Socket>();
            Delay = (int)fps;
        }

        /// <summary>
        /// Provides a server object for a screen stream.
        /// </summary>
        /// <param name="resolutions">Stream Resolution.</param>
        /// <param name="fps">FPS.</param>
        /// <param name="isDisplayCursor">Whether to display the cursor in screenshots.</param>
        /// <returns>The object of the StreamingServer class.</returns>
        public static StreamingServer GetInstance(Resolution.Resolutions resolutions,
            Fps fps, bool isDisplayCursor)
        {
            lock (s_syncRoot)
            {
                s_serverInstance ??= new StreamingServer(resolutions, fps, isDisplayCursor);
            }

            return s_serverInstance;
        }

        /// <summary>
        /// Provides an object for a window stream of a specific application.
        /// </summary>
        /// <param name="applicationName">The title of the main application window.</param>
        /// <param name="fps">FPS.</param>
        /// <param name="isDisplayCursor">Whether to display the cursor in screenshots.</param>
        /// <returns>The object of the StreamingServer class.</returns>
        public static StreamingServer GetInstance(string applicationName,
            Fps fps, bool isDisplayCursor)
        {
            lock (s_syncRoot)
            {
                s_serverInstance ??= new StreamingServer(applicationName, fps, isDisplayCursor);
            }

            return s_serverInstance;
        }

        /// <summary>
        /// Starts the server on the specified port.
        /// </summary>
        /// <param name="ipAddress">The IP address on which to start the server.</param>
        /// <param name="port">Server port.</param>
        public void Start(string ipAddress, int port)
        {
            var serverConfig = new ServerConfig(ipAddress, port);

            lock (this)
            {
                _thread = new Thread(StartServerThread)
                {
                    IsBackground = true
                };

                _thread.Start(serverConfig);
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
                _serverSocket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                _serverSocket.Close();
            }
            finally
            {
                _thread = null;
                _images = null;
                s_serverInstance = null;
            }
        }

        /// <summary>
        /// Starts the server in a separate thread.
        /// </summary>
        /// <param name="config">IP address and port on which you want to start the server.</param>
        private void StartServerThread(object config)
        {
            var serverConfig = (ServerConfig)config;

            try
            {
                _serverSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(serverConfig.IpAddress),
                    serverConfig.Port));
                _serverSocket.Listen(10);

                foreach (var client in _serverSocket.GetIncomingConnections())
                {
                    ThreadPool.QueueUserWorkItem(StartClientThread, client);
                }
            }
            catch (SocketException)
            {
                foreach (var client in Clients.ToArray())
                {
                    try
                    {
                        client.Shutdown(SocketShutdown.Both);
                    }
                    catch (ObjectDisposedException)
                    {
                        client.Close();
                    }

                    Clients.Remove(client);
                }
            }
        }

        /// <summary>
        /// Starts a thread to handle clients.
        /// </summary>
        /// <param name="client">Client socket.</param>
        private void StartClientThread(object client)
        {
            var clientSocket = (Socket)client;
            clientSocket.SendTimeout = 10000;

            Clients.Add(clientSocket);

            try
            {
                using var mjpegWriter = new MjpegWriter(new NetworkStream(clientSocket, true));
                // Writes the response header to the client.
                mjpegWriter.WriteHeaders();

                // Streams the images from the source to the client.
                foreach (var imgStream in _images.GetMjpegStream())
                {
                    Thread.Sleep(Delay);

                    mjpegWriter.WriteImage(imgStream);
                }
            }
            catch (SocketException)
            {
               
            }
            catch (IOException)
            {
                
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                try
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                }
                catch (ObjectDisposedException)
                {
                    clientSocket.Close();
                }

                lock (Clients)
                {
                    Clients.Remove(clientSocket);
                }
            }
        }
    }
}
