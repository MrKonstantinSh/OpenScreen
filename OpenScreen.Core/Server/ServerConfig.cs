namespace OpenScreen.Core.Server
{
    internal class ServerConfig
    {
        public string IpAddress { get; }

        public int Port { get; }

        public ServerConfig(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }
    }
}
