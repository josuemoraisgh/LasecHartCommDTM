using System;
using System.IO.Ports;
using System.Threading;

namespace HartEngine
{
    internal class SerialChannel : IChannel
    {
        private readonly string _portName;
        private readonly int _baudRate;
        private SerialPort _port;

        public SerialChannel(string portName, int baudRate)
        {
            _portName = portName;
            _baudRate = baudRate <= 0 ? 9600 : baudRate;
        }

        public void Open()
        {
            _port?.Dispose();
            _port = new SerialPort(_portName, _baudRate, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 100,
                WriteTimeout = 100
            };
            _port.Open();
        }

        public byte[] SendAndReceive(byte[] request, int timeoutMs)
        {
            if (_port == null || !_port.IsOpen)
                throw new InvalidOperationException("Serial port not open");

            _port.DiscardInBuffer();
            _port.Write(request, 0, request.Length);

            var buffer = new byte[256];
            int totalRead = 0;
            var start = Environment.TickCount;

            while (Environment.TickCount - start < timeoutMs)
            {
                try
                {
                    int b = _port.ReadByte();
                    if (b >= 0)
                    {
                        if (totalRead == buffer.Length)
                        {
                            Array.Resize(ref buffer, buffer.Length * 2);
                        }
                        buffer[totalRead++] = (byte)b;
                    }
                }
                catch (TimeoutException)
                {
                    Thread.Sleep(5);
                }
            }

            var result = new byte[totalRead];
            Array.Copy(buffer, result, totalRead);
            return result;
        }

        public void Dispose()
        {
            try { _port?.Close(); }
            catch { }
            _port?.Dispose();
        }
    }
}
