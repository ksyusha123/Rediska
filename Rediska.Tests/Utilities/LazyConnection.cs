﻿namespace Rediska.Tests.Utilities
{
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Protocol;
    using Utils;

    public sealed class LazyConnection : Connection
    {
        private readonly IPEndPoint endPoint;
        private Connection connection;

        public LazyConnection(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
        }

        public override async Task<Resource<Response>> SendAsync(DataType command)
        {
            if (connection == null)
            {
                var tcp = new TcpClient
                {
                    NoDelay = true
                };
                await tcp.ConnectAsync(endPoint.Address, endPoint.Port).ConfigureAwait(false);
                connection = new LoggingConnection(
                    new SimpleConnection(tcp.GetStream())
                );
            }

            return await connection.SendAsync(command).ConfigureAwait(false);
        }

        public override string ToString() => connection == null
            ? $"Not connected: {endPoint}"
            : $"Connected {endPoint}";
    }
}