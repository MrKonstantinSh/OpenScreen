using System.Net;

namespace OpenScreen.Core.Server
{
    internal class ServerConfig
    {
        public IPAddress IpAddress { get; }

        public int Port { get; }

        public ServerConfig(IPAddress ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }
    }
}
