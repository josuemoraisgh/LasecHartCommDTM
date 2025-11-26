using System;
using System.Runtime.InteropServices;

namespace FakeHartCommDTM
{
    // Interface COM que PACTware / outro cliente pode chamar
    [Guid("F8E2DAE6-1C7A-4D3E-9C8C-7A9A2BC4A001")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComVisible(true)]
    public interface IHartComm
    {
        // mode: "serial" ou "udp"
        void Configure(string mode, string serialPort, string udpHost, int udpPort, int baudRate);

        // Envia 1 frame HART (em bytes) e retorna resposta (ou array vazio se timeout)
        byte[] SendAndReceive(byte[] request, int timeoutMs);
    }

    // Classe COM concreta
    [Guid("F8E2DAE6-1C7A-4D3E-9C8C-7A9A2BC4A002")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class HartComm : IHartComm
    {
        private ChannelManager _manager = new ChannelManager();

        public void Configure(string mode, string serialPort, string udpHost, int udpPort, int baudRate)
        {
            _manager.Configure(mode, serialPort, udpHost, udpPort, baudRate);
        }

        public byte[] SendAndReceive(byte[] request, int timeoutMs)
        {
            return _manager.SendAndReceive(request, timeoutMs);
        }
    }
}
