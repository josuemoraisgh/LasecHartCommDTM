using System;

namespace HartEngine
{
    internal interface IChannel : IDisposable
    {
        void Open();
        byte[] SendAndReceive(byte[] request, int timeoutMs);
    }
}
