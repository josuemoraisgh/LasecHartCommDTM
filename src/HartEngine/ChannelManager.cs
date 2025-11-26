using System;

namespace HartEngine
{
    public class ChannelManager : IDisposable
    {
        private IChannel _channel;
        private string _mode;
        private string _serialPort;
        private string _udpHost;
        private int _udpPort;
        private int _baudRate;

        public void Configure(string mode, string serialPort, string udpHost, int udpPort, int baudRate)
        {
            _mode = mode?.ToLowerInvariant();
            _serialPort = serialPort;
            _udpHost = udpHost;
            _udpPort = udpPort;
            _baudRate = baudRate <= 0 ? 9600 : baudRate;

            _channel?.Dispose();
            _channel = null;

            if (_mode == "serial")
            {
                _channel = new SerialChannel(_serialPort, _baudRate);
            }
            else if (_mode == "udp")
            {
                _channel = new UdpChannel(_udpHost, _udpPort);
            }
            else
            {
                throw new ArgumentException("mode must be 'serial' or 'udp'");
            }

            _channel.Open();
        }

        public byte[] SendAndReceive(byte[] request, int timeoutMs)
        {
            if (_channel == null)
            {
                throw new InvalidOperationException("Channel not configured. Call Configure() first.");
            }
            return _channel.SendAndReceive(request, timeoutMs);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _channel = null;
        }
    }
}
